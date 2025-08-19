using AutoMapper;
using Core.Application.Common.Exceptions;
using Core.Application.Common.Interfaces.ICostCenter;
using Core.Application.CostCenter.Command.UpdateCostCenter;
using Core.Domain.Events;
using FluentAssertions;
using MediatR;
using Moq;

namespace MaintenanceManagement.Tests.UnitTests.Handlers.Commands.CostCenter
{
    [TestClass]
    public class UpdateCostCenterCommandHandlerTests
    {
        private Mock<ICostCenterCommandRepository> _cmdRepo = default!;
        private Mock<ICostCenterQueryRepository> _qryRepo = default!;
        private Mock<IMapper> _mapper = default!;
        private Mock<IMediator> _mediator = default!;
        private UpdateCostCenterCommandHandler _handler = default!;

        [TestInitialize]
        public void Setup()
        {
            _cmdRepo = new Mock<ICostCenterCommandRepository>(MockBehavior.Strict);
            _qryRepo = new Mock<ICostCenterQueryRepository>(MockBehavior.Strict); // not used in handler, but pass for ctor
            _mapper = new Mock<IMapper>(MockBehavior.Strict);
            _mediator = new Mock<IMediator>(MockBehavior.Strict);

            _handler = new UpdateCostCenterCommandHandler(
                _cmdRepo.Object,
                _qryRepo.Object,
                _mapper.Object,
                _mediator.Object);
        }

        [TestMethod]
        public async Task Handle_Should_Update_And_PublishEvent_And_Return_AffectedRows()
        {
            // Arrange
            var cmd = new UpdateCostCenterCommand
            {
                Id = 10,
                CostCenterName = "Updated Name"
            };

            var mapped = new Core.Domain.Entities.CostCenter
            {
                CostCenterName = cmd.CostCenterName
            };

            _mapper
                .Setup(m => m.Map<Core.Domain.Entities.CostCenter>(cmd))
                .Returns(mapped);

            _cmdRepo
                .Setup(r => r.UpdateAsync(cmd.Id, mapped))
                .ReturnsAsync(1);

            _mediator
                .Setup(m => m.Publish(It.IsAny<AuditLogsDomainEvent>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            var result = await _handler.Handle(cmd, CancellationToken.None);

            // Assert
            result.Should().Be(1);

            _mapper.Verify(m => m.Map<Core.Domain.Entities.CostCenter>(cmd), Times.Once);
            _cmdRepo.Verify(r => r.UpdateAsync(cmd.Id, mapped), Times.Once);

            _mediator.Verify(m =>
                m.Publish(It.Is<AuditLogsDomainEvent>(e =>
                    e.ActionDetail == "Update" &&
                    e.ActionName == cmd.CostCenterName &&
                    e.Module == "CostCenter"),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [TestMethod]
        public async Task Handle_Should_Throw_ExceptionRules_When_Update_Returns_Zero()
        {
            // Arrange
            var cmd = new UpdateCostCenterCommand
            {
                Id = 99,
                CostCenterName = "Does Not Exist"
            };

            var mapped = new Core.Domain.Entities.CostCenter
            {
                CostCenterName = cmd.CostCenterName
            };

            _mapper
                .Setup(m => m.Map<Core.Domain.Entities.CostCenter>(cmd))
                .Returns(mapped);

            _cmdRepo
                .Setup(r => r.UpdateAsync(cmd.Id, mapped))
                .ReturnsAsync(0); // simulate not found / no rows affected

            _mediator
                .Setup(m => m.Publish(It.IsAny<AuditLogsDomainEvent>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            var act = async () => await _handler.Handle(cmd, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ExceptionRules>()
                .WithMessage("CostCenter update failed.");

            _mapper.Verify(m => m.Map<Core.Domain.Entities.CostCenter>(cmd), Times.Once);
            _cmdRepo.Verify(r => r.UpdateAsync(cmd.Id, mapped), Times.Once);

            // Your handler publishes the event BEFORE checking result, so it
            // still publishes even when update fails.
            _mediator.Verify(m =>
                m.Publish(It.Is<AuditLogsDomainEvent>(e =>
                    e.ActionDetail == "Update" &&
                    e.ActionName == cmd.CostCenterName &&
                    e.Module == "CostCenter"),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
