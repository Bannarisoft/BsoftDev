using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Net;
using FluentAssertions;
using UserManagement.IntegrationTests.Factories;
using Microsoft.Extensions.DependencyInjection;
using UserManagement.Infrastructure.Data;
using Core.Domain.Entities;
using System.Linq;
using Grpc.Core;

namespace UserManagement.IntegrationTests.Tests.Users
{
    [TestClass]
    public class CreateUserIntegrationTests
    {
        private static HttpClient _client;
        private static CustomWebApplicationFactory _factory;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            _factory = new CustomWebApplicationFactory();
            _client = _factory.CreateClient();

            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            SeedTestLookupData(dbContext);
        }

        private static void SeedTestLookupData(ApplicationDbContext db)
        {
            // Avoid duplicate seeding
            if (!db.Companies.Any())
            {
                db.Companies.Add(new Company { Id = 1, CompanyName = "Test Co" });
                db.Department.Add(new Department { Id = 1, DeptName = "IT", DepartmentGroupId = 1, CreatedByName = "test", CreatedIP = "127.0.0.1", IsActive = Core.Domain.Enums.Common.Enums.Status.Active, IsDeleted = Core.Domain.Enums.Common.Enums.IsDelete.NotDeleted });
                db.Divisions.Add(new Division { Id = 1, Name = "Tech" });
                db.Unit.Add(new Unit { Id = 1, UnitName = "HQ" });
                db.UserRole.Add(new UserRole { Id = 1, RoleName = "Admin" });

                // Optional: Add DepartmentGroup if there's a FK constraint
                if (!db.DepartmentGroup.Any())
                {
                    db.DepartmentGroup.Add(new DepartmentGroup { Id = 1, DepartmentGroupName = "Default Group" });
                }

                db.SaveChanges();
            }
        }

        [TestMethod]
        public async Task PostUser_ShouldReturnCreated()
        {
            // Arrange
            var user = new
            {
                FirstName = "John",
                LastName = "Doe",
                EmailId = "john.doe@example.com",
                UserName = "johndoe",
                Password = "Secure@123",
                ConfirmPassword = "Secure@123",
                RoleId = 1,
                CompanyId = 1,
                DepartmentId = 1,
                DivisionId = 1,
                UnitId = 1
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/User", user);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }
    }
}
