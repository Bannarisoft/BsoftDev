using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BSOFT.Application.Common.Mappings;
using BSOFT.Application.Companies.Queries.GetCompanies;
using BSOFT.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BSOFT.Application.Companies.Commands.CreateCompany
{
    public class CreateCompanyCommand : IRequest<int>
    {
        public CompanyDTO Company { get; set; }
        public CompanyAddressDTO CompanyAddresses { get; set; } 
        public CompanyContactDTO CompanyContacts { get; set; } 
        public IFormFile File { get; set; }

    }
}