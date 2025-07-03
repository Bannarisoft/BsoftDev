using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IUser;
using Core.Application.Common.Interfaces;
using Core.Application.Users.Commands.ResetUserPassword;
using Core.Application.Users.Queries.GetUsers;
using Core.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using BackgroundService.Application;

namespace Core.Application.Tests.Users.Commands
{
    [TestClass]
    public class ResetUserPasswordCommandHandlerTests
    {
        private Mock<IMapper>? _mockMapper;
        private Mock<IChangePassword>? _mockChangePassword;
        private Mock<IUserQueryRepository>? _mockUserQueryRepository;
        private Mock<ITimeZoneService>? _mockTimeZoneService;
        private ResetUserPasswordCommandHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockMapper = new Mock<IMapper>();
            _mockChangePassword = new Mock<IChangePassword>();
            _mockUserQueryRepository = new Mock<IUserQueryRepository>();
            _mockTimeZoneService = new Mock<ITimeZoneService>();

            _handler = new ResetUserPasswordCommandHandler(
                _mockMapper.Object,
                _mockChangePassword.Object,
                _mockUserQueryRepository.Object,
                _mockTimeZoneService.Object);
        }

        [TestMethod]
        public async Task Handle_ShouldReturnSuccess_WhenPasswordResetIsSuccessful()
        {
            // Arrange
            var command = new ResetUserPasswordCommand
            {
                UserName = "testuser",
                Password = "newPassword123",
                VerificationCode = "ABC123"
            };

            var user = new User
            {
                UserId = 1,
                UserName = "testuser",
                PasswordHash = "oldhash"
            };

            var currentTime = DateTime.UtcNow;

            _mockTimeZoneService!.Setup(tz => tz.GetSystemTimeZone()).Returns("UTC");
            _mockTimeZoneService.Setup(tz => tz.GetCurrentTime("UTC")).Returns(currentTime);

            _mockUserQueryRepository!.Setup(repo => repo.GetByUsernameAsync(command.UserName))
                .ReturnsAsync(user);

            _mockChangePassword!.Setup(cp => cp.PasswordEncode(command.Password))
                .ReturnsAsync("newhash");

            // Setup mapping from PasswordLogDTO to PasswordLog
            _mockMapper!.Setup(m => m.Map<PasswordLog>(It.IsAny<PasswordLogDTO>()))
                .Returns<PasswordLogDTO>(dto => new PasswordLog
                {
                    UserId = dto.UserId,
                    UserName = dto.UserName,
                    PasswordHash = dto.PasswordHash,
                    CreatedAt = dto.CreatedAt
                });

            _mockChangePassword.Setup(cp => cp.ResetUserPassword(user.UserId, It.IsAny<PasswordLog>()))
                .ReturnsAsync("success");

            _mockChangePassword.Setup(cp => cp.PasswordLog(It.IsAny<PasswordLog>()))
                .ReturnsAsync(true);

            // Setup cache with verification code (simulate the existing code)
            ForgotPasswordCache.CodeStorage[command.UserName] = new VerificationCodeDetails
            {
                Code = command.VerificationCode,
                ExpiryTime = currentTime.AddMinutes(5)
            };

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("Password reset successfully.", result.Message);
            Assert.IsFalse(!ForgotPasswordCache.CodeStorage.ContainsKey(command.UserName));

            _mockUserQueryRepository.Verify(r => r.GetByUsernameAsync(command.UserName), Times.Once);
            _mockChangePassword.Verify(cp => cp.PasswordEncode(command.Password), Times.Once);
            _mockChangePassword.Verify(cp => cp.ResetUserPassword(user.UserId, It.IsAny<PasswordLog>()), Times.Once);
            _mockChangePassword.Verify(cp => cp.PasswordLog(It.IsAny<PasswordLog>()), Times.Once);
            
        }

        [TestMethod]
        public async Task Handle_ShouldReturnFailure_WhenResetPasswordFails()
        {
            // Arrange
            var command = new ResetUserPasswordCommand
            {
                UserName = "testuser",
                Password = "newPassword123",
                VerificationCode = "ABC123"
            };

            var user = new User
            {
                UserId = 1,
                UserName = "testuser",
                PasswordHash = "oldhash"
            };

            var currentTime = DateTime.UtcNow;

            _mockTimeZoneService!.Setup(tz => tz.GetSystemTimeZone()).Returns("UTC");
            _mockTimeZoneService.Setup(tz => tz.GetCurrentTime("UTC")).Returns(currentTime);

            _mockUserQueryRepository!.Setup(repo => repo.GetByUsernameAsync(command.UserName))
                .ReturnsAsync(user);

            _mockChangePassword!.Setup(cp => cp.PasswordEncode(command.Password))
                .ReturnsAsync("newhash");

            _mockMapper!.Setup(m => m.Map<PasswordLog>(It.IsAny<PasswordLogDTO>()))
                .Returns(new PasswordLog());

            // Simulate reset failure
            _mockChangePassword.Setup(cp => cp.ResetUserPassword(user.UserId, It.IsAny<PasswordLog>()))
                .ReturnsAsync(string.Empty);

            // PasswordLog returns false
            _mockChangePassword.Setup(cp => cp.PasswordLog(It.IsAny<PasswordLog>()))
                .ReturnsAsync(false);

            ForgotPasswordCache.CodeStorage[command.UserName] = new VerificationCodeDetails
            {
                Code = command.VerificationCode,
                ExpiryTime = currentTime.AddMinutes(5)
            };

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Failed to reset the password. Please try again.", result.Message);
            Assert.IsTrue(ForgotPasswordCache.CodeStorage.ContainsKey(command.UserName));

            _mockUserQueryRepository.Verify(r => r.GetByUsernameAsync(command.UserName), Times.Once);
            _mockChangePassword.Verify(cp => cp.PasswordEncode(command.Password), Times.Once);
            _mockChangePassword.Verify(cp => cp.ResetUserPassword(user.UserId, It.IsAny<PasswordLog>()), Times.Once);
            _mockChangePassword.Verify(cp => cp.PasswordLog(It.IsAny<PasswordLog>()), Times.Once);
        }
    }
}
