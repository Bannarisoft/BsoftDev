
using AutoMapper;
using Core.Application.WorkOrder.Command.CreateWorkOrder;
using Core.Application.WorkOrder.Command.UpdateWorkOrder;
using Core.Application.WorkOrder.Queries.GetWorkOderDropdown;
using Core.Application.WorkOrder.Queries.GetWorkOrder;
using Core.Domain.Entities.WorkOrderMaster;

namespace Core.Application.Common.Mappings
{
    public class WorkOrderProfile : Profile
    {
         public WorkOrderProfile()
        {   
            CreateMap<WorkOrderUpdateDto,Core.Domain.Entities.WorkOrderMaster.WorkOrder>()
                .ForMember(dest => dest.WorkOrderActivities, opt => opt.MapFrom(src => src.WorkOrderActivity))
                .ForMember(dest => dest.WorkOrderCheckLists, opt => opt.MapFrom(src => src.WorkOrderCheckList))
                .ForMember(dest => dest.WorkOrderItems, opt => opt.MapFrom(src => src.WorkOrderItem))
                .ForMember(dest => dest.WorkOrderTechnicians, opt => opt.MapFrom(src => src.WorkOrderTechnician))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)); // optional if you want to keep the ID
            
                CreateMap<WorkOrderCombineDto, Core.Domain.Entities.WorkOrderMaster.WorkOrder>()
                    .ForMember(dest => dest.WorkOrderActivities, opt => opt.MapFrom(src => src.WorkOrderActivity))
                    .ForMember(dest => dest.WorkOrderCheckLists, opt => opt.MapFrom(src => src.WorkOrderCheckList))
                    .ForMember(dest => dest.WorkOrderItems, opt => opt.MapFrom(src => src.WorkOrderItem))    
                    .ForMember(dest => dest.Id, opt => opt.Ignore()); // Prevent mapping to existing ID

                        // ✅ Create to domain mappings
            CreateMap<WorkOrderScheduleUpdateDto, WorkOrderSchedule>();
            CreateMap<WorkOrderActivityUpdateDto, WorkOrderActivity>().ReverseMap();
            CreateMap<WorkOrderItemUpdateDto, WorkOrderItem>().ReverseMap();
            CreateMap<WorkOrderTechnicianUpdateDto, WorkOrderTechnician>().ReverseMap();
            CreateMap<WorkOrderCheckListUpdateDto, WorkOrderCheckList>().ReverseMap();
                        
            CreateMap<Core.Domain.Entities.WorkOrderMaster.WorkOrder, GetWorkOderDropdownDto>();

            CreateMap<WorkOrderItemDto, WorkOrderItem>().ReverseMap();
            CreateMap<WorkOrderCheckListDto, WorkOrderCheckList>().ReverseMap();
            CreateMap<WorkOrderActivityDto, WorkOrderActivity>().ReverseMap();
            CreateMap<GetWorkOderDropdownDto, Core.Domain.Entities.WorkOrderMaster.WorkOrder>();
         
              // Use AfterMap to group schedules
            CreateMap<List<WorkOrderWithScheduleDto>, List<GetWorkOrderDto>>()
                .ConvertUsing(src => MapGroupedWorkOrders(src));
        
        }
        private List<GetWorkOrderDto> MapGroupedWorkOrders(List<WorkOrderWithScheduleDto> source)
        {
            return source
                .GroupBy(x => new
                {
                    x.Id,
                    x.WorkOrderDocNo,
                    x.Department,
                    x.DepartmentId,
                    x.Machine,
                    x.RequestDate,
                    x.RequestType,
                    x.Status,
                    x.MaintenanceType,
                    x.RequestId,
                    x.MachineName,
                    x.ScheduleStatus,
                    x.DueDate,
                    x.ActivityName
                })
                .Select(g => new GetWorkOrderDto
                {
                    Id = g.Key.Id,
                    WorkOrderDocNo = g.Key.WorkOrderDocNo,
                    Department = g.Key.Department,
                    DepartmentId = g.Key.DepartmentId,
                    Machine = g.Key.Machine,
                    RequestDate = g.Key.RequestDate,
                    RequestType = g.Key.RequestType,
                    Status = g.Key.Status,
                    MaintenanceType = g.Key.MaintenanceType,
                    RequestId = g.Key.RequestId,
                    MachineName = g.Key.MachineName,
                    ScheduleStatus = g.Key.ScheduleStatus,
                    DueDate = g.Key.DueDate,
                    ActivityName = g.Key.ActivityName,
                    Schedules = g
                        .Where(s => s.ScheduleStartTime.HasValue || s.ScheduleEndTime.HasValue)
                        .Select(s => new ScheduleDto
                        {
                            Start = s.ScheduleStartTime,
                            End = s.ScheduleEndTime,
                            IsCompleted=(byte)(s.IsCompleted ?? 0)
                        })
                        .ToList()
                })
                .ToList();
        }     
    }
}