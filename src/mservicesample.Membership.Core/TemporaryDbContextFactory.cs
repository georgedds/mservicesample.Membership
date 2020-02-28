using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using mservicesample.Membership.Core.DataAccess.Identity;
using Microsoft.Extensions.Configuration;
using mservicesample.Membership.Core.DataAccess;

namespace mservicesample.Membership.Core
{
    public class AppIdentityDbContextFactory : IDesignTimeDbContextFactory<AppIdentityDbContext>
    {
        public AppIdentityDbContext CreateDbContext(string[] args)
        {
          
            //local path for migrations
            var basepath =
                @"C:\george\projects\μservicesample\mservicesample.Membership\src\mservicesample.Membership.Core\";


            var builder = new ConfigurationBuilder()
                .SetBasePath(basepath)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            var config = builder.Build();

            var connstr = config.GetConnectionString("Default");


            var dbbuilder = new DbContextOptionsBuilder<AppIdentityDbContext>();
            
            dbbuilder.UseSqlServer(connstr,
                optionsBuilder => optionsBuilder.MigrationsAssembly(typeof(AppIdentityDbContext).GetTypeInfo().Assembly.GetName().Name));
            return new AppIdentityDbContext(dbbuilder.Options);
        }
    }

    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            //local path for migrations
            var basepath =
                @"C:\george\projects\μservicesample\mservicesample.Membership\src\mservicesample.Membership.Core\";


            var builder = new ConfigurationBuilder()
                .SetBasePath(basepath)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            var config = builder.Build();

            var connstr = config.GetConnectionString("Default");


            var dbbuilder = new DbContextOptionsBuilder<AppDbContext>();
            
            dbbuilder.UseSqlServer(connstr,
                optionsBuilder => optionsBuilder.MigrationsAssembly(typeof(AppIdentityDbContext).GetTypeInfo().Assembly.GetName().Name));
            return new AppDbContext(dbbuilder.Options);
        }
    }
}
