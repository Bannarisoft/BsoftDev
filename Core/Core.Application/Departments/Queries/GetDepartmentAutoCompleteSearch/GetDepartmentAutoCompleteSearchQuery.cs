using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Application.Departments.Queries.GetDepartmentAutoComplete;
using Core.Application.Departments.Queries.GetDepartments;
using Core.Application.Common.HttpResponse;

namespace Core.Application.Departments.Queries.GetDepartmentAutoCompleteSearch
{
    public class GetDepartmentAutoCompleteSearchQuery : IRequest<ApiResponseDTO<List<DepartmentDto>>>
    {
        public string SearchPattern { get; set; } 
    }
}