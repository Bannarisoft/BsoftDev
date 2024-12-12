using BSOFT.Domain.Entities;
using BSOFT.Application.Common.Mappings;

namespace BSOFT.Application.Role.Queries.GetRolesAutocomplete
{
    public class GetRolesAutocompleteVm
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }
}