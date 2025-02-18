
using AutoMapper;
using Core.Application.AssetMaster.AssetMasterGeneral.Commands.CreateAssetMasterGeneral;
using Core.Application.AssetMaster.AssetMasterGeneral.Commands.DeleteAssetMasterGeneral;
using Core.Application.AssetMaster.AssetMasterGeneral.Commands.UpdateAssetMasterGeneral;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Domain.Entities;
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
             
            CreateMap<AssetMasterGenerals,AssetMasterGeneralAutoCompleteDTO>();    
            CreateMap<AssetMasterGenerals, AssetMasterGeneralDTO>();             
        }        
    }
}
