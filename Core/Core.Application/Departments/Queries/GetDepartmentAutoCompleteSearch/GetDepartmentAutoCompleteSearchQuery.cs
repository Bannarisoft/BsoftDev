using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Application.Departments.Queries.GetDepartmentAutoComplete;

namespace Core.Application.Departments.Queries.GetDepartmentAutoCompleteSearch
{
    public class GetDepartmentAutoCompleteSearchQuery : IRequest<List<DepartmentAutoCompleteVm>>
    {
        public string SearchDept { get; set; } 
    }
}