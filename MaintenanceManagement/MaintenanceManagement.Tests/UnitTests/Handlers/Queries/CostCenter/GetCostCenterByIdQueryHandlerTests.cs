using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using AutoMapper;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Core.Domain.Entities;
using Core.Domain.Events;
using Core.Application.Common.Interfaces.ICostCenter;
using Core.Application.CostCenter.Queries.GetCostCenter;          // CostCenterDto
using Core.Application.CostCenter.Queries.GetCostCenterById;     // GetCostCenterByIdQuery(+Handler)

using Contracts.Interfaces.External.IUser;                        // IDepartmentGrpcClient, IUnitGrpcClient
using Contracts.Dtos.Maintenance;                                // UnitDto (UnitId, UnitName)

namespace MaintenanceManagement.Tests.UnitTests.Handlers.Queries.CostCenter
{
    [TestClass]
    public class GetCostCenterByIdQueryHandlerTests
    {
        [TestMethod]
        public async Task Handle_Found_Enriches_Department_And_Unit_And_Publishes()
        {
            // Arrange
            var repo     = new Mock<ICostCenterQueryRepository>(MockBehavior.Strict);
            var mapper   = new Mock<IMapper>(MockBehavior.Strict);
            var mediator = new Mock<IMediator>(MockBehavior.Strict);
            var deptGrpc = new Mock<IDepartmentGrpcClient>(MockBehavior.Strict);
            var unitGrpc = new Mock<IUnitGrpcClient>(MockBehavior.Strict);

            var entity = new Core.Domain.Entities.CostCenter
            {
                Id = 101,
                CostCenterCode = "CC-101",
                CostCenterName = "Spinning",
                DepartmentId = 10,
                UnitId = 70
            };

            repo.Setup(r => r.GetByIdAsync(101)).ReturnsAsync(entity);

            mapper.Setup(m => m.Map<CostCenterDto>(entity))
                  .Returns(new CostCenterDto
                  {
                      Id = 101,
                      CostCenterCode = "CC-101",
                      CostCenterName = "Spinning",
                      DepartmentId = 10,
                      UnitId = 70
                  });

            // gRPC lookups
            deptGrpc.Setup(g => g.GetAllDepartmentAsync())
                    .Returns(Task.FromResult(new List<DepartmentDto>
                    {
                        new DepartmentDto { DepartmentId = 10, DepartmentName = "Dept-A" }
                    }));

            unitGrpc.Setup(g => g.GetAllUnitAsync())
                    .Returns(Task.FromResult(new List<UnitDto>
                    {
                        new UnitDto { UnitId = 70, UnitName = "Main Unit" }
                    }));

            mediator.Setup(m => m.Publish(It.IsAny<AuditLogsDomainEvent>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask)
                    .Verifiable();

            var sut = new GetCostCenterByIdQueryHandler(
                repo.Object, mapper.Object, mediator.Object, deptGrpc.Object, unitGrpc.Object);

            // Act
            var dto = await sut.Handle(new GetCostCenterByIdQuery { Id = 101 }, CancellationToken.None);

            // Assert
            dto.Should().NotBeNull();
            dto.Id.Should().Be(101);
            dto.CostCenterCode.Should().Be("CC-101");
            dto.DepartmentId.Should().Be(10);
            dto.UnitId.Should().Be(70);
            dto.DepartmentName.Should().Be("Dept-A");
            dto.UnitName.Should().Be("Main Unit");

            repo.Verify(r => r.GetByIdAsync(101), Times.Once);
            mapper.Verify(m => m.Map<CostCenterDto>(entity), Times.Once);
            deptGrpc.Verify(g => g.GetAllDepartmentAsync(), Times.Once);
            unitGrpc.Verify(g => g.GetAllUnitAsync(), Times.Once);
            mediator.Verify(m => m.Publish(It.IsAny<AuditLogsDomainEvent>(), It.IsAny<CancellationToken>()), Times.Once);

            repo.VerifyNoOtherCalls();
            mapper.VerifyNoOtherCalls();
            deptGrpc.VerifyNoOtherCalls();
            unitGrpc.VerifyNoOtherCalls();
            mediator.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task Handle_Found_But_Names_Missing_Leaves_Null_And_Publishes()
        {
            // Arrange
            var repo     = new Mock<ICostCenterQueryRepository>(MockBehavior.Strict);
            var mapper   = new Mock<IMapper>(MockBehavior.Strict);
            var mediator = new Mock<IMediator>(MockBehavior.Strict);
            var deptGrpc = new Mock<IDepartmentGrpcClient>(MockBehavior.Strict);
            var unitGrpc = new Mock<IUnitGrpcClient>(MockBehavior.Strict);

            var entity = new Core.Domain.Entities.CostCenter
            {
                Id = 202,
                CostCenterCode = "CC-202",
                CostCenterName = "Carding",
                DepartmentId = 999, // not present in gRPC list
                UnitId = 888        // not present in gRPC list
            };

            repo.Setup(r => r.GetByIdAsync(202)).ReturnsAsync(entity);

            mapper.Setup(m => m.Map<CostCenterDto>(entity))
                  .Returns(new CostCenterDto
                  {
                      Id = 202,
                      CostCenterCode = "CC-202",
                      CostCenterName = "Carding",
                      DepartmentId = 999,
                      UnitId = 888
                  });

            // Return lists that don't contain the IDs above
            deptGrpc.Setup(g => g.GetAllDepartmentAsync())
                    .Returns(Task.FromResult(new List<DepartmentDto>
                    {
                        new DepartmentDto { DepartmentId = 10, DepartmentName = "Dept-A" }
                    }));

            unitGrpc.Setup(g => g.GetAllUnitAsync())
                    .Returns(Task.FromResult(new List<UnitDto>
                    {
                        new UnitDto { UnitId = 70, UnitName = "Main Unit" }
                    }));

            mediator.Setup(m => m.Publish(It.IsAny<AuditLogsDomainEvent>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask)
                    .Verifiable();

            var sut = new GetCostCenterByIdQueryHandler(
                repo.Object, mapper.Object, mediator.Object, deptGrpc.Object, unitGrpc.Object);

            // Act
            var dto = await sut.Handle(new GetCostCenterByIdQuery { Id = 202 }, CancellationToken.None);

            // Assert
            dto.Should().NotBeNull();
            dto.Id.Should().Be(202);
            dto.DepartmentId.Should().Be(999);
            dto.UnitId.Should().Be(888);
            dto.DepartmentName.Should().BeNull();
            dto.UnitName.Should().BeNull();

            repo.Verify(r => r.GetByIdAsync(202), Times.Once);
            mapper.Verify(m => m.Map<CostCenterDto>(entity), Times.Once);
            deptGrpc.Verify(g => g.GetAllDepartmentAsync(), Times.Once);
            unitGrpc.Verify(g => g.GetAllUnitAsync(), Times.Once);
            mediator.Verify(m => m.Publish(It.IsAny<AuditLogsDomainEvent>(), It.IsAny<CancellationToken>()), Times.Once);

            repo.VerifyNoOtherCalls();
            mapper.VerifyNoOtherCalls();
            deptGrpc.VerifyNoOtherCalls();
            unitGrpc.VerifyNoOtherCalls();
            mediator.VerifyNoOtherCalls();
        }
    }
}
