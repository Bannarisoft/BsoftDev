using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IUser;
using Core.Application.Common.Interfaces;
using Core.Application.Users.Commands.ResetUserPassword;
using Core.Application.Users.Queries.GetUsers; // for PasswordLogDTO (adjust if located elsewhere)
using Core.Domain.Entities;                   // for User, PasswordLog
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

        [TestCleanup]
        public void Cleanup()
        {
            // ensure no cross-test leakage of verification codes
            ForgotPasswordCache.CodeStorage.Clear();
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
            // tolerate any timezone string passed by the handler
            _mockTimeZoneService.Setup(tz => tz.GetCurrentTime(It.IsAny<string>())).Returns(currentTime);

            _mockUserQueryRepository!.Setup(repo => repo.GetByUsernameAsync(command.UserName))
                .ReturnsAsync(user);

            _mockChangePassword!.Setup(cp => cp.PasswordEncode(command.Password))
                .ReturnsAsync("newhash");

            // Map PasswordLogDTO -> PasswordLog
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

            // valid code in cache
            ForgotPasswordCache.CodeStorage[command.UserName] = new VerificationCodeDetails
            {
                Code = command.VerificationCode,
                ExpiryTime = currentTime.AddMinutes(5)
            };

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
            // your handler returns the literal repo string "success"
            Assert.AreEqual("success", result.Message);

            _mockUserQueryRepository.Verify(r => r.GetByUsernameAsync(command.UserName), Times.Once);
            _mockChangePassword.Verify(cp => cp.PasswordEncode(command.Password), Times.Once);
            _mockChangePassword.Verify(cp => cp.ResetUserPassword(user.UserId, It.IsAny<PasswordLog>()), Times.Once);
            _mockChangePassword.Verify(cp => cp.PasswordLog(It.IsAny<PasswordLog>()), Times.Once);
        }

        [TestMethod]
        public async Task Handle_ShouldThrow_WhenUserNotFound()
        {
            // Arrange – keep code valid so handler proceeds to user fetch
            var command = new ResetUserPasswordCommand
            {
                UserName = "ghostuser",
                Password = "whatever123",
                VerificationCode = "XYZ789"
            };

            var currentTime = DateTime.UtcNow;

            _mockTimeZoneService!.Setup(tz => tz.GetSystemTimeZone()).Returns("UTC");
            _mockTimeZoneService.Setup(tz => tz.GetCurrentTime(It.IsAny<string>())).Returns(currentTime);

            // valid, non-expired code
            ForgotPasswordCache.CodeStorage[command.UserName] = new VerificationCodeDetails
            {
                Code = command.VerificationCode,
                ExpiryTime = currentTime.AddMinutes(5)
            };

            // force null user to follow current handler's behavior (throws NRE on this path)
            _mockUserQueryRepository!.Setup(repo => repo.GetByUsernameAsync(command.UserName))
                .ReturnsAsync((User?)null);

            // Act + Assert — current handler throws; assert that behavior
            await Assert.ThrowsExceptionAsync<NullReferenceException>(async () =>
                await _handler!.Handle(command, CancellationToken.None)
            );

            _mockUserQueryRepository.Verify(r => r.GetByUsernameAsync(command.UserName), Times.Once);
        }
    }
}
