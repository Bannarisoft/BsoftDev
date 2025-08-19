using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AutoMapper;
using FluentValidation;
using MediatR;

using Core.Application.Location.Command.DeleteLocation;                 // handler + command
using Core.Application.Location.Queries.GetLocations;                   // LocationDto
using Core.Application.Common.Interfaces.ILocation;                     // ILocation* repos
using Core.Application.Common.Interfaces.IAssetMaster.IAssetLocation;   // IAssetLocationQueryRepository
using Core.Domain.Events;                                               // AuditLogsDomainEvent
using Core.Domain.Common;                                               // BaseEntity.IsDelete
using Core.Domain.Entities;                                             // Location
using Core.Domain.Entities.AssetMaster;                                 // AssetLocation (DOMAIN TYPE)

namespace FixedAssetManagement.Tests.UnitTests.Locations
{
    [TestClass]
    public class DeleteLocationCommandHandlerTests
    {
        // For "active" (i.e., not deleted) we simply don't set IsDeleted (defaults to 0/not-deleted)
        private static Location ActiveEntity(int id, string code = "LOC-001", string name = "Main Store") =>
            new Location { Id = id, Code = code, LocationName = name /* IsDeleted default (0) */ };

        private static Location DeletedEntity(int id) =>
            new Location { Id = id, IsDeleted = BaseEntity.IsDelete.Deleted };

        [TestMethod]
        public async Task Handle_WhenNotFound_Or_Deleted_ThrowsValidationException()
        {
            var cmdRepo   = new Mock<ILocationCommandRepository>(MockBehavior.Strict);
            var qryRepo   = new Mock<ILocationQueryRepository>(MockBehavior.Strict);
            var mediator  = new Mock<IMediator>(MockBehavior.Strict);
            var mapper    = new Mock<IMapper>(MockBehavior.Strict);
            var assetRepo = new Mock<IAssetLocationQueryRepository>(MockBehavior.Strict);

            var sut = new DeleteLocationCommandHandler(cmdRepo.Object, mediator.Object, mapper.Object, qryRepo.Object, assetRepo.Object);

            // Case 1: Not found
            qryRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Location?)null);

            var ex1 = await Assert.ThrowsExceptionAsync<ValidationException>(() =>
                sut.Handle(new DeleteLocationCommand { Id = 999 }, CancellationToken.None));

            StringAssert.Contains(ex1.Message, "Invalid LocationID", StringComparison.OrdinalIgnoreCase);

            qryRepo.Verify(r => r.GetByIdAsync(999), Times.Once);
            qryRepo.VerifyNoOtherCalls();
            cmdRepo.VerifyNoOtherCalls();
            mapper.VerifyNoOtherCalls();
            mediator.VerifyNoOtherCalls();
            assetRepo.VerifyNoOtherCalls();

            // Case 2: Found but IsDeleted
            qryRepo   = new Mock<ILocationQueryRepository>(MockBehavior.Strict);
            cmdRepo   = new Mock<ILocationCommandRepository>(MockBehavior.Strict);
            mapper    = new Mock<IMapper>(MockBehavior.Strict);
            mediator  = new Mock<IMediator>(MockBehavior.Strict);
            assetRepo = new Mock<IAssetLocationQueryRepository>(MockBehavior.Strict);

            sut = new DeleteLocationCommandHandler(cmdRepo.Object, mediator.Object, mapper.Object, qryRepo.Object, assetRepo.Object);

            qryRepo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(DeletedEntity(10));

            var ex2 = await Assert.ThrowsExceptionAsync<ValidationException>(() =>
                sut.Handle(new DeleteLocationCommand { Id = 10 }, CancellationToken.None));

            StringAssert.Contains(ex2.Message, "Invalid LocationID", StringComparison.OrdinalIgnoreCase);

