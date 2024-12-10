namespace BSOFT.Application.RoleEntitlement.Queries.GetUsers
{
    public class RoleEntitlementDto
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public List<MenuPermissionDto> MenuPermissions { get; set; } = new List<MenuPermissionDto>();
    }
}