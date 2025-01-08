using Core.Application.UserRoleAllocation.Queries.GetUserRoleAllocation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.UserRoleAllocation.Queries.GetUserRoleAllocationAutocomplete
{
    public class GetUserRoleAllocationAutocompleteQuery : IRequest<List<CreateUserRoleAllocationDto>>
    {
        public string SearchPattern { get; set; }
        public GetUserRoleAllocationAutocompleteQuery(string searchPattern)
        {
            SearchPattern = searchPattern;
        }
    }
}