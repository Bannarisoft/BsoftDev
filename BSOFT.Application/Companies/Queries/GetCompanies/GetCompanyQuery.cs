using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using System.Text;

namespace BSOFT.Application.Companies.Queries.GetCompanies
{
    public class GetCompanyQuery : IRequest<List<CompanyVm>>
    {
        
    }
}