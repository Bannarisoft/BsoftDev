using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using System.Text;
using Core.Application.Common.HttpResponse;

namespace Core.Application.Companies.Queries.GetCompanies
{
    public class GetCompanyQuery : IRequest<ApiResponseDTO<List<GetCompanyDTO>>>
    {
        
    }
}