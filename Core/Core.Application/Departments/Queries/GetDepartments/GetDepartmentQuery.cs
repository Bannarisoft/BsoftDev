using Core.Application.Common.HttpResponse;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Departments.Queries.GetDepartments
{
    public class GetDepartmentQuery : IRequest<ApiResponseDTO<List<DepartmentDto>>>
    {
        
    }
}