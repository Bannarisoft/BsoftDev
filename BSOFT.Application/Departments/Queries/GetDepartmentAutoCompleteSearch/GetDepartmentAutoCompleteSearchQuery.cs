using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSOFT.Application.Departments.Queries.GetDepartmentAutoComplete;

namespace BSOFT.Application.Departments.Queries.GetDepartmentAutoCompleteSearch
{
    public class GetDepartmentAutoCompleteSearchQuery : IRequest<List<DepartmentAutoCompleteVm>>
    {
        public string SearchDept { get; set; } 
    }
}