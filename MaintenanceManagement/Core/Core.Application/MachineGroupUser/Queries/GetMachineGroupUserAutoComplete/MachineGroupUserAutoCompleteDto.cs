
namespace Core.Application.MachineGroupUser.Queries.GetMachineGroupUserAutoComplete
{
    public class MachineGroupUserAutoCompleteDto
    {
        public int Id { get; set; }
        public string? GroupName { get; set; }
        public string? DeptName { get; set; }
        public string? UserName { get; set; }
    }
}