
using Core.Application.Common.HttpResponse;
using Core.Application.Departments.Queries.GetDepartments;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Core.Application.Departments.Queries.GetDepartmentById
{
    public class GetDepartmentByIdQuery :IRequest<ApiResponseDTO<DepartmentDto>>
    {
        
        public int DepartmentId { get; set; }
        
    }
}