using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetMaster.AssetAdditionalCost.Commands.CreateAssetAdditionalCost;
using Core.Application.AssetMaster.AssetAdditionalCost.Commands.UpdateAssetAdditionalCost;
using Core.Application.AssetMaster.AssetAdditionalCost.Queries.GetAssetAdditionalCost;
using Core.Domain.Entities.AssetPurchase;

namespace Core.Application.Common.Mappings.AssetMaster
{
    public class AssetAdditionalCostProfile : Profile
    {
        public AssetAdditionalCostProfile()
        {
           CreateMap<AssetAdditionalCost,AssetAdditionalCostDto>();
           CreateMap<CreateAssetAdditionalCostCommand, AssetAdditionalCost>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
           CreateMap<UpdateAssetAdditionalCostCommand, AssetAdditionalCost>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AssetId, opt => opt.Ignore())
                .ForMember(dest => dest.AssetSourceId, opt => opt.Ignore());

            

        }
    }
}