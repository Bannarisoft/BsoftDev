using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Moq;
using AutoMapper;
using MediatR;

using Core.Application.Common.Interfaces.ICostCenter;
using Core.Application.CostCenter.Queries.GetCostCenter;
using Core.Domain.Entities;
using Contracts.Interfaces.External.IUser;      // IDepartmentAllGrpcClient, IUnitGrpcClient
using Contracts.Dtos.Maintenance;             // UnitDto (UnitId, UnitName)
using Contracts.Dtos.Users;                   // <-- DepartmentAllDto (DepartmentId, DepartmentName)

namespace MaintenanceManagement.Tests.UnitTests.Handlers.Queries.CostCenter
{
    [TestClass]
    public class GetCostCenterQueryHandlerTests
    {
        [TestMethod]
        public async Task Handle_Returns_List_Of_CostCenters()
        {
            var repoQuery = new Mock<ICostCenterQueryRepository>(MockBehavior.Strict);
            var mapper    = new Mock<IMapper>(MockBehavior.Strict);
            var mediator  = new Mock<IMediator>(MockBehavior.Strict);
            var deptGrpc  = new Mock<IDepartmentAllGrpcClient>(MockBehavior.Strict);
            var unitGrpc  = new Mock<IUnitGrpcClient>(MockBehavior.Strict);

            repoQuery.Setup(r => r.GetAllCostCenterGroupAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>()))
                     .ReturnsAsync((
                         new List<CostCenter>
                         {
                             new CostCenter { Id = 1, CostCenterCode = "CC-001", CostCenterName = "Spinning", UnitId = 70, DepartmentId = 10 },
                             new CostCenter { Id = 2, CostCenterCode = "CC-002", CostCenterName = "Winding",  UnitId = 70, DepartmentId = 20 }
                         },
                         2
                     ));

            mapper.Setup(m => m.Map<List<CostCenterDto>>(It.IsAny<List<CostCenter>>()))
                  .Returns(new List<CostCenterDto>
                  {
                      new CostCenterDto { Id = 1, CostCenterCode = "CC-001", CostCenterName = "Spinning", UnitId = 70, DepartmentId = 10 },
                      new CostCenterDto { Id = 2, CostCenterCode = "CC-002", CostCenterName = "Winding",  UnitId = 70, DepartmentId = 20 }
                  });

            // ✅ Must return List<DepartmentAllDto> (from Contracts.Dtos.Users)
            deptGrpc.Setup(g => g.GetDepartmentAllAsync())
                    .ReturnsAsync(new List<DepartmentAllDto>
                    {
                        new DepartmentAllDto { DepartmentId = 10, DepartmentName = "Dept-A" },
                        new DepartmentAllDto { DepartmentId = 20, DepartmentName = "Dept-B" }
                    });
            // If your Moq complains about ReturnsAsync, use:
            // .Returns(Task.FromResult(new List<DepartmentAllDto> { ... }))

            // Unit gRPC method name is GetUnitAsync and returns List<UnitDto>
            unitGrpc.Setup(g => g.GetAllUnitAsync())
                    .ReturnsAsync(new List<UnitDto>
                    {
                        new UnitDto { UnitId = 70, UnitName = "Main Unit" }
                    });

            mediator.Setup(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            var handler = new GetCostCenterQueryHandler(
                repoQuery.Object, mapper.Object, mediator.Object, deptGrpc.Object, unitGrpc.Object
            );

            var query = new GetCostCenterQuery
            {
                PageNumber = 1,
                PageSize   = 10,
                SearchTerm = null
            };

            var result = await handler.Handle(query, CancellationToken.None);

            result.Should().NotBeNull();
            result.Data.Should().NotBeNull();
            result.Data.Should().HaveCount(2);
            result.Data[0].CostCenterCode.Should().Be("CC-001");

            repoQuery.Verify(r => r.GetAllCostCenterGroupAsync(1, 10, null), Times.Once);
            mapper.Verify(m => m.Map<List<CostCenterDto>>(It.IsAny<List<CostCenter>>()), Times.Once);
            deptGrpc.Verify(g => g.GetDepartmentAllAsync(), Times.Once);
            unitGrpc.Verify(g => g.GetAllUnitAsync(), Times.Once);
            mediator.Verify(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.AtMostOnce);

            repoQuery.VerifyNoOtherCalls();
            mapper.VerifyNoOtherCalls();
            deptGrpc.VerifyNoOtherCalls();
            unitGrpc.VerifyNoOtherCalls();
        }
    }
}
