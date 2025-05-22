using Core.Application.Common.Interfaces.IUser;
using Core.Domain.Entities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace UserManagement.Tests.UnitTests.Services
{
    [TestClass]
    public class UserCommandServiceTests
    {
        private Mock<IUserCommandRepository> _userCommandRepoMock = null!;
        private IUserCommandRepository _userCommandService = null!;

        [TestInitialize]
        public void Setup()
        {
            _userCommandRepoMock = new Mock<IUserCommandRepository>();
            _userCommandService = _userCommandRepoMock.Object;
        }

        private User GenerateTestUser(int userId)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                FirstName = "First",
                LastName = "Last",
                UserName = $"user_{userId}",
                EmailId = $"user{userId}@example.com",
                Mobile = "1234567890",
                IsLocked = 0,
                IsFirstTimeUser = Core.Domain.Enums.Common.Enums.FirstTimeUserStatus.Yes,
                UserType = 1,
                EntityId = 1,
                UserGroupId = 1,
                userDivisions = new List<UserDivision>(),
                userDepartments = new List<UserDepartment>(),
                UserRoleAllocations = new List<UserRoleAllocation>(),
                UserCompanies = new List<UserCompany>(),
                UserUnits = new List<UserUnit>(),
                Passwords = new List<PasswordLog>()
            };
            user.SetPassword("Password@123");
            return user;
        }

        [TestMethod]
        public async Task CreateUserAsync_ShouldReturnUser_WhenUserIsCreated()
        {
            // Arrange
            var newUser = GenerateTestUser(0);
            var expectedUser = GenerateTestUser(10);

            _userCommandRepoMock
                .Setup(repo => repo.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _userCommandService.CreateAsync(newUser);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(expectedUser.Id);
            result.UserId.Should().Be(expectedUser.UserId);
            result.UserName.Should().Be(expectedUser.UserName);
            result.EmailId.Should().Be(expectedUser.EmailId);
        }

        [TestMethod]
        public async Task UpdateUserAsync_ShouldReturn1_WhenUpdateIsSuccessful()
        {
            // Arrange
            var updatedUser = GenerateTestUser(1);

            _userCommandRepoMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<int>(), It.IsAny<User>()))
                .ReturnsAsync(1);

            // Act
            var result = await _userCommandService.UpdateAsync(updatedUser.UserId, updatedUser);

            // Assert
            result.Should().Be(1);
        }

        [TestMethod]
        public async Task UpdateUserAsync_ShouldReturn0_WhenUserNotFound()
        {
            // Arrange
            var nonExistentUser = GenerateTestUser(9999);

            _userCommandRepoMock
                .Setup(repo => repo.UpdateAsync(It.IsAny<int>(), It.IsAny<User>()))
                .ReturnsAsync(0);

            // Act
            var result = await _userCommandService.UpdateAsync(nonExistentUser.UserId, nonExistentUser);

            // Assert
            result.Should().Be(0);
        }
    }
}
