using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.ICostCenter;
using Grpc.Core;
using GrpcServices.Maintenance;
using Microsoft.AspNetCore.Authorization;

namespace MaintenanceManagement.API.GrpcServices
{
    // [Authorize] // 👈 Optional: Remove this if you don't want token enforcement
    [AllowAnonymous]
    public class DepartmentValidationGrpcService : DepartmentValidationService.DepartmentValidationServiceBase
    {
        private readonly ICostCenterQueryRepository _costCenterQueryRepository;

        public DepartmentValidationGrpcService(ICostCenterQueryRepository costCenterQueryRepository)
        {
            _costCenterQueryRepository = costCenterQueryRepository;
        }

        public override async Task<DepartmentUsageResponse> IsDepartmentUsedInCostCenter(
            DepartmentUsageRequest request,
            ServerCallContext context)
        {
            var isUsed = await _costCenterQueryRepository.DepartmentSoftDeleteValidation(request.DepartmentId);      

            return new DepartmentUsageResponse
            {
                IsUsed = isUsed
            };
        }
    }
}
