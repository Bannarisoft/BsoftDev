using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetMaster.AssetTransferReceipt.Queries.GetAssetReceiptPending;
using Core.Domain.Entities.AssetMaster;

namespace Core.Application.Common.Mappings.AssetMaster
{
    public class AssetTransferReceiptProfile : Profile
    {
        public AssetTransferReceiptProfile()
        {
            
            CreateMap<AssetTransferReceiptHdrDto, Core.Domain.Entities.AssetMaster.AssetTransferReceiptHdr>()
             .ForMember(dest => dest.AssetTransferReceiptDtl, opt => opt.MapFrom(src => src.AssetTransferReceiptDtl));
            CreateMap<AssetTransferReceiptDtlDto, Core.Domain.Entities.AssetMaster.AssetTransferReceiptDtl>();
        }
    }
}