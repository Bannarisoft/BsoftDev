
using AutoMapper;
using Core.Application.AssetLocation.Queries.GetAssetLocation;
using Core.Application.AssetMaster.AssetMasterGeneral.Commands.CreateAssetMasterGeneral;
using Core.Application.AssetMaster.AssetMasterGeneral.Commands.DeleteAssetMasterGeneral;
using Core.Application.AssetMaster.AssetMasterGeneral.Commands.UpdateAssetMasterGeneral;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneralById;
using Core.Application.AssetMaster.AssetPurchase.Queries.GetAssetPurchase;
using Core.Application.AssetMaster.AssetSpecification.Queries.GetAssetSpecification;
using Core.Domain.Entities;
using Core.Domain.Entities.AssetMaster;
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

            /*  CreateMap<UpdateAssetMasterGeneralCommand, AssetMasterGenerals>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.AssetMaster.IsActive ==1 ? Status.Active : Status.Inactive))
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore()); */
             
            CreateMap<AssetMasterGeneralDTO,AssetMasterGeneralAutoCompleteDTO>();    
            CreateMap<AssetMasterGenerals, AssetMasterGeneralDTO>();  
            
            
            CreateMap<AssetMasterDto, AssetMasterGenerals>()
            .ForMember(dest => dest.AssetPurchase, opt => opt.MapFrom(src => src.AssetPurchaseDetails))
            .ForMember(dest => dest.AssetLocation, opt => opt.MapFrom(src => src.AssetLocation)) 
            .ForMember(dest => dest.AssetAdditionalCost, opt => opt.MapFrom(src => src.AssetAdditionalCost)) 
            .ForMember(dest => dest.AssetSpecification, opt => opt.MapFrom(src => src.AssetSpecification))            
            .ForMember(dest => dest.Id, opt => opt.Ignore()); 


            CreateMap<AssetMasterUpdateDto, AssetMasterGenerals>()
            .ForMember(dest => dest.AssetPurchase, opt => opt.MapFrom(src => src.AssetPurchaseDetails))
            .ForMember(dest => dest.AssetLocation, opt => opt.MapFrom(src => src.AssetLocation)) 
            .ForMember(dest => dest.AssetAdditionalCost, opt => opt.MapFrom(src => src.AssetAdditionalCost))     
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive ==1 ? Status.Active : Status.Inactive))                               
            .ForMember(dest => dest.Id, opt => opt.Ignore());      

             CreateMap<AssetLocationUpdateDto, Core.Domain.Entities.AssetMaster.AssetLocation>()
                .ReverseMap(); 
            CreateMap<AssetAdditionalCostUpdateDto, Core.Domain.Entities.AssetPurchase.AssetAdditionalCost>()
                .ReverseMap(); 


            CreateMap<AssetPurchaseUpdateDto, AssetPurchaseDetails>()
                .ForMember(dest => dest.CapitalizationDate,
                    opt => opt.MapFrom(src => src.CapitalizationDate.HasValue 
                        ? src.CapitalizationDate.Value.ToDateTime(TimeOnly.MinValue) 
                        : (DateTime?)null));         
             

            // **Add these mappings to clear your error:**
            CreateMap<AssetPurchaseCombineDto, AssetPurchaseDetails>()
                .ForMember(dest => dest.CapitalizationDate,
                    opt => opt.MapFrom(src => src.CapitalizationDate.HasValue 
                        ? src.CapitalizationDate.Value.ToDateTime(TimeOnly.MinValue) 
                        : (DateTime?)null));
               // .ReverseMap();
    
             CreateMap<AssetLocationCombineDto, Core.Domain.Entities.AssetMaster.AssetLocation>()
                .ReverseMap(); 
            CreateMap<AssetAdditionalCostCombineDto, Core.Domain.Entities.AssetPurchase.AssetAdditionalCost>()
                .ReverseMap(); 
            CreateMap<AssetSpecificationCombineDto, AssetSpecifications>()
                .ReverseMap();
             CreateMap<AssetInsuranceCombineDto, AssetInsurance>()
                .ReverseMap();           
                
        }        
    }
}
