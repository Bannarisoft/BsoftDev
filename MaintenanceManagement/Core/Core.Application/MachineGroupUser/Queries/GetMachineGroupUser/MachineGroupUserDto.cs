using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.MachineGroupUser.Queries.GetMachineGroupUser
{
    public class MachineGroupUserDto
    {
        public int Id { get; set; }
        public int  MachineGroupId { get; set; }
        public string? GroupName { get; set; }
        public int  DepartmentId { get; set; }
        public string? DeptName { get; set; }
        public int  UserId { get; set; }
        public string? UserName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}