using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using Core.Domain.Events;                                      // AuditLogsDomainEvent
using Core.Application.Common.Interfaces.ILocation;            // ILocationQueryRepository
using Core.Application.Location.Queries.GetLocations;          // LocationDto
using Core.Application.Location.Queries.GetLocationById;       // GetLocationByIdQuery, Handler
using Core.Domain.Entities;                                    // Location (domain entity)

namespace FixedAssetManagement.Tests.UnitTests.Handlers.Queries.Locations
{
    [TestClass]
    public class GetLocationByIdQueryHandlerTests
    {
        [TestMethod]
        public async Task Handle_Found_MapsAndPublishes_ReturnsDto()
        {
            // Arrange
            var repo     = new Mock<ILocationQueryRepository>(MockBehavior.Strict);
            var mapper   = new Mock<IMapper>(MockBehavior.Strict);
            var mediator = new Mock<IMediator>(MockBehavior.Strict);

            var entity = new Location
            {
                Id = 123,
                Code = "LOC-123",
                LocationName = "Main Store",
                DepartmentId = 10
            };

            repo.Setup(r => r.GetByIdAsync(123))
                .ReturnsAsync(entity);

            var mapped = new LocationDto
            {
                Id = 123,
                Code = "LOC-123",
                LocationName = "Main Store",
                DepartmentId = 10
            };

            mapper.Setup(m => m.Map<LocationDto>(entity))
                  .Returns(mapped);

            mediator.Setup(m => m.Publish(It.IsAny<AuditLogsDomainEvent>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            var sut = new GetLocationByIdQueryHandler(repo.Object, mediator.Object, mapper.Object);

            // Act
            var result = await sut.Handle(new GetLocationByIdQuery { Id = 123 }, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(123, result.Id);
            Assert.AreEqual("LOC-123", result.Code);
            Assert.AreEqual("Main Store", result.LocationName);
            Assert.AreEqual(10, result.DepartmentId);

            repo.Verify(r => r.GetByIdAsync(123), Times.Once);
            mapper.Verify(m => m.Map<LocationDto>(entity), Times.Once);
            mediator.Verify(m => m.Publish(It.IsAny<AuditLogsDomainEvent>(), It.IsAny<CancellationToken>()), Times.Once);

            repo.VerifyNoOtherCalls();
            mapper.VerifyNoOtherCalls();
            mediator.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task Handle_NotFound_ThrowsValidationException_DoesNotPublish()
        {
            // Arrange
            var repo     = new Mock<ILocationQueryRepository>(MockBehavior.Strict);
            var mapper   = new Mock<IMapper>(MockBehavior.Strict);
            var mediator = new Mock<IMediator>(MockBehavior.Strict);

            repo.Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Location?)null);

            var sut = new GetLocationByIdQueryHandler(repo.Object, mediator.Object, mapper.Object);

            // Act + Assert
            var ex = await Assert.ThrowsExceptionAsync<ValidationException>(() =>
                sut.Handle(new GetLocationByIdQuery { Id = 999 }, CancellationToken.None));

            StringAssert.Contains(ex.Message.ToLowerInvariant(), "not found");

            repo.Verify(r => r.GetByIdAsync(999), Times.Once);

            // ensure no mapping or publish when not found
            mapper.VerifyNoOtherCalls();
            mediator.VerifyNoOtherCalls();
            repo.VerifyNoOtherCalls();
        }
    }
}
