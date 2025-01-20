using Core.Application.Common.HttpResponse;
using Core.Application.Departments.Queries.GetDepartments;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Core.Application.Departments.Queries.GetDepartmentAutoComplete
{
    public class GetDepartmentAutoCompleteQuery : IRequest<ApiResponseDTO<List<DepartmentDto>>>
    {
         public string SearchPattern { get; set; }
        
    }
}