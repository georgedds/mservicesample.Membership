using System;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using mservicesample.Membership.Api.Settings;
using mservicesample.Membership.Core.DataAccess;
using mservicesample.Membership.Core.DataAccess.Entities;
using mservicesample.Membership.Core.DataAccess.Identity;
using mservicesample.Membership.Core.DataAccess.Repositories;
using mservicesample.Membership.Core.Helpers;
using mservicesample.Membership.Core.Middleware;
using mservicesample.Membership.Core.Services;
using mservicesample.Membership.Core.Settings;

namespace mservicesample.Membership.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            var appSettings = appSettingsSection.Get<AppSettings>();
            var secretkey = Encoding.ASCII.GetBytes(appSettings.Secret);
            var signingKey = new SymmetricSecurityKey(secretkey);

             // Add framework services.
            services.AddDbContext<AppIdentityDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Default"), b => b.MigrationsAssembly("mservicesample.Membership")));
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Default"), b => b.MigrationsAssembly("mservicesample.Membership")));


            services.AddCors();

            // jwt wire up
            // Get options from app settings
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.Expiresinminutes = Convert.ToInt32( jwtAppSettingOptions[nameof(JwtIssuerOptions.Expiresinminutes)]);
                options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature);
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                //ValidateIssuer = true,
                //ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                //ValidateAudience = true,
                //ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                //ValidateIssuerSigningKey = true,
                //IssuerSigningKey = signingKey,

                //RequireExpirationTime = false,
                //ValidateLifetime = true,
                //ClockSkew = TimeSpan.Zero

                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],
                IssuerSigningKey = new SymmetricSecurityKey(secretkey)

            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(configureOptions =>
            {
                configureOptions.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                configureOptions.TokenValidationParameters = tokenValidationParameters;
                configureOptions.SaveToken = true;

                configureOptions.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            // api user claim policy
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiUser", policy => policy.RequireClaim(Core.Helpers.Constants.Strings.JwtClaimIdentifiers.Rol, Core.Helpers.Constants.Strings.JwtClaims.ApiAccess));
            });


            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();


            services.AddIdentityCore<AppUser>()
                .AddRoles<AppRole>()
                .AddRoleManager<RoleManager<AppRole>>()
                .AddUserManager<UserManager<AppUser>>()
                .AddClaimsPrincipalFactory<UserClaimsPrincipalFactory<AppUser, IdentityRole>>()
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();
          
            //add healthcheck
            services.RegisterHealthCheck(Configuration.GetConnectionString("Default"), jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)]);
            
            services.AddMvc();
            services.AddAutoMapper(typeof(Startup));


            //Add swagger
            services.RegisterSwagger();
            //register our services
            services.RegisterServices();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Membership API V1");
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();


            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());


            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware(typeof(ExceptionHandlingMiddleware));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //add healthcheck options
            app.UseHealthChecks("/alive", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            app.UseHealthChecksUI(); //http://localhost:5000/healthchecks-ui#/healthchecks
        }
    }
}
