using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetTransferIssue;
using Grpc.Core;
using GrpcServices.FixedAsset;
using Microsoft.AspNetCore.Authorization;

namespace FAM.API.GrpcServices
{
      [AllowAnonymous]
    public class FixedAssetDepartmentValidationGrpcService : FixedAssetDepartmentValidationService.FixedAssetDepartmentValidationServiceBase
    {
        private readonly IAssetTransferQueryRepository _assetTransferQueryRepository;

        public FixedAssetDepartmentValidationGrpcService(IAssetTransferQueryRepository assetTransferQueryRepository)
        {
            _assetTransferQueryRepository = assetTransferQueryRepository;
        }

        public override async Task<DepartmentUsageResponse> IsDepartmentUsed(
         DepartmentUsageRequest request,
         ServerCallContext context)
        {
            var isUsed = await _assetTransferQueryRepository.DepartmentSoftDeleteValidation(request.DepartmentId);

            return new DepartmentUsageResponse
            {
                IsUsed = isUsed
            };
        }

    }
}