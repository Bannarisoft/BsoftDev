using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IManufacture;
using Grpc.Core;
using GrpcServices.FixedAsset;
using Microsoft.AspNetCore.Authorization;

namespace FAM.API.GrpcServices
{
       [AllowAnonymous]
    public class FixedAssetCityValidationGrpcService : FixedAssetCityService.FixedAssetCityServiceBase
    {

        private readonly IManufactureQueryRepository _manufactureQueryRepository;
        public FixedAssetCityValidationGrpcService(IManufactureQueryRepository manufactureQueryRepository)
        {

            _manufactureQueryRepository = manufactureQueryRepository;

        }

        public override async Task<CityUsageResponse> IsCityUsed(
         CityUsageRequest request,
         ServerCallContext context)
        {
            try
            {
                var isUsed = await _manufactureQueryRepository.CitySoftDeleteValidation(request.CityId);

                return new CityUsageResponse
                {
                    IsUsed = isUsed
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"gRPC Error: {ex.Message}");
                throw new RpcException(new Status(StatusCode.Internal, "Internal error occurred while checking City usage."));
            }
        }

    }
}