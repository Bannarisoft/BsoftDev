
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
            
            CreateMap<AssetMasterDto, AssetMasterGenerals>()
            .ForMember(dest => dest.AssetPurchase, opt => opt.MapFrom(src => src.AssetPurchaseDetails))
            .ForMember(dest => dest.AssetLocation, opt => opt.MapFrom(src => src.AssetLocation)) 
            .ForMember(dest => dest.Id, opt => opt.Ignore());  
                       
            // Mapping for the composite DTO with reverse mapping enabled.

            // **Add these mappings to clear your error:**
            CreateMap<AssetPurchaseCombineDto, AssetPurchaseDetails>()
                .ReverseMap();

            CreateMap<AssetLocationCombineDto, Core.Domain.Entities.AssetMaster.AssetLocation>()
                .ReverseMap(); 
        }        
    }
}
