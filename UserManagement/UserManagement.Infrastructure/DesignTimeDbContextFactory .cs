using UserManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using Core.Application.Common.Interfaces;
using UserManagement.Infrastructure.Repositories;
using MongoDB.Driver;
using Microsoft.AspNetCore.Http;
using UserManagement.Infrastructure.Services;
// using UserManagement.Infrastructure.Helpers;  // Ensure this is included if needed for IHttpContextAccessor

namespace UserManagement.Infrastructure
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";                        
            // Build configuration
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../UserManagement.Api"))
                .AddJsonFile($"appsettings.{environment}.json", optional: false, reloadOnChange: true)
                .Build();

        //   var connectionString = ConnectionStringHelper.GetDefaultConnectionString(configuration);

        //     optionsBuilder.UseSqlServer(connectionString);

            IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();
            // Create a dummy or mock IPAddressService implementation
            IIPAddressService ipAddressService = new IPAddressService(httpContextAccessor);
            ITimeZoneService timeZoneService = new TimeZoneService();

            return new ApplicationDbContext(optionsBuilder.Options, ipAddressService,timeZoneService);  // Pass both dependencies
            //return new ApplicationDbContext(optionsBuilder.Options);  // Pass both dependencies
        }
    }
}
