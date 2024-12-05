using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using System.Text;
using BSOFT.Application.Companies.Queries.GetCompanies;

namespace BSOFT.Application.Companies.Queries.GetCompanyById
{
    public class GetCompanyByIdQuery : IRequest<CompanyVm>
    {
        public int CompanyId { get; set; }
    }
}