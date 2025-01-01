using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Companies.Queries.GetCompanies;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Core.Application.Companies.Commands.UpdateCompany
{
    public class UpdateCompanyCommand : IRequest<int>
    {
        public CompanyDTO Company { get; set; }
        public CompanyAddressDTO CompanyAddresses { get; set; } 
        public CompanyContactDTO CompanyContacts { get; set; } 
        public IFormFile File { get; set; }
    }
}