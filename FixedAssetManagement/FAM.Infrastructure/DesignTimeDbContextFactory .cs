using FAM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using Core.Application.Common.Interfaces;
using FAM.Infrastructure.Repositories;
using MongoDB.Driver;
using Microsoft.AspNetCore.Http;
using FAM.Infrastructure.Services;
using System.Text;
using System.Security.Cryptography;
using FAM.Infrastructure.Helpers;

namespace FAM.Infrastructure
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            // Build configuration
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../FAM.Api"))
                .AddJsonFile($"appsettings.{environment}.json", optional: false, reloadOnChange: true)
                .Build();

          
            var connectionString = ConnectionStringHelper.GetDefaultConnectionString(configuration);

            optionsBuilder.UseSqlServer(connectionString);

            IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();
            IIPAddressService ipAddressService = new IPAddressService(httpContextAccessor);
            ITimeZoneService timeZoneService = new TimeZoneService();

            return new ApplicationDbContext(optionsBuilder.Options, ipAddressService, timeZoneService);
        }
    }
}
