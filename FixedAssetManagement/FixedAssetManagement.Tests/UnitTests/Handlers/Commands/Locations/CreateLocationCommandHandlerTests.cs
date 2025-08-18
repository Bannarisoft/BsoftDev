using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using Core.Application.Location.Command.CreateLocation;  // handler + command
using Core.Application.Location.Queries.GetLocations;    // LocationDto
using Core.Application.Common.Interfaces.ILocation;      // repos
using Core.Domain.Events;                                // AuditLogsDomainEvent

namespace FixedAssetManagement.Tests.UnitTests.Locations
{
    [TestClass]
    public class CreateLocationCommandHandlerTests
    {
        [TestMethod]
        public async Task Handle_WhenLocationAlreadyExists_ThrowsValidationException()
        {
            var cmdRepo  = new Mock<ILocationCommandRepository>(MockBehavior.Strict);
            var qryRepo  = new Mock<ILocationQueryRepository>(MockBehavior.Strict);
            var mapper   = new Mock<IMapper>(MockBehavior.Strict);
            var mediator = new Mock<IMediator>(MockBehavior.Strict);

            var request = new CreateLocationCommand
            {
                LocationName = "Store A",
                DepartmentId = 10,
                UnitId       = 5
            };

            // IMPORTANT: pass the 4th optional int? argument explicitly
            qryRepo.Setup(r => r.GetByLocationNameAsync(
                              request.LocationName,
                              request.DepartmentId,
                              request.UnitId,
                              (int?)null))
                   .ReturnsAsync(new Core.Domain.Entities.Location());

            var sut = new CreateLocationCommandHandler(
                cmdRepo.Object, qryRepo.Object, mapper.Object, mediator.Object);

            await Assert.ThrowsExceptionAsync<ValidationException>(() =>
                sut.Handle(request, CancellationToken.None));

            qryRepo.Verify(r => r.GetByLocationNameAsync(
                             request.LocationName, request.DepartmentId, request.UnitId, (int?)null), Times.Once);
            cmdRepo.VerifyNoOtherCalls();
            mapper.VerifyNoOtherCalls();
            mediator.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task Handle_WhenNew_Creates_Maps_Publishes_ReturnsDto()
        {
            var cmdRepo  = new Mock<ILocationCommandRepository>(MockBehavior.Strict);
            var qryRepo  = new Mock<ILocationQueryRepository>(MockBehavior.Strict);
            var mapper   = new Mock<IMapper>(MockBehavior.Strict);
            var mediator = new Mock<IMediator>(MockBehavior.Strict);

            var request = new CreateLocationCommand
            {
                LocationName = "Main Warehouse",
                DepartmentId = 2,
                UnitId       = 1
            };

            qryRepo.Setup(r => r.GetByLocationNameAsync(
                              request.LocationName,
                              request.DepartmentId,
                              request.UnitId,
                              (int?)null))
                   .ReturnsAsync((Core.Domain.Entities.Location?)null);

            var toCreate = new Core.Domain.Entities.Location
            {
                Code         = "LOC-NEW",
                LocationName = request.LocationName,
                DepartmentId = request.DepartmentId,
                UnitId       = request.UnitId
            };
            mapper.Setup(m => m.Map<Core.Domain.Entities.Location>(request)).Returns(toCreate);

            var created = new Core.Domain.Entities.Location
            {
                Id           = 123,
                Code         = "LOC-123",
                LocationName = request.LocationName,
                DepartmentId = request.DepartmentId,
                UnitId       = request.UnitId
            };
            cmdRepo.Setup(r => r.CreateAsync(toCreate)).ReturnsAsync(created);

            var dto = new LocationDto
            {
                Id           = created.Id,
                Code         = created.Code,
                LocationName = created.LocationName,
                DepartmentId = created.DepartmentId,
                UnitId       = created.UnitId
            };
            mapper.Setup(m => m.Map<LocationDto>(created)).Returns(dto);

            mediator.Setup(m => m.Publish(It.IsAny<AuditLogsDomainEvent>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            var sut = new CreateLocationCommandHandler(
                cmdRepo.Object, qryRepo.Object, mapper.Object, mediator.Object);

            var result = await sut.Handle(request, CancellationToken.None);

            Assert.IsNotNull(result);
            Assert.AreEqual(123, result.Id);
            Assert.AreEqual("LOC-123", result.Code);
            Assert.AreEqual("Main Warehouse", result.LocationName);

            qryRepo.Verify(r => r.GetByLocationNameAsync(
                             request.LocationName, request.DepartmentId, request.UnitId, (int?)null), Times.Once);
            mapper.Verify(m => m.Map<Core.Domain.Entities.Location>(request), Times.Once);
            cmdRepo.Verify(r => r.CreateAsync(toCreate), Times.Once);
            mapper.Verify(m => m.Map<LocationDto>(created), Times.Once);
            mediator.Verify(m => m.Publish(It.IsAny<AuditLogsDomainEvent>(), It.IsAny<CancellationToken>()), Times.Once);

            qryRepo.VerifyNoOtherCalls();
            cmdRepo.VerifyNoOtherCalls();
            mapper.VerifyNoOtherCalls();
            mediator.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task Handle_WhenCreateReturnsZeroId_ThrowsException()
        {
            var cmdRepo  = new Mock<ILocationCommandRepository>(MockBehavior.Strict);
            var qryRepo  = new Mock<ILocationQueryRepository>(MockBehavior.Strict);
            var mapper   = new Mock<IMapper>(MockBehavior.Strict);
            var mediator = new Mock<IMediator>(MockBehavior.Strict);

            var request = new CreateLocationCommand
            {
                LocationName = "Temp",
                DepartmentId = 1,
                UnitId       = 1
            };

            qryRepo.Setup(r => r.GetByLocationNameAsync(
                              request.LocationName,
                              request.DepartmentId,
                              request.UnitId,
                              (int?)null))
                   .ReturnsAsync((Core.Domain.Entities.Location?)null);

            var toCreate = new Core.Domain.Entities.Location
            {
                Code         = "X",
                LocationName = "Temp"
            };
            mapper.Setup(m => m.Map<Core.Domain.Entities.Location>(request)).Returns(toCreate);

            var created = new Core.Domain.Entities.Location
            {
                Id           = 0,
                Code         = "X",
                LocationName = "Temp"
            };
            cmdRepo.Setup(r => r.CreateAsync(toCreate)).ReturnsAsync(created);

            // Map may not be used by the handler in this branch, but safe to set:
            mapper.Setup(m => m.Map<LocationDto>(created)).Returns(new LocationDto { Id = 0 });

            var sut = new CreateLocationCommandHandler(
                cmdRepo.Object, qryRepo.Object, mapper.Object, mediator.Object);

            var ex = await Assert.ThrowsExceptionAsync<System.Exception>(() =>
                sut.Handle(request, CancellationToken.None));

            StringAssert.Contains(ex.Message.ToLowerInvariant(), "not created");

            qryRepo.Verify(r => r.GetByLocationNameAsync(
                             request.LocationName, request.DepartmentId, request.UnitId, (int?)null), Times.Once);
            mapper.Verify(m => m.Map<Core.Domain.Entities.Location>(request), Times.Once);
            cmdRepo.Verify(r => r.CreateAsync(toCreate), Times.Once);
            mediator.VerifyNoOtherCalls();
        }
    }
}
