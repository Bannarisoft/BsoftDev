using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.UserRole.Queries.GetRolesAutocomplete
{
    public class GetRolesAutocompleteQuery : IRequest<List<GetRolesAutocompleteVm>>
    {
        public string SearchTerm { get; set; }
    }
}