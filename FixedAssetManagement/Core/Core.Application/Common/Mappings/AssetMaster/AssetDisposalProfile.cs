using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetMaster.AssetDisposal.Command.CreateAssetDisposal;
using Core.Application.AssetMaster.AssetDisposal.Command.UpdateAssetDisposal;
using Core.Application.AssetMaster.AssetDisposal.Queries.GetAssetDisposal;
using Core.Domain.Entities.AssetMaster;

namespace Core.Application.Common.Mappings.AssetMaster
{
    public class AssetDisposalProfile : Profile
    {
        public AssetDisposalProfile()
        {
           CreateMap<AssetDisposal,AssetDisposalDto>();
           CreateMap<CreateAssetDisposalCommand, AssetDisposal>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
           CreateMap<UpdateAssetDisposalCommand, AssetDisposal>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AssetId, opt => opt.Ignore())
                .ForMember(dest => dest.AssetPurchaseId, opt => opt.Ignore());
        }   
    }
}