
using AutoMapper;
using Core.Application.WorkOrder.Command.CreateWorkOrder;
using Core.Application.WorkOrder.Command.UpdateWorkOrder;
using Core.Application.WorkOrder.Queries.GetWorkOrderById;
using Core.Domain.Entities.WorkOrderMaster;
using static Core.Domain.Common.BaseEntity;

namespace Core.Application.Common.Mappings
{
    public class WorkOrderProfile : Profile
    {
         public WorkOrderProfile()
        {   
      /*       CreateMap<WorkOrderCombineDto, Core.Domain.Entities.WorkOrderMaster.WorkOrder>()
            .ForMember(dest => dest.WOPreventiveScheduler, opt => opt.MapFrom(src => src.WorkOrderSchedule))
            .ForMember(dest => dest.WorkOrderActivities, opt => opt.MapFrom(src => src.WorkOrderActivity)) 
            .ForMember(dest => dest.WorkOrderCheckLists, opt => opt.MapFrom(src => src.WorkOrderCheckList)) 
            .ForMember(dest => dest.WorkOrderItems, opt => opt.MapFrom(src => src.WorkOrderItem))            
            .ForMember(dest => dest.WorkOrderTechnicians, opt => opt.MapFrom(src => src.WorkOrderTechnician))   
            .ForMember(dest => dest.Id, opt => opt.Ignore()); 


            CreateMap<WorkOrderUpdateDto, Core.Domain.Entities.WorkOrderMaster.WorkOrder>()
            .ForMember(dest => dest.WOPreventiveScheduler, opt => opt.MapFrom(src => src.WorkOrderSchedule))
            .ForMember(dest => dest.WorkOrderActivities, opt => opt.MapFrom(src => src.WorkOrderActivity)) 
            .ForMember(dest => dest.WorkOrderCheckLists, opt => opt.MapFrom(src => src.WorkOrderCheckList)) 
            .ForMember(dest => dest.WorkOrderItems, opt => opt.MapFrom(src => src.WorkOrderItem))            
            .ForMember(dest => dest.WorkOrderTechnicians, opt => opt.MapFrom(src => src.WorkOrderTechnician))                                   
            .ForMember(dest => dest.Id, opt => opt.Ignore());   */    

            CreateMap<WorkOrderScheduleUpdateDto, WorkOrderSchedule>();

            CreateMap<WorkOrderScheduleUpdateDto, WorkOrderSchedule>()
                .ReverseMap(); 
            CreateMap<WorkOrderTechnicianUpdateDto,WorkOrderTechnician>()
                .ReverseMap(); 
            CreateMap<WorkOrderItemUpdateDto, WorkOrderItem>()
                .ReverseMap(); 
            CreateMap<WorkOrderCheckListUpdateDto, WorkOrderCheckList>()
                .ReverseMap(); 
            CreateMap<WorkOrderActivityUpdateDto, WorkOrderActivity>()
                .ReverseMap();  
            CreateMap<WorkOrderUpdateDto, Core.Domain.Entities.WorkOrderMaster.WorkOrder>();



            CreateMap<WorkOrderCombineDto, Core.Domain.Entities.WorkOrderMaster.WorkOrder>();
            CreateMap<WorkOrderScheduleDto,WorkOrderSchedule>()
                .ReverseMap(); 
            CreateMap<WorkOrderTechnicianDto, WorkOrderTechnician>()
                .ReverseMap(); 
            CreateMap<WorkOrderItemDto, WorkOrderItem>()
                .ReverseMap(); 
            CreateMap<WorkOrderCheckListDto,WorkOrderCheckList>()
                .ReverseMap(); 
            CreateMap<WorkOrderActivityDto, WorkOrderActivity>()
                .ReverseMap();              
                
        }     
    }
}