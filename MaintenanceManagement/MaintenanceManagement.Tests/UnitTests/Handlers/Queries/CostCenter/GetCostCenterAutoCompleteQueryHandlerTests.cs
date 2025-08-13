using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Interfaces.ICostCenter;
using Core.Application.CostCenter.Queries.GetCostCenter;
using Core.Application.CostCenter.Queries.GetCostCenterAutoComplete;
using Core.Domain.Events;
using FluentAssertions;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MaintenanceManagement.Tests.UnitTests.Handlers.Queries.CostCenter
{
    [TestClass]
    public class GetCostCenterAutoCompleteQueryHandlerTests
    {
        private Mock<ICostCenterQueryRepository> _repo = default!;
        private Mock<IMapper> _mapper = default!;
        private Mock<IMediator> _mediator = default!;
        private GetCostCenterAutoCompleteQueryHandler _handler = default!;

        [TestInitialize]
        public void Setup()
        {
            _repo = new Mock<ICostCenterQueryRepository>(MockBehavior.Strict);
            _mapper = new Mock<IMapper>(MockBehavior.Strict);
            _mediator = new Mock<IMediator>(MockBehavior.Strict);

            _handler = new GetCostCenterAutoCompleteQueryHandler(
                _repo.Object, _mapper.Object, _mediator.Object);
        }

        [TestMethod]
        public async Task Handle_Should_Map_Results_And_Publish_Event()
        {
            // Arrange
            var query = new GetCostCenterAutoCompleteQuery { SearchPattern = "spin" };

            // Simulate repository returning domain entities
            var domainList = new List<Core.Domain.Entities.CostCenter>
            {
                new() { Id = 1, CostCenterCode = "CC-001", CostCenterName = "Spinning" },
                new() { Id = 2, CostCenterCode = "CC-002", CostCenterName = "Speed Frame" }
            };

            var dtoList = new List<CostCenterAutoCompleteDto>
            {
                new() { Id = 1, CostCenterName = "Spinning" },
                new() { Id = 2, CostCenterName = "Speed Frame" }
            };

            _repo
                .Setup(r => r.GetCostCenterGroups(query.SearchPattern))
                .ReturnsAsync(domainList);

            _mapper
                .Setup(m => m.Map<List<CostCenterAutoCompleteDto>>(domainList))
                .Returns(dtoList);

            _mediator
                .Setup(m => m.Publish(It.IsAny<AuditLogsDomainEvent>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result[0].Id.Should().Be(1);
            result[0].CostCenterName.Should().Be("Spinning");

            _repo.Verify(r => r.GetCostCenterGroups("spin"), Times.Once);
            _mapper.Verify(m => m.Map<List<CostCenterAutoCompleteDto>>(domainList), Times.Once);

            _mediator.Verify(m =>
                m.Publish(It.Is<AuditLogsDomainEvent>(e =>
                        e.ActionDetail == "GetAll" &&
                        e.ActionCode   == "GetCostCenterAutoCompleteQueryHandler" &&
                        e.ActionName   == "2" &&            // count as string
                        e.Module       == "CostCenter"),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [TestMethod]
        public async Task Handle_Should_Return_Empty_List_And_Publish_Event_When_No_Results()
        {
            // Arrange
            var query = new GetCostCenterAutoCompleteQuery { SearchPattern = "zzz" };
            var emptyDomain = new List<Core.Domain.Entities.CostCenter>();
            var emptyDto    = new List<CostCenterAutoCompleteDto>();

            _repo
                .Setup(r => r.GetCostCenterGroups(query.SearchPattern))
                .ReturnsAsync(emptyDomain);

            _mapper
                .Setup(m => m.Map<List<CostCenterAutoCompleteDto>>(emptyDomain))
                .Returns(emptyDto);

            _mediator
                .Setup(m => m.Publish(It.IsAny<AuditLogsDomainEvent>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();

            _repo.Verify(r => r.GetCostCenterGroups("zzz"), Times.Once);
            _mapper.Verify(m => m.Map<List<CostCenterAutoCompleteDto>>(emptyDomain), Times.Once);

            _mediator.Verify(m =>
                m.Publish(It.Is<AuditLogsDomainEvent>(e =>
                        e.ActionDetail == "GetAll" &&
                        e.ActionCode   == "GetCostCenterAutoCompleteQueryHandler" &&
                        e.ActionName   == "0" &&            // count as string
                        e.Module       == "CostCenter"),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
