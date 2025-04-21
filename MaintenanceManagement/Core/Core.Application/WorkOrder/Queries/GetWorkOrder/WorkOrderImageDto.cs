
using Core.Application.Common.Mappings;

namespace Core.Application.WorkOrder.Queries.GetWorkOrder
{
    public class WorkOrderImageDto : IMapFrom<Core.Domain.Entities.WorkOrderMaster.WorkOrder>
    {
        public string? WorkOrderImage { get; set; }
        public string? WorkOrderImageBase64 { get; set; } 
        public string? WorkOrderItemImage { get; set; }
        public string? WorkOrderImageItemBase64 { get; set; } 
    }
}