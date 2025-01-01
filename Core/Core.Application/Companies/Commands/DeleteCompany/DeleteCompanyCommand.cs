using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using System.Reflection;
using System.Text;
using Core.Application.Companies.Queries.GetCompanies;

namespace Core.Application.Companies.Commands.DeleteCompany
{
    public class DeleteCompanyCommand : IRequest<int>
    {
        public int Id { get; set; }
        public CompanyDeleteDTO CompanyDelete { get; set; }
    }
}