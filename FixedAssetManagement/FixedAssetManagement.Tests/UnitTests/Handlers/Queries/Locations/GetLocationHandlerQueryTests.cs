using Moq;
using AutoMapper;
using MediatR;
using Core.Application.Location.Queries.GetLocations;      // GetLocationHandlerQuery, GetLocationQuery, LocationDto
using Core.Application.Common.Interfaces.ILocation;        // ILocationQueryRepository
using Core.Domain.Events;                                  // AuditLogsDomainEvent
using Core.Domain.Entities;                                 // Location (domain entity)
using Contracts.Interfaces.External.IUser;                 // IDepartmentAllGrpcClient
using UsersDtos = Contracts.Dtos.Users;                    // ✅ alias for DepartmentAllDto

namespace FixedAssetManagement.Tests.UnitTests.Handlers.Queries.Locations
{
    [TestClass]
    public class GetLocationHandlerQueryTests
    {
        private static List<Location> MakeDomainLocations() => new()
        {
            new Location { Id = 1, Code = "LOC-001", LocationName = "Main Store", DepartmentId = 10 },
            new Location { Id = 2, Code = "LOC-002", LocationName = "Spare Store", DepartmentId = 20 }
        };

        private static List<LocationDto> MakeMappedDtos() => new()
        {
            new LocationDto { Id = 1, Code = "LOC-001", LocationName = "Main Store", DepartmentId = 10, DepartmentName = null },
            new LocationDto { Id = 2, Code = "LOC-002", LocationName = "Spare Store", DepartmentId = 20, DepartmentName = null }
        };

        [TestMethod]
        public async Task Handle_ReturnsPagedLocations_InsertsDepartmentNames_PublishesAudit()
        {
            // Arrange
            var repo = new Mock<ILocationQueryRepository>(MockBehavior.Strict);
            var mapper = new Mock<IMapper>(MockBehavior.Strict);
            var mediator = new Mock<IMediator>(MockBehavior.Strict);
            var deptClient = new Mock<IDepartmentAllGrpcClient>(MockBehavior.Strict);

            int page = 2, size = 5, total = 27;
            var domainList = MakeDomainLocations();
            var dtoList = MakeMappedDtos();

            repo.Setup(r => r.GetAllLocationAsync(page, size, "store"))
                .ReturnsAsync((domainList, total));

            mapper.Setup(m => m.Map<List<LocationDto>>(domainList))
                  .Returns(dtoList);

            // Departments: only 10 exists; 20 missing -> second item should remain null
            var departments = new List<UsersDtos.DepartmentAllDto>
            {
                new UsersDtos.DepartmentAllDto { DepartmentId = 10, DepartmentName = "Dept-A" }
            };
            deptClient.Setup(d => d.GetDepartmentAllAsync())
                      .ReturnsAsync(departments);

            mediator.Setup(m => m.Publish(It.IsAny<AuditLogsDomainEvent>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            var sut = new GetLocationHandlerQuery(repo.Object, mediator.Object, mapper.Object, deptClient.Object);

            // Act
            var result = await sut.Handle(new GetLocationQuery
            {
                PageNumber = page,
                PageSize = size,
                SearchTerm = "store"
            }, CancellationToken.None);

            // Assert: envelope
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("Success", result.Message);
            Assert.AreEqual(total, result.TotalCount);
            Assert.AreEqual(page, result.PageNumber);
            Assert.AreEqual(size, result.PageSize);

            // Assert: data mapping & department names
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(2, result.Data!.Count);

            var first = result.Data[0];
            var second = result.Data[1];

            Assert.AreEqual(1, first.Id);
            Assert.AreEqual("LOC-001", first.Code);
            Assert.AreEqual(10, first.DepartmentId);
            Assert.AreEqual("Dept-A", first.DepartmentName);     // filled from gRPC

            Assert.AreEqual(2, second.Id);
            Assert.AreEqual("LOC-002", second.Code);
            Assert.AreEqual(20, second.DepartmentId);
            Assert.IsTrue(string.IsNullOrEmpty(second.DepartmentName)); // no matching dept

            // Verify interactions
            repo.Verify(r => r.GetAllLocationAsync(page, size, "store"), Times.Once);
            mapper.Verify(m => m.Map<List<LocationDto>>(domainList), Times.Once);
            deptClient.Verify(d => d.GetDepartmentAllAsync(), Times.Once);
            mediator.Verify(m => m.Publish(It.IsAny<AuditLogsDomainEvent>(), It.IsAny<CancellationToken>()), Times.Once);

            repo.VerifyNoOtherCalls();
            mapper.VerifyNoOtherCalls();
            deptClient.VerifyNoOtherCalls();
            mediator.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task Handle_WhenNoLocations_ReturnsEmptyList_StillPublishesAudit()
        {
            // Arrange
            var repo = new Mock<ILocationQueryRepository>(MockBehavior.Strict);
            var mapper = new Mock<IMapper>(MockBehavior.Strict);
            var mediator = new Mock<IMediator>(MockBehavior.Strict);
            var deptClient = new Mock<IDepartmentAllGrpcClient>(MockBehavior.Strict);

            repo.Setup(r => r.GetAllLocationAsync(1, 10, null))
                .ReturnsAsync((new List<Location>(), 0));

            mapper.Setup(m => m.Map<List<LocationDto>>(It.IsAny<List<Location>>()))
                  .Returns(new List<LocationDto>());

            deptClient.Setup(d => d.GetDepartmentAllAsync())
                      .ReturnsAsync(new List<UsersDtos.DepartmentAllDto>());

            mediator.Setup(m => m.Publish(It.IsAny<AuditLogsDomainEvent>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            var sut = new GetLocationHandlerQuery(repo.Object, mediator.Object, mapper.Object, deptClient.Object);

            // Act
            var result = await sut.Handle(new GetLocationQuery
            {
                PageNumber = 1,
                PageSize = 10,
                SearchTerm = null
            }, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(0, result.TotalCount);
            Assert.AreEqual(1, result.PageNumber);
            Assert.AreEqual(10, result.PageSize);
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(0, result.Data!.Count);

            // Verify
            repo.Verify(r => r.GetAllLocationAsync(1, 10, null), Times.Once);
            mapper.Verify(m => m.Map<List<LocationDto>>(It.Is<List<Location>>(l => l.Count == 0)), Times.Once);
            deptClient.Verify(d => d.GetDepartmentAllAsync(), Times.Once);
            mediator.Verify(m => m.Publish(It.IsAny<AuditLogsDomainEvent>(), It.IsAny<CancellationToken>()), Times.Once);

            repo.VerifyNoOtherCalls();
            mapper.VerifyNoOtherCalls();
            deptClient.VerifyNoOtherCalls();
            mediator.VerifyNoOtherCalls();
        }
    }
}
