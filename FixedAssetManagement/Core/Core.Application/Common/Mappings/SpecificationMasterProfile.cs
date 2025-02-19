using AutoMapper;
using Core.Application.SpecificationMaster.Commands.CreateSpecificationMaster;
using Core.Application.SpecificationMaster.Commands.DeleteSpecificationMaster;
using Core.Application.SpecificationMaster.Commands.UpdateSpecificationMaster;
using Core.Application.SpecificationMaster.Queries.GetSpecificationMaster;
using Core.Domain.Entities;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings
{
    public class SpecificationMasterProfile : Profile
    {
        public SpecificationMasterProfile()
        { 
            CreateMap<DeleteSpecificationMasterCommand, SpecificationMasters>()            
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted));            
            
            CreateMap<CreateSpecificationMasterCommand, SpecificationMasters>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))            
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted));             

            CreateMap<UpdateSpecificationMasterCommand, SpecificationMasters>()
            .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => IsDelete.NotDeleted));     
             
            CreateMap<SpecificationMasters, SpecificationMasterAutoCompleteDTO>();    
            CreateMap<SpecificationMasters, SpecificationMasterDTO>();             
        }
    }
}