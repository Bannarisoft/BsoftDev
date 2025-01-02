using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using System.Text;
using Core.Application.Companies.Queries.GetCompanies;

namespace Core.Application.Companies.Queries.GetCompanyById
{
    public class GetCompanyByIdQuery : IRequest<GetCompanyDTO>
    {
        public int CompanyId { get; set; }
    }
}