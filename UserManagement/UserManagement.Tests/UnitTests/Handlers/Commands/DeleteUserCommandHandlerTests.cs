using AutoMapper;
using Core.Application.Common.Interfaces.IUser;
using Core.Application.Users.Commands.DeleteUser;
using Core.Domain.Entities;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
namespace UserManagement.Tests.UnitTests.Handlers
{

    [TestClass]
    public class DeleteUserCommandHandlerTests
    {
        private Mock<IUserCommandRepository>? _mockUserRepo;
        private Mock<IMapper>? _mockMapper;
        private Mock<IMediator>? _mockMediator;
        private Mock<ILogger<DeleteUserCommandHandler>>? _mockLogger;
        private DeleteUserCommandHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockUserRepo = new Mock<IUserCommandRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockMediator = new Mock<IMediator>();
            _mockLogger = new Mock<ILogger<DeleteUserCommandHandler>>();
            _handler = new DeleteUserCommandHandler(
                _mockUserRepo.Object,
                _mockMapper.Object,
                _mockMediator.Object,
                _mockLogger.Object
            );
        }

        [TestMethod]
        public async Task Handle_ShouldReturnSuccess_WhenUserIsDeleted()
        {
            // Arrange
            var userId = 123;
            var request = new DeleteUserCommand { UserId = userId };
            var mappedUser = new User
            {
                UserId = userId,
                UserName = "testuser",
                FirstName = "Test",
                LastName = "User"
            };

            _mockMapper!.Setup(m => m.Map<User>(request)).Returns(mappedUser);
            _mockUserRepo!
                .Setup(r => r.DeleteAsync(userId, mappedUser))
                .ReturnsAsync(true);

            // Act
            var result = await _handler!.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeTrue();
            result.Message.Should().Be("User deleted successfully.");
            _mockMediator!.Verify(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockUserRepo.Verify(r => r.DeleteAsync(userId, mappedUser), Times.Once);
        }

        [TestMethod]
        public async Task Handle_ShouldReturnFailure_WhenUserIsNotDeleted()
        {
            // Arrange
            var userId = 456;
            var request = new DeleteUserCommand { UserId = userId };
            var mappedUser = new User
            {
                UserId = userId,
                UserName = "testuser",
                FirstName = "Test",
                LastName = "User"
            };

            _mockMapper!.Setup(m => m.Map<User>(request)).Returns(mappedUser);
            _mockUserRepo!
                .Setup(r => r.DeleteAsync(userId, mappedUser))
                .ReturnsAsync(false);

            // Act
            var result = await _handler!.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeFalse();
            result.Message.Should().Be("User could not be deleted.");
            _mockMediator!.Verify(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockUserRepo.Verify(r => r.DeleteAsync(userId, mappedUser), Times.Once);
        }
    }
}