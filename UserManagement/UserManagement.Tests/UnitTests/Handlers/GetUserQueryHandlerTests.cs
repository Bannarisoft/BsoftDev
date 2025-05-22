using AutoMapper;
using Core.Application.Common.Interfaces.IUser;
using Core.Application.Users.Queries.GetUsers;
using Core.Domain.Entities;
using Core.Domain.Events;
using FluentAssertions;
using MediatR; // ✅ Correct namespace for IMediator
using Microsoft.Extensions.Logging;
using Moq;

namespace UserManagement.Tests.UnitTests.Handlers
{
    [TestClass]
    public class GetUserQueryHandlerTests
    {
        private Mock<IUserQueryRepository> _userQueryRepoMock = null!;
        private Mock<IMapper> _mapperMock = null!;
        private Mock<IMediator> _mediatorMock = null!;
        private ILogger<GetUserQueryHandler> _logger = null!;
        private GetUserQueryHandler _handler = null!;

        [TestInitialize]
        public void Setup()
        {
            _userQueryRepoMock = new Mock<IUserQueryRepository>();
            _mapperMock = new Mock<IMapper>();
            _mediatorMock = new Mock<IMediator>(); // ✅ Now correctly from MediatR
            _logger = LoggerFactory.Create(builder => builder.AddDebug()).CreateLogger<GetUserQueryHandler>();

            _handler = new GetUserQueryHandler(
                _userQueryRepoMock.Object,
                _mapperMock.Object,
                _mediatorMock.Object,
                _logger
            );
        }

        [TestMethod]
        public async Task Handle_ShouldReturnUsers_WhenUsersExist()
        {
            // Arrange
            var query = new GetUserQuery { PageNumber = 1, PageSize = 10, SearchTerm = null };

            var users = new List<User>
            {
                new User { UserId = 1, FirstName = "John" },
                new User { UserId = 2, FirstName = "Jane" }
            };

            var userDtos = new List<UserDto>
            {
                new UserDto { UserId = 1, FirstName = "John" },
                new UserDto { UserId = 2, FirstName = "Jane" }
            };

            _userQueryRepoMock.Setup(r => r.GetAllUsersAsync(1, 10, null))
                .ReturnsAsync((users, 2));

            _mapperMock.Setup(m => m.Map<List<UserDto>>(users))
                .Returns(userDtos);

            _mediatorMock.Setup(m => m.Publish(It.IsAny<AuditLogsDomainEvent>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(userDtos);
            result.TotalCount.Should().Be(2);

            _mediatorMock.Verify(m => m.Publish(It.IsAny<AuditLogsDomainEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public async Task Handle_ShouldReturnFailure_WhenNoUsersFound()
        {
            // Arrange
            var query = new GetUserQuery { PageNumber = 1, PageSize = 10, SearchTerm = null };

            _userQueryRepoMock.Setup(r => r.GetAllUsersAsync(1, 10, null))
                .ReturnsAsync((new List<User>(), 0));

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Be("No users found");
        }
    }
}
