using AutoMapper;
using Core.Application.DepreciationDetail.Commands.CreateDepreciationDetail;
using Core.Application.DepreciationDetail.Commands.DeleteDepreciationDetail;
using Core.Application.DepreciationDetail.Queries.GetDepreciationDetail;
using Core.Domain.Entities;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings 
{
    public class DepreciationDetailProfile : Profile
    {
        public DepreciationDetailProfile()
        { 
            CreateMap<DeleteDepreciationDetailCommand, DepreciationDetails>()            
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.Deleted));            
            
            CreateMap<CreateDepreciationDetailCommand, DepreciationDetails>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => Status.Active))            
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => IsDelete.NotDeleted)); 

             CreateMap<DepreciationDetails, DepreciationDto>();     
             CreateMap<DepreciationDetails, DepreciationCalculationDto>();     
                    
        }
             
    }
}