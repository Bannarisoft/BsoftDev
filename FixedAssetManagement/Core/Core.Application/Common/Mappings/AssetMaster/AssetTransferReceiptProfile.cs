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
            // .ForMember(dest => dest.AssetTransferIssueHdr, opt => opt.Ignore()) ;
            CreateMap<AssetTransferReceiptDtlDto, Core.Domain.Entities.AssetMaster.AssetTransferReceiptDtl>()
            .ForMember(dest => dest.AckStatus, opt => opt.MapFrom(src => src.AckStatus ?? 0))
            .ForMember(dest => dest.AckDate, opt => opt.MapFrom(src => src.AckStatus == 1 ? DateTimeOffset.UtcNow : (DateTimeOffset?)null));
            

             CreateMap<AssetTransferReceiptDtlDto, Core.Domain.Entities.AssetMaster.AssetLocation>()
            .ForMember(dest => dest.AssetId, opt => opt.MapFrom(src => src.AssetId))
            .ForMember(dest => dest.LocationId, opt => opt.MapFrom(src => src.LocationId))
            .ForMember(dest => dest.SubLocationId, opt => opt.MapFrom(src => src.SubLocationId))
            .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.UserID ?? 0));


            // CreateMap<AssetTransferIssueHdrIdDto, AssetTransferIssueHdr>()
            // .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AssetTransferId))
            // .ForMember(dest => dest.AckStatus, opt => opt.MapFrom(src => (byte)1));  // Ensure AckStatus is always 1
        }
    }
}