namespace Core.Application.WorkOrderMaster.WorkOrder.Queries.GetWorkOrderById
{
    public class GetWorkOrderItemByIdDto
    {
        public int? DeptId { get; set; }
        public string? DeptName { get; set; }
        public string? ItemId { get; set; }
        public string? ItemName { get; set; }
        public int AvailableQty { get; set; }
        public int UsedQty { get; set; }
        public string? Image { get; set; }
    }
}