using Core.Application.Common.Mappings;

namespace Core.Application.WorkOrder.Command.UploadFileWorOrder.Item
{
    public class ItemImageDto : IMapFrom<Core.Domain.Entities.WorkOrderMaster.WorkOrderItem>
    {
        public string? WorkOrderItemImage { get; set; }
        public string? WorkOrderImageItemBase64 { get; set; } 
    }
}