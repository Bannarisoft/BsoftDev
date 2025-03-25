using AutoMapper;
using Core.Application.AssetMaster.AssetWarranty.Commands.CreateAssetWarranty;
using Core.Application.AssetMaster.AssetWarranty.Commands.DeleteAssetWarranty;
using Core.Application.AssetMaster.AssetWarranty.Commands.UpdateAssetWarranty;
using Core.Application.AssetMaster.AssetWarranty.Queries.GetAssetWarranty;
using Core.Domain.Entities.AssetMaster;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings.AssetMaster
{
    public class AssetWarrantyProfile : Profile
    {
        public AssetWarrantyProfile()
        { 
            CreateMap<DeleteAssetWarrantyCommand, AssetWarranties>()            
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted));            
            
            CreateMap<CreateAssetWarrantyCommand, AssetWarranties>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))            
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted)); 

            CreateMap<UpdateAssetWarrantyCommand, AssetWarranties>()
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ==1 ? Status.Active : Status.Inactive));            

            CreateMap<AssetWarrantyDTO, AssetWarrantyAutoCompleteDTO>();
            CreateMap<AssetWarranties, AssetWarrantyDTO>();      
        }       
    }
}