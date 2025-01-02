using BSOFT.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using Core.Application.Common.Interfaces;
using BSOFT.Infrastructure.Repositories;
using MongoDB.Driver;
namespace BSOFT.Infrastructure
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // Build configuration
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../BSOFT.Api"))
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString);


              // MongoDB Configuration
            var mongoSection = configuration.GetSection("MongoDb");
            var mongoConnectionString = mongoSection.GetValue<string>("MongoDbConnectionString");
            var mongoDatabaseName = mongoSection.GetValue<string>("DatabaseName");

            if (string.IsNullOrWhiteSpace(mongoConnectionString) || string.IsNullOrWhiteSpace(mongoDatabaseName))
            {
                throw new InvalidOperationException("MongoDB connection string or database name not found in configuration.");
            }

            var mongoClient = new MongoClient(mongoConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDatabaseName);

            IIPAddressService ipAddressService = new IPAddressService();
            return new ApplicationDbContext(optionsBuilder.Options, ipAddressService);
        }
    }
}
