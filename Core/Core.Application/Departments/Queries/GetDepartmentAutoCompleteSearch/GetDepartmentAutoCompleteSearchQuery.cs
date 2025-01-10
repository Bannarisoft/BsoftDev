using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Application.Departments.Queries.GetDepartments;

namespace Core.Application.Departments.Queries.GetDepartmentAutoCompleteSearch
{
    public class GetDepartmentAutoCompleteSearchQuery : IRequest<List<DepartmentDto>>
    {
        public string SearchPattern { get; set; } 
    }
}