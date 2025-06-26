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
    public class FixedAssetCountryValidationGrpcService : FixedAssetCountryService.FixedAssetCountryServiceBase
    {

        private readonly IManufactureQueryRepository _manufactureQueryRepository;

        public FixedAssetCountryValidationGrpcService(IManufactureQueryRepository manufactureQueryRepository)
        {
            _manufactureQueryRepository = manufactureQueryRepository;
        }

        public override async Task<CountryUsageResponse> IsCountryUsed(
         CountryUsageRequest request,
         ServerCallContext context)
        {
            try
                {
                    var isUsed = await _manufactureQueryRepository.CountrySoftDeleteValidation(request.CountryId);

                    return new CountryUsageResponse
                    {
                        IsUsed = isUsed
                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"gRPC Error: {ex.Message}");
                    throw new RpcException(new Status(StatusCode.Internal, "Internal error occurred while checking country usage."));
                }
        }
    }
}