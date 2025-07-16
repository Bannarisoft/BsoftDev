using AutoMapper;
using Core.Application.DepreciationDetail.Commands.CreateDepreciationDetail;
using Core.Application.DepreciationDetail.Commands.DeleteDepreciationDetail;
using Core.Application.DepreciationDetail.Commands.UpdateDepreciationDetail;
using Core.Application.DepreciationDetail.Queries.GetDepreciationDetail;
using Core.Domain.Entities;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings 
{
    public class DepreciationDetailProfile : Profile
    {
        public DepreciationDetailProfile()
        { 
            CreateMap<DeleteDepreciationDetailCommand, DepreciationDetails>();                                  
            CreateMap<CreateDepreciationDetailCommand, DepreciationDetails>(); 
            CreateMap<UpdateDepreciationDetailCommand,DepreciationDetails>();               
            CreateMap<DepreciationDetails, DepreciationDto>();     
            CreateMap<DepreciationDetails, DepreciationCalculationDto>();   
            CreateMap<DepreciationDetails, DepreciationAbstractDto>();                      
        }
             
    }
}