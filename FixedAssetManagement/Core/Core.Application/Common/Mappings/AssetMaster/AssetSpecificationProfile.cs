using AutoMapper;
using Core.Application.AssetMaster.AssetSpecification.Commands.CreateAssetSpecification;
using Core.Application.AssetMaster.AssetSpecification.Commands.DeleteAssetSpecification;
using Core.Application.AssetMaster.AssetSpecification.Commands.UpdateAssetSpecification;
using Core.Application.AssetMaster.AssetSpecification.Queries.GetAssetSpecification;
using Core.Domain.Entities.AssetMaster;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings.AssetMaster
{
    public class AssetSpecificationProfile : Profile
    {
        public AssetSpecificationProfile()
        { 
            CreateMap<DeleteAssetSpecificationCommand, AssetSpecifications>()            
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted));            
            
            CreateMap<CreateAssetSpecificationCommand, AssetSpecifications>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))            
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted)); 


            CreateMap<UpdateAssetSpecificationCommand, AssetSpecifications>()
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => (Status)src.IsActive));

            CreateMap<AssetSpecificationJsonDto, AssetSpecificationAutoCompleteDTO>();
            CreateMap<AssetSpecifications, AssetSpecificationDTO>();      
            CreateMap<AssetSpecificationJsonDto, AssetSpecificationDTO>();
        }       
    }
}