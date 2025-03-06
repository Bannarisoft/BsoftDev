using AutoMapper;
using Core.Application.AssetLocation.Commands.CreateAssetLocation;
using Core.Application.AssetLocation.Queries.GetAssetLocation;
using Core.Application.AssetMaster.AssetLocation.Commands.UpdateAssetLocation;
using Core.Application.AssetMaster.AssetLocation.Queries.GetCustodian;

namespace Core.Application.Common.Mappings
{
    public class AssetLocationProfile : Profile
    {

        public AssetLocationProfile()
        {
            CreateMap<Core.Domain.Entities.AssetMaster.AssetLocation,AssetLocationDto>();

            CreateMap<CreateAssetLocationCommand, Core.Domain.Entities.AssetMaster.AssetLocation>();
            CreateMap<UpdateAssetLocationCommand, Core.Domain.Entities.AssetMaster.AssetLocation>() 
            .ForMember(dest => dest.AssetId, opt => opt.Ignore())  
            .ForMember(dest => dest.UnitId, opt => opt.MapFrom(src => src.UnitId))
            .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.DepartmentId))                
            .ForMember(dest => dest.LocationId, opt => opt.MapFrom(src => src.LocationId))
            .ForMember(dest => dest.SubLocationId, opt => opt.MapFrom(src => src.SubLocationId))
            .ForMember(dest => dest.CustodianId, opt => opt.MapFrom(src => src.CustodianId))
            .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.DepartmentId))
            .ForMember(dest => dest.UnitId, opt => opt.MapFrom(src => src.UnitId));
            
            CreateMap<Core.Domain.Entities.AssetMaster.Employee,GetCustodianDto>()           
            .ForMember(dest => dest.CustodianId, opt => opt.MapFrom(src => src.Empcode))
            .ForMember(dest => dest.CustodianName, opt => opt.MapFrom(src => src.Empname));

           
                
        }
        
    }
}