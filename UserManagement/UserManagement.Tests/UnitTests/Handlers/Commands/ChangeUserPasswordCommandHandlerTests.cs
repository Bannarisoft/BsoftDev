using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IUser;
using Core.Application.Users.Commands.ChangeUserPassword;
using Core.Domain.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Core.Application.Tests.Users.Commands
{
    [TestClass]
    public class ChangeUserPasswordCommandHandlerTests
    {
        private Mock<IMapper>? _mockMapper;
        private Mock<IUserQueryRepository>? _mockUserQueryRepository;
        private Mock<IChangePassword>? _mockChangePassword;
        private ChangeUserPasswordCommandHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockMapper = new Mock<IMapper>();
            _mockUserQueryRepository = new Mock<IUserQueryRepository>();
            _mockChangePassword = new Mock<IChangePassword>();

            // Ensure all dependencies are properly initialized
            _handler = new ChangeUserPasswordCommandHandler(
                _mockMapper.Object,
                _mockUserQueryRepository.Object,
                _mockChangePassword.Object);
        }

        [TestMethod]
        public async Task Handle_ShouldReturnSuccess_WhenPasswordChanged()
        {
            // Arrange
            var command = new ChangeUserPasswordCommand
            {
                UserId = 1,
                NewPassword = "newPassword123"
            };

            var passwordLog = new PasswordLog();

            _mockMapper!
                .Setup(m => m.Map<PasswordLog>(command))
                .Returns(passwordLog);

            _mockChangePassword!
                .Setup(c => c.PasswordEncode(command.NewPassword))
                .ReturnsAsync("encodedPassword");

            _mockChangePassword
                .Setup(c => c.ChangePassword(command.UserId, command.NewPassword, passwordLog))
                .ReturnsAsync(true);

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("Password changed successfully.", result.Message);
        }

        [TestMethod]
        public async Task Handle_ShouldReturnFailure_WhenPasswordChangeFails()
        {
            // Arrange
            var command = new ChangeUserPasswordCommand
            {
                UserId = 1,
                NewPassword = "newPassword123"
            };

            var passwordLog = new PasswordLog();

            _mockMapper!
                .Setup(m => m.Map<PasswordLog>(command))
                .Returns(passwordLog);

            _mockChangePassword!
                .Setup(c => c.PasswordEncode(command.NewPassword))
                .ReturnsAsync("encodedPassword");

            _mockChangePassword
                .Setup(c => c.ChangePassword(command.UserId, command.NewPassword, passwordLog))
                .ReturnsAsync(false);

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Try a different Password", result.Message);
        }

        [TestMethod]
        public async Task Handle_ShouldReturnFailure_WhenNewPasswordIsNullOrEmpty()
        {
            // Arrange
            var command = new ChangeUserPasswordCommand
            {
                UserId = 1,
                NewPassword = ""
            };

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual("Invalid input parameters.", result.Message);
        }
    }
}
