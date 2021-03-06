﻿using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using mservicesample.Membership.Core.DataAccess.Entities;
using mservicesample.Membership.Core.DataAccess.Repositories;
using mservicesample.Membership.Core.Dtos.Mapping;
using mservicesample.Membership.Core.Helpers;
using mservicesample.Membership.Core.Services;
using mservicesample.Membership.Core.Settings;

namespace mservicesample.Membership.Api.Settings
{
    public static class ServiceRegistrations
    {

        public static IServiceCollection RegisterHealthCheck(this IServiceCollection services, string connectionstring, string apiurl)
        {
            //health ckeck configuration
            services.AddHealthChecks()
                .AddSqlServer(connectionString: connectionstring,
                    healthQuery: "SELECT 1;",
                    name: "Sql Server",
                    failureStatus: HealthStatus.Degraded)
                .AddUrlGroup(new Uri(apiurl + "api/Health/Check"),
                    name: "Health URL",
                    failureStatus: HealthStatus.Degraded);
            //add healthcheck ui
            services.AddHealthChecksUI(setupSettings: setup =>
            {
                setup.AddHealthCheckEndpoint("Basic healthcheck", apiurl + "alive");
            });
            return services;
        }

        public static IServiceCollection RegisterSwagger(this IServiceCollection services)
        {
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",new OpenApiInfo{ Title = "Auth API", Version = "v1" });
                // Swagger 2.+ support
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });

            return services;
        }

        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRolesService, RolesService>();
            services.AddScoped<IUserRoleService, UserRoleService>();
            services.AddScoped<IUserDetailsRepository, UserDetailsRepository>();
            services.AddScoped<UserManager<AppUser>>();
            services.AddScoped<RoleManager<AppRole>>();

            services.AddScoped<UserManager<AppUser>, UserManager<AppUser>>();
            services.AddScoped<RoleManager<AppRole>, RoleManager<AppRole>>();
            services.TryAddScoped<IRoleValidator<AppRole>, RoleValidator<AppRole>>();
            services.TryAddScoped<IUserClaimsPrincipalFactory<AppUser>, UserClaimsPrincipalFactory<AppUser, AppRole>>();

            services.AddScoped<IUserClaimsPrincipalFactory<IdentityUser>, UserClaimsPrincipalFactory<IdentityUser, IdentityRole>>();

            services.AddScoped<ILoginService, LoginService>();
            services.AddSingleton<JwtIssuerOptions>();
            services.AddSingleton<ITokenGenerator, TokenGenerator>();
            services.AddSingleton<ILogger, Logger>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new DataMapping());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            //services.AddHttpContextAccessor();
            //// Identity services
            //services.TryAddScoped<IUserValidator<AppUser>, UserValidator<AppUser>>();
            //services.TryAddScoped<IPasswordValidator<AppUser>, PasswordValidator<AppUser>>();
            //services.TryAddScoped<IPasswordHasher<AppUser>, PasswordHasher<AppUser>>();
            //services.TryAddScoped<ILookupNormalizer, UpperInvariantLookupNormalizer>();
            //services.TryAddScoped<IRoleValidator<AppRole>, RoleValidator<AppRole>>();
            //// No interface for the error describer so we can add errors without rev'ing the interface
            //services.TryAddScoped<IdentityErrorDescriber>();
            //services.TryAddScoped<ISecurityStampValidator, SecurityStampValidator<AppUser>>();
            //services.TryAddScoped<ITwoFactorSecurityStampValidator, TwoFactorSecurityStampValidator<AppUser>>();
            //services.TryAddScoped<IUserClaimsPrincipalFactory<AppUser>, UserClaimsPrincipalFactory<AppUser, AppRole>>();
            //services.TryAddScoped<UserManager<AppUser>>();
            //services.TryAddScoped<SignInManager<AppUser>>();
            //services.TryAddScoped<RoleManager<AppRole>>();

            return services;
        }
    }
}
