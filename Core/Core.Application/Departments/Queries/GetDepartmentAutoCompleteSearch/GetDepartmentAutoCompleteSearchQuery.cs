using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Application.Departments.Queries.GetDepartments;
using Core.Application.Common;

namespace Core.Application.Departments.Queries.GetDepartmentAutoCompleteSearch
{
    public class GetDepartmentAutoCompleteSearchQuery : IRequest<Result<List<DepartmentDto>>>
    {
        public string SearchPattern { get; set; } 
    }
}