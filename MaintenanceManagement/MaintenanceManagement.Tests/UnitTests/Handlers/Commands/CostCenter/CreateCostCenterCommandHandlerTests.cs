using AutoMapper;
using Core.Application.Common.Exceptions;
using Core.Application.Common.Interfaces.ICostCenter;
using Core.Application.CostCenter.Command.CreateCostCenter;
using Core.Domain.Events;
using FluentAssertions;
using MediatR;
using Moq;

namespace MaintenanceManagement.Tests.UnitTests.Handlers.Commands.CostCenter
{
    [TestClass]
    public class CreateCostCenterCommandHandlerTests
    {
        private Mock<ICostCenterCommandRepository> _repo = default!;
        private Mock<IMediator> _mediator = default!;
        private Mock<IMapper> _mapper = default!;
        private CreateCostCenterCommandHandler _handler = default!;

        [TestInitialize]
        public void Setup()
        {
            _repo = new Mock<ICostCenterCommandRepository>(MockBehavior.Strict);
            _mediator = new Mock<IMediator>(MockBehavior.Strict);
            _mapper = new Mock<IMapper>(MockBehavior.Strict);

            _handler = new CreateCostCenterCommandHandler(
                _repo.Object,
                _mediator.Object,
                _mapper.Object);
        }

        [TestMethod]
        public async Task Handle_Should_Create_And_PublishEvent_And_Return_NewId()
        {
            // Arrange
            var cmd = new CreateCostCenterCommand
            {
                // set the fields your mapper/domain need
                CostCenterCode = "CC-100",
                CostCenterName = "Spinning"
            };

            var mappedEntity = new Core.Domain.Entities.CostCenter
            {
                CostCenterCode = cmd.CostCenterCode,
                CostCenterName = cmd.CostCenterName
            };

            _mapper
                .Setup(m => m.Map<Core.Domain.Entities.CostCenter>(cmd))
                .Returns(mappedEntity);

            _repo
                .Setup(r => r.CreateAsync(mappedEntity))
                .ReturnsAsync(123); // simulate DB-generated Id

            _mediator
                .Setup(m => m.Publish(It.IsAny<AuditLogsDomainEvent>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            var result = await _handler.Handle(cmd, CancellationToken.None);

            // Assert
            result.Should().Be(123);

            _repo.Verify(r => r.CreateAsync(mappedEntity), Times.Once);
            _mapper.Verify(m => m.Map<Core.Domain.Entities.CostCenter>(cmd), Times.Once);

            // We verify an AuditLogsDomainEvent was published once
            _mediator.Verify(m =>
                m.Publish(It.IsAny<AuditLogsDomainEvent>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [TestMethod]
        public async Task Handle_Should_Throw_ExceptionRules_When_Insert_Fails()
        {
            // Arrange
            var cmd = new CreateCostCenterCommand
            {
                CostCenterCode = "CC-200",
                CostCenterName = "Weaving"
            };

            var mappedEntity = new Core.Domain.Entities.CostCenter
            {
                CostCenterCode = cmd.CostCenterCode,
                CostCenterName = cmd.CostCenterName
            };

            _mapper
                .Setup(m => m.Map<Core.Domain.Entities.CostCenter>(cmd))
                .Returns(mappedEntity);

            // simulate failure from repository
            _repo
                .Setup(r => r.CreateAsync(mappedEntity))
                .ReturnsAsync(0);

            // Even in failure, your handler constructs & publishes the domain event
            // (If you DON'T want that, move Publish after the success check.)
            _mediator
                .Setup(m => m.Publish(It.IsAny<AuditLogsDomainEvent>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            var act = async () => await _handler.Handle(cmd, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ExceptionRules>()
                .WithMessage("CostCenter Creation Failed.");

            _repo.Verify(r => r.CreateAsync(mappedEntity), Times.Once);
            _mapper.Verify(m => m.Map<Core.Domain.Entities.CostCenter>(cmd), Times.Once);
            _mediator.Verify(m =>
                m.Publish(It.IsAny<AuditLogsDomainEvent>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
