using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;
using FluentValidation;
using MediatR;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Core.Application.Location.Command.UpdateLocation;   // handler + command
using Core.Application.Common.Interfaces.ILocation;       // repos
using Core.Domain.Events;                                 // AuditLogsDomainEvent

namespace FixedAssetManagement.Tests.UnitTests.Locations
{
    [TestClass]
    public class UpdateLocationCommandHandlerTests
    {
        private static UpdateLocationCommand MakeCommand(
            int id = 123, string name = "Main Store", int sortOrder = 5, string code = "LOC-123")
            => new UpdateLocationCommand
            {
                Id = id,
                LocationName = name,
                SortOrder = sortOrder,   // now non-nullable -> matches command property
                Code = code,
                DepartmentId = 1,
                UnitId = 1
            };

        [TestMethod]
        public async Task Handle_WhenNameDuplicate_ThrowsValidationException()
        {
            var cmdRepo = new Mock<ILocationCommandRepository>(MockBehavior.Strict);
            var qryRepo = new Mock<ILocationQueryRepository>(MockBehavior.Strict);
            var mediator = new Mock<IMediator>(MockBehavior.Strict);
            var mapper = new Mock<IMapper>(MockBehavior.Strict);

            var request = MakeCommand();

            cmdRepo.Setup(r => r.CheckForDuplicatesAsync(request.LocationName, request.SortOrder, request.Id))
                   .ReturnsAsync((isNameDuplicate: true, isSortOrderDuplicate: false));

            var sut = new UpdateLocationCommandHandler(cmdRepo.Object, qryRepo.Object, mediator.Object, mapper.Object);

            var ex = await Assert.ThrowsExceptionAsync<ValidationException>(() =>
                sut.Handle(request, CancellationToken.None));

            StringAssert.Contains(ex.Message, "Location with the same LocationName already exists.");

            cmdRepo.Verify(r => r.CheckForDuplicatesAsync(request.LocationName, request.SortOrder, request.Id), Times.Once);
            cmdRepo.VerifyNoOtherCalls();
            qryRepo.VerifyNoOtherCalls();
            mapper.VerifyNoOtherCalls();
            mediator.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task Handle_WhenSortOrderDuplicate_ThrowsValidationException()
        {
            var cmdRepo = new Mock<ILocationCommandRepository>(MockBehavior.Strict);
            var qryRepo = new Mock<ILocationQueryRepository>(MockBehavior.Strict);
            var mediator = new Mock<IMediator>(MockBehavior.Strict);
            var mapper = new Mock<IMapper>(MockBehavior.Strict);

            var request = MakeCommand();

            cmdRepo.Setup(r => r.CheckForDuplicatesAsync(request.LocationName, request.SortOrder, request.Id))
                   .ReturnsAsync((isNameDuplicate: false, isSortOrderDuplicate: true));

            var sut = new UpdateLocationCommandHandler(cmdRepo.Object, qryRepo.Object, mediator.Object, mapper.Object);

            var ex = await Assert.ThrowsExceptionAsync<ValidationException>(() =>
                sut.Handle(request, CancellationToken.None));

            StringAssert.Contains(ex.Message, "Location with the same Sort Order already exists.");

            cmdRepo.Verify(r => r.CheckForDuplicatesAsync(request.LocationName, request.SortOrder, request.Id), Times.Once);
            cmdRepo.VerifyNoOtherCalls();
            qryRepo.VerifyNoOtherCalls();
            mapper.VerifyNoOtherCalls();
            mediator.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task Handle_WhenBothDuplicates_ThrowsValidationException()
        {
            var cmdRepo = new Mock<ILocationCommandRepository>(MockBehavior.Strict);
            var qryRepo = new Mock<ILocationQueryRepository>(MockBehavior.Strict);
            var mediator = new Mock<IMediator>(MockBehavior.Strict);
            var mapper = new Mock<IMapper>(MockBehavior.Strict);

            var request = MakeCommand();

            cmdRepo.Setup(r => r.CheckForDuplicatesAsync(request.LocationName, request.SortOrder, request.Id))
                   .ReturnsAsync((isNameDuplicate: true, isSortOrderDuplicate: true));

            var sut = new UpdateLocationCommandHandler(cmdRepo.Object, qryRepo.Object, mediator.Object, mapper.Object);

            var ex = await Assert.ThrowsExceptionAsync<ValidationException>(() =>
                sut.Handle(request, CancellationToken.None));

            StringAssert.Contains(ex.Message, "Both Location Name and Sort Order already exist.");

            cmdRepo.Verify(r => r.CheckForDuplicatesAsync(request.LocationName, request.SortOrder, request.Id), Times.Once);
            cmdRepo.VerifyNoOtherCalls();
            qryRepo.VerifyNoOtherCalls();
            mapper.VerifyNoOtherCalls();
            mediator.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task Handle_WhenUpdateSucceeds_Maps_Updates_Publishes_ReturnsTrue()
        {
            var cmdRepo = new Mock<ILocationCommandRepository>(MockBehavior.Strict);
            var qryRepo = new Mock<ILocationQueryRepository>(MockBehavior.Strict);
            var mediator = new Mock<IMediator>(MockBehavior.Strict);
            var mapper = new Mock<IMapper>(MockBehavior.Strict);

            var request = MakeCommand();

            cmdRepo.Setup(r => r.CheckForDuplicatesAsync(request.LocationName, request.SortOrder, request.Id))
                   .ReturnsAsync((false, false));

            var mappedEntity = new Core.Domain.Entities.Location
            {
                Id = request.Id,
                Code = request.Code,
                LocationName = request.LocationName,
                SortOrder = request.SortOrder
            };

            mapper.Setup(m => m.Map<Core.Domain.Entities.Location>(request))
                  .Returns(mappedEntity);

            cmdRepo.Setup(r => r.UpdateAsync(mappedEntity)).ReturnsAsync(true);

            mediator.Setup(m => m.Publish(It.IsAny<AuditLogsDomainEvent>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            var sut = new UpdateLocationCommandHandler(cmdRepo.Object, qryRepo.Object, mediator.Object, mapper.Object);

            var result = await sut.Handle(request, CancellationToken.None);

            Assert.IsTrue(result);

            cmdRepo.Verify(r => r.CheckForDuplicatesAsync(request.LocationName, request.SortOrder, request.Id), Times.Once);
            mapper.Verify(m => m.Map<Core.Domain.Entities.Location>(request), Times.Once);
            cmdRepo.Verify(r => r.UpdateAsync(mappedEntity), Times.Once);
            mediator.Verify(m => m.Publish(It.IsAny<AuditLogsDomainEvent>(), It.IsAny<CancellationToken>()), Times.Once);

            cmdRepo.VerifyNoOtherCalls();
            qryRepo.VerifyNoOtherCalls();
            mapper.VerifyNoOtherCalls();
            mediator.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task Handle_WhenUpdateReturnsFalse_Publishes_ThenThrows()
        {
            var cmdRepo = new Mock<ILocationCommandRepository>(MockBehavior.Strict);
            var qryRepo = new Mock<ILocationQueryRepository>(MockBehavior.Strict);
            var mediator = new Mock<IMediator>(MockBehavior.Strict);
            var mapper = new Mock<IMapper>(MockBehavior.Strict);

            var request = MakeCommand();

            cmdRepo.Setup(r => r.CheckForDuplicatesAsync(request.LocationName, request.SortOrder, request.Id))
                   .ReturnsAsync((false, false));

            var mappedEntity = new Core.Domain.Entities.Location
            {
                Id = request.Id,
                Code = request.Code,
                LocationName = request.LocationName,
                SortOrder = request.SortOrder
            };

            mapper.Setup(m => m.Map<Core.Domain.Entities.Location>(request))
                  .Returns(mappedEntity);

            cmdRepo.Setup(r => r.UpdateAsync(mappedEntity)).ReturnsAsync(false);

            // Handler publishes regardless; keep expectation.
            mediator.Setup(m => m.Publish(It.IsAny<AuditLogsDomainEvent>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            var sut = new UpdateLocationCommandHandler(cmdRepo.Object, qryRepo.Object, mediator.Object, mapper.Object);

            var ex = await Assert.ThrowsExceptionAsync<Exception>(() =>
                sut.Handle(request, CancellationToken.None));

            StringAssert.Contains(ex.Message.ToLowerInvariant(), "not updated");

            cmdRepo.Verify(r => r.CheckForDuplicatesAsync(request.LocationName, request.SortOrder, request.Id), Times.Once);
            mapper.Verify(m => m.Map<Core.Domain.Entities.Location>(request), Times.Once);
            cmdRepo.Verify(r => r.UpdateAsync(mappedEntity), Times.Once);
            mediator.Verify(m => m.Publish(It.IsAny<AuditLogsDomainEvent>(), It.IsAny<CancellationToken>()), Times.Once);

            cmdRepo.VerifyNoOtherCalls();
            qryRepo.VerifyNoOtherCalls();
            mapper.VerifyNoOtherCalls();
            mediator.VerifyNoOtherCalls();
        }
    }
}
