
using Core.Application.Common.Mappings;

namespace Core.Application.WorkOrder.Queries.GetWorkOrder
{
    public class WorkOrderImageDto : IMapFrom<Core.Domain.Entities.WorkOrderMaster.WorkOrder>
    {
        public string? AssetImage { get; set; }
        public string? AssetImageBase64 { get; set; } 

    }
}