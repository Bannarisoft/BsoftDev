using System.Linq;
using System.Threading.Tasks;
using Core.Application.Users.Commands.CreateUser;
using Core.Application.Common.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UserManagement.API.Validation.Users;
using Core.Application.Common.Interfaces.IUser;
using Core.Application.Common.Interfaces.ICompany;
using Core.Application.Common.Interfaces.IDivision;
using Core.Application.Common.Interfaces.IDepartment;
using Core.Application.Common.Interfaces.IUserRole;
using Core.Application.Common.Interfaces.IUnit;
using UserManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using UserManagement.API.Validation.Common;
using Moq;

namespace UserManagement.SecurityTest.Validation
{
    [TestClass]
    public class InputSanitizationTests
    {
        [TestMethod]
        public async Task ShouldReject_SQLInjection_Username()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                UserName = "'; DROP TABLE Users;--",
                EmailId = "test@example.com"
            };

            // Setup required services
            var ipAddressServiceMock = new Mock<IIPAddressService>();
            var timeZoneServiceMock = new Mock<ITimeZoneService>();

            // Setup in-memory DbContext
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            var dbContext = new ApplicationDbContext(options, ipAddressServiceMock.Object, timeZoneServiceMock.Object);
            var maxLengthProvider = new MaxLengthProvider(dbContext);

            // Mock other dependencies
            var userQueryRepository = new Mock<IUserQueryRepository>();
            var companyQueryRepository = new Mock<ICompanyQueryRepository>();
            var divisionQueryRepository = new Mock<IDivisionQueryRepository>();
            var departmentQueryRepository = new Mock<IDepartmentQueryRepository>();
            var userRoleQueryRepository = new Mock<IUserRoleQueryRepository>();
            var unitQueryRepository = new Mock<IUnitQueryRepository>();

            var validator = new CreateUserCommandValidator(
                maxLengthProvider,
                userQueryRepository.Object,
                companyQueryRepository.Object,
                divisionQueryRepository.Object,
                departmentQueryRepository.Object,
                userRoleQueryRepository.Object,
                unitQueryRepository.Object,
                ipAddressServiceMock.Object
            );

            // Act
            var result = await validator.ValidateAsync(command);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(e => e.PropertyName == "UserName"));
        }
    }
}
