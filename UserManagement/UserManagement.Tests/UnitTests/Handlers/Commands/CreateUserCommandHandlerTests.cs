using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using System.Threading;
using Core.Application.Users.Commands.CreateUser;
using Core.Application.Common.Interfaces.IUser;
using Core.Domain.Entities;
using Core.Application.Users.Queries.GetUsers;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Core.Application.Common.Interfaces;
using MediatR;
using Contracts.Events.Users;
using System;

namespace UserManagement.Tests.UnitTests.Handlers
{
    [TestClass]
    public class CreateUserCommandHandlerTests
    {
        private Mock<IUserCommandRepository>? _mockUserRepository;
        private Mock<IMapper>? _mockMapper;
        private Mock<IMediator>? _mockMediator;
        private Mock<ILogger<CreateUserCommandHandler>>? _mockLogger;
        private Mock<IEventPublisher>? _mockEventPublisher;

        private CreateUserCommandHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockUserRepository = new Mock<IUserCommandRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockMediator = new Mock<IMediator>();
            _mockLogger = new Mock<ILogger<CreateUserCommandHandler>>();
            _mockEventPublisher = new Mock<IEventPublisher>();

            _handler = new CreateUserCommandHandler(
                _mockUserRepository.Object,
                _mockMapper.Object,
                _mockMediator.Object,
                _mockLogger.Object,
                _mockEventPublisher.Object
            );
        }

        [TestMethod]
        public async Task Handle_ShouldReturnSuccess_WhenUserCreated()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                UserName = "testuser",
                EmailId = "test@example.com",
                FirstName = "Test",
                LastName = "User"
            };

            var userEntity = new User {UserName = command.UserName };
            var createdUser = new User
            {
                UserId = userEntity.UserId,
                UserName = command.UserName,
                EmailId = command.EmailId,
                FirstName = command.FirstName,
                LastName = command.LastName
            };

            var userDto = new UserDto
            {
                UserId = createdUser.UserId,
                UserName = createdUser.UserName,
                EmailId = createdUser.EmailId
            };

            _mockMapper!.Setup(m => m.Map<User>(command)).Returns(userEntity);
            _mockUserRepository!.Setup(r => r.CreateAsync(It.IsAny<User>())).ReturnsAsync(createdUser);
            _mockMapper.Setup(m => m.Map<UserDto>(createdUser)).Returns(userDto);

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.UserName.Should().Be("testuser");

            _mockEventPublisher!.Verify(p => p.SaveEventAsync(It.IsAny<UserCreatedEvent>()), Times.Once);
            _mockEventPublisher.Verify(p => p.PublishPendingEventsAsync(), Times.Once);
            _mockMediator!.Verify(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task Handle_ShouldReturnFailure_WhenRepositoryReturnsNull()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                UserName = "faileduser"
            };

            var userEntity = new User { UserName = command.UserName };

            _mockMapper!.Setup(m => m.Map<User>(command)).Returns(userEntity);
            _mockUserRepository!.Setup(r => r.CreateAsync(It.IsAny<User>())).ReturnsAsync((User?)null!);

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("Failed to create user. Please try again.");

            _mockEventPublisher!.Verify(p => p.SaveEventAsync(It.IsAny<UserCreationFailedEvent>()), Times.Once);
            _mockEventPublisher.Verify(p => p.PublishPendingEventsAsync(), Times.Once);
        }
    }
}
