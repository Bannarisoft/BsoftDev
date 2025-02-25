
using AutoMapper;
using Core.Application.AssetLocation.Queries.GetAssetLocation;
using Core.Application.AssetMaster.AssetMasterGeneral.Commands.CreateAssetMasterGeneral;
using Core.Application.AssetMaster.AssetMasterGeneral.Commands.DeleteAssetMasterGeneral;
using Core.Application.AssetMaster.AssetMasterGeneral.Commands.UpdateAssetMasterGeneral;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.AssetMaster.AssetPurchase.Queries.GetAssetPurchase;
using Core.Domain.Entities;
using Core.Domain.Entities.AssetPurchase;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings.AssetMaster
{
    public class AssetMasterGeneralProfile : Profile
    {
        public AssetMasterGeneralProfile()
        { 
            CreateMap<DeleteAssetMasterGeneralCommand, AssetMasterGenerals>()            
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted));            
            
            CreateMap<CreateAssetMasterGeneralCommand, AssetMasterGenerals>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))            
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted)); 

            CreateMap<UpdateAssetMasterGeneralCommand, AssetMasterGenerals>()
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => IsDelete.NotDeleted));     
             
            CreateMap<AssetMasterGeneralDTO,AssetMasterGeneralAutoCompleteDTO>();    
            CreateMap<AssetMasterGenerals, AssetMasterGeneralDTO>();             
            // Mapping for the composite DTO with reverse mapping enabled.

            // **Add these mappings to clear your error:**
            CreateMap<AssetPurchaseDetailsDto, AssetPurchaseDetails>()
                .ReverseMap();

            CreateMap<AssetLocationDto, Core.Domain.Entities.AssetMaster.AssetLocation>()
                .ReverseMap(); 
        }        
    }
}
