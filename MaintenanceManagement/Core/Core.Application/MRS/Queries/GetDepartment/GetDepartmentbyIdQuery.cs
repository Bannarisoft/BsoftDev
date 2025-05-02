using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.MRS.Queries.GetDepartment;
using MediatR;

namespace Core.Application.MRS.Queries
{
     public class GetDepartmentbyIdQuery : IRequest<ApiResponseDTO<List<MDepartmentDto>>>
    {
        public string? OldUnitcode { get; set; }
    }
}