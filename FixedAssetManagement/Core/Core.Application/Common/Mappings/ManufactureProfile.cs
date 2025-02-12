using AutoMapper;

namespace Core.Application.Common.Mappings
{
    public class ManufactureProfile : Profile
    {
        public DepreciationGroupProfile()
        { 
            CreateMap<DeleteDepreciationGroupCommand, DepreciationGroups>()            
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted));            
            
            CreateMap<CreateDepreciationGroupCommand, DepreciationGroups>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))            
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted)); 

            CreateMap<UpdateDepreciationGroupCommand, DepreciationGroups>()
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => IsDelete.NotDeleted));     
             
            CreateMap<DepreciationGroups, DepreciationGroupAutoCompleteDTO>();    
            CreateMap<DepreciationGroups, DepreciationGroupDTO>();             
        }
    }
}