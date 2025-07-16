using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.MRS.Queries.GetSubDepartment
{
    public class GetSubDepartmentQuery : IRequest<List<MSubDepartment>>
    {
        public string? OldUnitcode { get; set; }   
    }
}