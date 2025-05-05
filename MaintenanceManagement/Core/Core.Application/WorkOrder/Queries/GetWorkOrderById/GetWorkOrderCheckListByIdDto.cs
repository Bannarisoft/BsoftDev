
namespace Core.Application.WorkOrder.Queries.GetWorkOrderById
{
    public class GetWorkOrderCheckListByIdDto
    {
        public int CheckListId { get; set; }
        public string? activityChecklist { get; set; }       
        public byte ISCompleted { get; set; }      
        public int activityID { get; set; }    
        public string? activityName { get; set; }
    }
}