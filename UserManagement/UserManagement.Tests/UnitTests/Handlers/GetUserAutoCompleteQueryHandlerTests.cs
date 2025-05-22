using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Interfaces.IUser;
using Core.Application.Users.Queries.GetUserAutoComplete;
using Core.Domain.Entities;
using Core.Domain.Events;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace UserManagement.Tests.UnitTests.Handlers
{
    [TestClass]
    public class GetUserAutoCompleteQueryHandlerTests
    {
        private Mock<IUserQueryRepository> _userQueryRepoMock = null!;
        private Mock<IMapper> _mapperMock = null!;
        private Mock<IMediator> _mediatorMock = null!;
        private ILogger<GetUserAutoCompleteQueryHandler> _logger = null!;
        private GetUserAutoCompleteQueryHandler _handler = null!;
        [TestInitialize]
        public void Setup()
        {
            _userQueryRepoMock = new Mock<IUserQueryRepository>();
            _mapperMock = new Mock<IMapper>();
            _mediatorMock = new Mock<IMediator>();
            _logger = LoggerFactory.Create(builder => builder.AddDebug()).CreateLogger<GetUserAutoCompleteQueryHandler>();

            _handler = new GetUserAutoCompleteQueryHandler(
                _userQueryRepoMock.Object,
                _mapperMock.Object,
                _mediatorMock.Object,
                _logger
            );
        }

        [TestMethod]
        public async Task Handle_ShouldReturnUserList_WhenUsersFound()
        {
            // Arrange
            var searchPattern = "john";
            var query = new GetUserAutoCompleteQuery { SearchPattern = searchPattern };

            var users = new List<User>
            {
                new User { UserId = 1, UserName = "john.doe" },
                new User { UserId = 2, UserName = "johnny" }
            };

            var userDtos = new List<UserAutoCompleteDto>
            {
                new UserAutoCompleteDto { UserId = 1, UserName = "john.doe" },
                new UserAutoCompleteDto { UserId = 2, UserName = "johnny" }
            };

            _userQueryRepoMock.Setup(r => r.GetUser(searchPattern)).ReturnsAsync(users);
            _mapperMock.Setup(m => m.Map<List<UserAutoCompleteDto>>(users)).Returns(userDtos);
            _mediatorMock.Setup(m => m.Publish(It.IsAny<AuditLogsDomainEvent>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEquivalentTo(userDtos);
        }

        [TestMethod]
        public async Task Handle_ShouldReturnEmptyList_WhenNoUsersFound()
        {
            // Arrange
            var query = new GetUserAutoCompleteQuery { SearchPattern = "unknown" };

            _userQueryRepoMock.Setup(r => r.GetUser("unknown")).ReturnsAsync(new List<User>());
            _mapperMock.Setup(m => m.Map<List<UserAutoCompleteDto>>(It.IsAny<List<User>>()))
                       .Returns(new List<UserAutoCompleteDto>());

            _mediatorMock.Setup(m => m.Publish(It.IsAny<AuditLogsDomainEvent>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeEmpty();
        }

    }
}