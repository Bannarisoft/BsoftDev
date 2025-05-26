using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetSpecification;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcServices.FixedAssetManagement;
using Microsoft.AspNetCore.Authorization;


[Authorize]
public class AssetSpecificationGrpcService : AssetSpecificationService.AssetSpecificationServiceBase
{
   private readonly IAssetSpecificationQueryRepository _assetSpecificationQueryRepository;
   public AssetSpecificationGrpcService(IAssetSpecificationQueryRepository assetSpecificationQueryRepository)
   {
      _assetSpecificationQueryRepository = assetSpecificationQueryRepository;
   }

   public override async Task<AssetSpecificationListResponse> GetAllAssetSpecification(Empty request, ServerCallContext context)
   {
      var (items, totalCount) = await _assetSpecificationQueryRepository.GetAssetSpecBasedOnMachineNos(1, 100, null);
      var response = new AssetSpecificationListResponse();
      response.AssetSpecifications.AddRange(items.Select(d => new GrpcServices.FixedAssetManagement.AssetSpecificationDto
      {
         AssetId = d.AssetId,
         SpecificationName = d.SpecificationName,
         SpecificationValue = d.SpecificationValue,
         CapitalizationDate = Timestamp.FromDateTimeOffset(d.CapitalizationDate)
      }));

      return response;
   }
}
