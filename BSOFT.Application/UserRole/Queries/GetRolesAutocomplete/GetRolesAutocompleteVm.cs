using BSOFT.Domain.Entities;
using BSOFT.Application.Common.Mappings;

namespace BSOFT.Application.UserRole.Queries.GetRolesAutocomplete
{
    public class GetRolesAutocompleteVm
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
    }
}