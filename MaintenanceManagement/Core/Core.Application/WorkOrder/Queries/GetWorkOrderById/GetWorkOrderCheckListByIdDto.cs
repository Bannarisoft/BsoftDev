
namespace Core.Application.WorkOrder.Queries.GetWorkOrderById
{
    public class GetWorkOrderCheckListByIdDto
    {
        public int CheckListId { get; set; }
        public string? ActivityCheckList { get; set; }       
        public byte IsCompleted { get; set; }      
        public int ActivityID { get; set; }    
        public string? ActivityName { get; set; }
        public string? Description { get; set; }
        public int Id { get; set; }
    }
}