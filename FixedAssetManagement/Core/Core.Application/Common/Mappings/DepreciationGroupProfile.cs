using AutoMapper;
using Core.Application.DepreciationGroup.Commands.CreateDepreciationGroup;
using Core.Application.DepreciationGroup.Commands.DeleteDepreciationGroup;
using Core.Application.DepreciationGroup.Commands.UpdateDepreciationGroup;
using Core.Application.DepreciationGroup.Queries.GetDepreciationGroup;
using Core.Domain.Entities;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings
{
    public class DepreciationGroupProfile : Profile
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