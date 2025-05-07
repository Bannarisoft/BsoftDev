namespace Core.Application.WorkOrder.Queries.GetWorkOrderById
{
    public class GetWorkOrderItemByIdDto
    {
        public int StoreTypeId { get; set; }        
        public string? StoreTypeDesc { get; set; }
        public string? ItemCode { get; set; }   
        public string? OldItemCode { get; set; }   
        public string? ItemName { get; set; }        
        public int AvailableQty { get; set; }
        public int UsedQty { get; set; }
        public int? ScarpQty { get; set; }
        public int? ToSubStoreQty { get; set; }
        public string? Image { get; set; }
        public string? ImagePath { get; set; }
     //   public string? ImageBase64 { get; set; }
    }
}