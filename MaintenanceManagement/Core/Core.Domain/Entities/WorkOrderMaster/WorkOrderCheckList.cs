
namespace Core.Domain.Entities.WorkOrderMaster
{
    public class WorkOrderCheckList
    {
        public int Id { get; set; }
        public int WorkOrderId { get; set; }
        public WorkOrder WOCheckList { get; set; } = null!;         
        public int CheckListId { get; set; }
        public ActivityCheckListMaster CheckListMaster { get; set; } = null!;        
        public string? Description { get; set; }
    }
}