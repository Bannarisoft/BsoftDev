using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using System.Text;
using Core.Application.Common.HttpResponse;

namespace Core.Application.Divisions.Queries.GetDivisions
{
    public class GetDivisionQuery : IRequest<ApiResponseDTO<List<DivisionDTO>>>
    {
        
    }
}