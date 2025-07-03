using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using Core.Application.Users.Commands.UpdateUser;
using Core.Domain.Entities;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IUser;

namespace UserManagement.Tests.UnitTests.Handlers
{
    [TestClass]
    public class UpdateUserCommandHandlerTests
    {
        private Mock<IUserCommandRepository>? _mockUserCommandRepo;
        private Mock<IUserQueryRepository>? _mockUserQueryRepo;
        private Mock<IMapper>? _mockMapper;
        private Mock<IMediator>? _mockMediator;
        private Mock<ILogger<UpdateUserCommandHandler>>? _mockLogger;

        private UpdateUserCommandHandler? _handler;

        [TestInitialize]
        public void Setup()
        {
            _mockUserCommandRepo = new Mock<IUserCommandRepository>();
            _mockUserQueryRepo = new Mock<IUserQueryRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockMediator = new Mock<IMediator>();
            _mockLogger = new Mock<ILogger<UpdateUserCommandHandler>>();

            _handler = new UpdateUserCommandHandler(
                _mockUserCommandRepo.Object,
                _mockUserQueryRepo.Object,
                _mockMapper.Object,
                _mockMediator.Object,
                _mockLogger.Object
            );
        }

        [TestMethod]
        public async Task Handle_ShouldReturnSuccess_WhenUserIsUpdated()
        {
            // Arrange
            var userId = 1; // Use int for UserId
            var command = new UpdateUserCommand
            {
                UserId = userId,
                UserName = "UpdatedUser"
            };

            var existingUser = new User
            {
                UserId = userId,
                UserName = "OldUser",
                FirstName = "First",
                LastName = "Last"
            };

            _mockUserQueryRepo!.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(existingUser);
            _mockMapper!.Setup(m => m.Map(command, existingUser)); // simulate mapping
            _mockUserCommandRepo!.Setup(r => r.UpdateAsync(userId, existingUser)).ReturnsAsync(1);

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeTrue();
            result.Message.Should().Be("User updated successfully.");

            _mockUserCommandRepo!.Verify(r => r.UpdateAsync(userId, existingUser), Times.Once);
            _mockMediator?.Verify(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task Handle_ShouldReturnFailure_WhenUpdateFails()
        {
            // Arrange
            var userId = 1; // Use int for UserId
            var command = new UpdateUserCommand
            {
                UserId = userId,
                UserName = "UpdatedUser"
            };

            var existingUser = new User
            {
                UserId = userId,
                UserName = "OldUser",
                FirstName = "First",
                LastName = "Last"
            };

            _mockUserQueryRepo!.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(existingUser);
            _mockMapper!.Setup(m => m.Map(command, existingUser));
            _mockUserCommandRepo!.Setup(r => r.UpdateAsync(userId, existingUser)).ReturnsAsync(0);

            // Act
            var result = await _handler!.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Data.Should().BeFalse();
            result.Message.Should().Be("User update failed.");

            _mockMediator!.Verify(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
