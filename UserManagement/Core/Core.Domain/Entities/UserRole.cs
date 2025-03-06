using Core.Domain.Common;

namespace Core.Domain.Entities
{
     
    public class UserRole  : BaseEntity
    {        
        public int Id { get; set; }
        public string? RoleName { get; set; }
        public string? Description { get; set; }
        public int CompanyId { get; set; }          
        public IList<UserRoleAllocation> UserRoleAllocations { get; set; }
        
        public IList<RoleModule> roleModules { get; set; }
        public IList<RoleParent> roleParents { get; set; }
        public IList<RoleChild> roleChildren { get; set; }
        public IList<RoleMenuPrivileges> roleMenuPrivileges { get; set; }



    }
}
