using Core.Domain.Entities;
using Core.Application.Common.Mappings;

namespace Core.Application.UserRole.Queries.GetRolesAutocomplete
{
    public class GetRolesAutocompleteVm
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
    }
}