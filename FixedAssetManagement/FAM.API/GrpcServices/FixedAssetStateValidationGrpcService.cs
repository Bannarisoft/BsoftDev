using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IManufacture;
using Grpc.Core;
using GrpcServices.FixedAsset;

namespace FAM.API.GrpcServices
{
    public class FixedAssetStateValidationGrpcService : FixedAssetStateService.FixedAssetStateServiceBase
    {

        private readonly IManufactureQueryRepository _manufactureQueryRepository;
        public FixedAssetStateValidationGrpcService(IManufactureQueryRepository manufactureQueryRepository)
        {
            _manufactureQueryRepository = manufactureQueryRepository;
        }
        
        public override async Task<StateUsageResponse> IsStateUsed(
         StateUsageRequest request,
         ServerCallContext context)
        {
            try
                {
                    var isUsed = await _manufactureQueryRepository.StateSoftDeleteValidation(request.StateId);

                    return new StateUsageResponse
                    {
                        IsUsed = isUsed
                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"gRPC Error: {ex.Message}");
                    throw new RpcException(new Status(StatusCode.Internal, "Internal error occurred while checking State usage."));
                }
        }

        
    }
}