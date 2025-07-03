using System;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IUser;
using Core.Domain.Entities;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UserManagement.Tests.UnitTests.Services
{
    [TestClass]
    public class UserQueryServiceTests
    {
        private Mock<IUserQueryRepository> _userRepositoryMock = null!;
        private IUserQueryRepository _userService = null!;

        [TestInitialize]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserQueryRepository>();
            _userService = _userRepositoryMock.Object;
        }
        [TestCleanup]
        public void Cleanup()
        {
            _userRepositoryMock = null!;
            _userService = null!;
        }

        private User GenerateTestUser(int userId)
        {
            return new User
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                UserName = $"User_{userId}",
                EmailId = $"user{userId}@example.com",
                Mobile = "9876543210",
                IsLocked = 0,
                IsFirstTimeUser = Core.Domain.Enums.Common.Enums.FirstTimeUserStatus.Yes
            };
        }

        [TestMethod]
        public async Task GetUserByIdAsync_ShouldReturnUser_WhenExists()
        {
            // Arrange
            var userId = 1;
            var expectedUser = GenerateTestUser(userId);

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(It.Is<int>(id => id == userId)))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _userService.GetByIdAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result!.UserId.Should().Be(expectedUser.UserId);
            result.UserName.Should().Be(expectedUser.UserName);
            result.EmailId.Should().Be(expectedUser.EmailId);
        }

        [TestMethod]
        public async Task GetUserByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            var userId = 99;

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(It.Is<int>(id => id == userId)))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _userService.GetByIdAsync(userId);

            // Assert
            result.Should().BeNull();
        }
    }
}
