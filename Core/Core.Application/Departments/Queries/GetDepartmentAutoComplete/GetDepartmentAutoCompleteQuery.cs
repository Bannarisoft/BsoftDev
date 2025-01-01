using Core.Application.Departments.Queries.GetDepartments;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Core.Application.Departments.Queries.GetDepartmentAutoComplete
{
    public class GetDepartmentAutoCompleteQuery : IRequest<List<DepartmentDto>>
    {
         public string SearchPattern { get; set; }
        
    }
}