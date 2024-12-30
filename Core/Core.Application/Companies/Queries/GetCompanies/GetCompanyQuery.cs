using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using System.Text;

namespace Core.Application.Companies.Queries.GetCompanies
{
    public class GetCompanyQuery : IRequest<List<CompanyDTO>>
    {
        
    }
}