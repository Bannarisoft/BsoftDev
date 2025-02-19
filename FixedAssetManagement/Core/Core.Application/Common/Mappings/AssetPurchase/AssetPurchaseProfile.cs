using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetMaster.AssetPurchase;
using Core.Application.AssetMaster.AssetPurchase.Queries;
using Core.Application.AssetMaster.AssetPurchase.Queries.GetAssetGrnDetails;
using Core.Application.AssetMaster.AssetPurchase.Queries.GetAssetGRNItem;
using Core.Application.AssetMaster.AssetPurchase.Queries.GetAssetSourceAutoComplete;
using Core.Domain.Entities.AssetPurchase;

namespace Core.Application.Common.Mappings.AssetPurchase
{
    public class AssetPurchaseProfile : Profile
    {
        
        public AssetPurchaseProfile()
        {
               CreateMap<Core.Domain.Entities.AssetSource, AssetSourceAutoCompleteDto>(); 
               CreateMap<Core.Domain.Entities.AssetPurchase.AssetUnit, AssetUnitAutoCompleteDto>(); 
               CreateMap<Core.Domain.Entities.AssetPurchase.AssetGrn, GetAssetGrnDto>();
               CreateMap<Core.Domain.Entities.AssetPurchase.AssetGrnItem, AssetGrnItemDto>();
               CreateMap<Core.Domain.Entities.AssetPurchase.AssetGrnDetails, AssetDetailsDto>();
        }      
    }
}