using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Interfaces.IUser;
using Core.Application.Users.Queries.GetUserById;
using Core.Application.Users.Queries.GetUsers;
using Core.Domain.Entities;
using Core.Domain.Events;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace UserManagement.Tests.UnitTests.Handlers
{
    [TestClass]
    public class GetUserByIdQueryHandlerTests
    {
        private Mock<IUserQueryRepository> _userQueryRepoMock = null!;
        private Mock<IMapper> _mapperMock = null!;
        private Mock<IMediator> _mediatorMock = null!;
        private ILogger<GetUserByIdQueryHandler> _logger = null!;
        private GetUserByIdQueryHandler _handler = null!;

        [TestInitialize]
        public void Setup()
        {
            _userQueryRepoMock = new Mock<IUserQueryRepository>();
            _mapperMock = new Mock<IMapper>();
            _mediatorMock = new Mock<IMediator>();
            _logger = LoggerFactory.Create(builder => builder.AddDebug()).CreateLogger<GetUserByIdQueryHandler>();

            _handler = new GetUserByIdQueryHandler(
                _userQueryRepoMock.Object,
                _mapperMock.Object,
                _mediatorMock.Object,
                _logger
            );
        }

        [TestMethod]
        public async Task Handle_ShouldReturnUserByIdDTO_WhenUserExists()
        {
            // Arrange
            int userId = 1;
            var user = new User
            {
                UserId = userId,
                UserName = "john.doe",
                FirstName = "John",
                LastName = "Doe"
            };

            var expectedDto = new UserByIdDTO
            {
                UserId = userId,
                UserName = "john.doe",
                FirstName = "John Doe"
            };

            _userQueryRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
            _mapperMock.Setup(m => m.Map<UserByIdDTO>(user)).Returns(expectedDto);
            _mediatorMock.Setup(m => m.Publish(It.IsAny<AuditLogsDomainEvent>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var query = new GetUserByIdQuery { UserId = userId };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedDto);
            _mediatorMock.Verify(m => m.Publish(It.IsAny<AuditLogsDomainEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task Handle_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            int userId = 99;
            _userQueryRepoMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User)null!);

            var query = new GetUserByIdQuery { UserId = userId };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}