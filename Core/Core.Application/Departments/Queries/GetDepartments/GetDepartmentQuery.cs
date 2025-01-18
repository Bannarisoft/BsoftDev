using Core.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Departments.Queries.GetDepartments
{
    public class GetDepartmentQuery : IRequest<Result<List<DepartmentDto>> >
    {
        
    }
}