            qryRepo.Verify(r => r.GetByIdAsync(10), Times.Once);
            qryRepo.VerifyNoOtherCalls();
            cmdRepo.VerifyNoOtherCalls();
            mapper.VerifyNoOtherCalls();
            mediator.VerifyNoOtherCalls();
            assetRepo.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task Handle_WhenReferencedByAssetLocation_ThrowsValidationException()
        {
            var cmdRepo   = new Mock<ILocationCommandRepository>(MockBehavior.Strict);
            var qryRepo   = new Mock<ILocationQueryRepository>(MockBehavior.Strict);
            var mediator  = new Mock<IMediator>(MockBehavior.Strict);
            var mapper    = new Mock<IMapper>(MockBehavior.Strict);
            var assetRepo = new Mock<IAssetLocationQueryRepository>(MockBehavior.Strict);

            var sut = new DeleteLocationCommandHandler(cmdRepo.Object, mediator.Object, mapper.Object, qryRepo.Object, assetRepo.Object);

            var locationId = 5;
            qryRepo.Setup(r => r.GetByIdAsync(locationId)).ReturnsAsync(ActiveEntity(locationId));

            // Return DOMAIN AssetLocation, not DTO
            assetRepo.Setup(a => a.GetAllAssetLocationAsync(1, int.MaxValue, null))
                     .ReturnsAsync((new List<AssetLocation>
                     {
                         new AssetLocation { LocationId = locationId }
                     }, 1));

            var ex = await Assert.ThrowsExceptionAsync<ValidationException>(() =>
                sut.Handle(new DeleteLocationCommand { Id = locationId }, CancellationToken.None));

            StringAssert.Contains(ex.Message, "Cannot delete", StringComparison.OrdinalIgnoreCase);

            qryRepo.Verify(r => r.GetByIdAsync(locationId), Times.Once);
            assetRepo.Verify(a => a.GetAllAssetLocationAsync(1, int.MaxValue, null), Times.Once);
            qryRepo.VerifyNoOtherCalls();
            assetRepo.VerifyNoOtherCalls();
            cmdRepo.VerifyNoOtherCalls();
            mapper.VerifyNoOtherCalls();
            mediator.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task Handle_WhenDeleteSucceeds_Maps_Publishes_ReturnsDto()
        {
            var cmdRepo   = new Mock<ILocationCommandRepository>(MockBehavior.Strict);
            var qryRepo   = new Mock<ILocationQueryRepository>(MockBehavior.Strict);
            var mediator  = new Mock<IMediator>(MockBehavior.Strict);
            var mapper    = new Mock<IMapper>(MockBehavior.Strict);
            var assetRepo = new Mock<IAssetLocationQueryRepository>(MockBehavior.Strict);

            var sut = new DeleteLocationCommandHandler(cmdRepo.Object, mediator.Object, mapper.Object, qryRepo.Object, assetRepo.Object);

            var id = 7;
            var existing = ActiveEntity(id, code: "LOC-007", name: "Spare Store");

            qryRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);

            assetRepo.Setup(a => a.GetAllAssetLocationAsync(1, int.MaxValue, null))
                     .ReturnsAsync((new List<AssetLocation>(), 0));

            mapper.Setup(m => m.Map<Location>(It.IsAny<DeleteLocationCommand>()))
                  .Returns((DeleteLocationCommand c) => new Location { Id = c.Id, Code = "LOC-007", LocationName = "Spare Store" });

            cmdRepo.Setup(r => r.DeleteAsync(id, It.Is<Location>(l => l.Id == id))).ReturnsAsync(1);

            mapper.Setup(m => m.Map<LocationDto>(It.Is<Location>(l => l.Id == id)))
                  .Returns(new LocationDto { Id = id, Code = "LOC-007", LocationName = "Spare Store" });

            mediator.Setup(m => m.Publish(It.IsAny<AuditLogsDomainEvent>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            var result = await sut.Handle(new DeleteLocationCommand { Id = id }, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(id, result.Id);
            Assert.AreEqual("LOC-007", result.Code);

            qryRepo.Verify(r => r.GetByIdAsync(id), Times.Once);
            assetRepo.Verify(a => a.GetAllAssetLocationAsync(1, int.MaxValue, null), Times.Once);
            mapper.Verify(m => m.Map<Location>(It.IsAny<DeleteLocationCommand>()), Times.Once);
            cmdRepo.Verify(r => r.DeleteAsync(id, It.Is<Location>(l => l.Id == id)), Times.Once);
            mapper.Verify(m => m.Map<LocationDto>(It.Is<Location>(l => l.Id == id)), Times.Once);
            mediator.Verify(m => m.Publish(It.IsAny<AuditLogsDomainEvent>(), It.IsAny<CancellationToken>()), Times.Once);

            cmdRepo.VerifyNoOtherCalls();
            qryRepo.VerifyNoOtherCalls();
            mapper.VerifyNoOtherCalls();
            mediator.VerifyNoOtherCalls();
            assetRepo.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task Handle_WhenDeleteReturnsZero_ThrowsException_AndDoesNotPublish()
        {
            var cmdRepo   = new Mock<ILocationCommandRepository>(MockBehavior.Strict);
            var qryRepo   = new Mock<ILocationQueryRepository>(MockBehavior.Strict);
            var mediator  = new Mock<IMediator>(MockBehavior.Strict);
            var mapper    = new Mock<IMapper>(MockBehavior.Strict);
            var assetRepo = new Mock<IAssetLocationQueryRepository>(MockBehavior.Strict);

            var sut = new DeleteLocationCommandHandler(cmdRepo.Object, mediator.Object, mapper.Object, qryRepo.Object, assetRepo.Object);

            var id = 42;
            qryRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(ActiveEntity(id));
            assetRepo.Setup(a => a.GetAllAssetLocationAsync(1, int.MaxValue, null))
                     .ReturnsAsync((new List<AssetLocation>(), 0));

            mapper.Setup(m => m.Map<Location>(It.IsAny<DeleteLocationCommand>()))
                  .Returns(new Location { Id = id });

            cmdRepo.Setup(r => r.DeleteAsync(id, It.Is<Location>(l => l.Id == id))).ReturnsAsync(0);

            var ex = await Assert.ThrowsExceptionAsync<Exception>(() =>
                sut.Handle(new DeleteLocationCommand { Id = id }, CancellationToken.None));

            StringAssert.Contains(ex.Message, "deletion failed", StringComparison.OrdinalIgnoreCase);

            qryRepo.Verify(r => r.GetByIdAsync(id), Times.Once);
            assetRepo.Verify(a => a.GetAllAssetLocationAsync(1, int.MaxValue, null), Times.Once);
            mapper.Verify(m => m.Map<Location>(It.IsAny<DeleteLocationCommand>()), Times.Once);
            cmdRepo.Verify(r => r.DeleteAsync(id, It.Is<Location>(l => l.Id == id)), Times.Once);

            mediator.Verify(m => m.Publish(It.IsAny<AuditLogsDomainEvent>(), It.IsAny<CancellationToken>()), Times.Never);

            cmdRepo.VerifyNoOtherCalls();
            qryRepo.VerifyNoOtherCalls();
            mapper.VerifyNoOtherCalls();
            mediator.VerifyNoOtherCalls();
            assetRepo.VerifyNoOtherCalls();
        }
    }
}
