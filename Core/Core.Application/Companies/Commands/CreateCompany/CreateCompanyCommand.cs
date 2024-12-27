using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Mappings;
using Core.Application.Companies.Queries.GetCompanies;
using Core.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Core.Application.Companies.Commands.CreateCompany
{
    public class CreateCompanyCommand : IRequest<int>
    {
        public CompanyDTO Company { get; set; }
        public CompanyAddressDTO CompanyAddresses { get; set; } 
        public CompanyContactDTO CompanyContacts { get; set; } 
        public IFormFile File { get; set; }

    }
}