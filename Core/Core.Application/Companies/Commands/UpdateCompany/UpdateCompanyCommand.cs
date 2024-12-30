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
        //  public int Id { get; set; }
        // public string CompanyName { get; set; }
        // public string LegalName { get; set; }
        // public string AddressLine1 { get; set; }
        // public string AddressLine2 { get; set; }
        // public string PinCode { get; set; }
        // public int CountryId { get; set; }
        // public int StateId { get; set; }
        // public int CityId { get; set; }
        // public string Phone { get; set; }
        // public string AddressPhone { get; set; }
        // public string ContactName { get; set; }
        // public string Designation { get; set; }
        // public string Remarks { get; set; }
        // public int CompanyId { get; set; }
        // public string AlternatePhone { get; set; }
        // public string Email { get; set; }
        // public string GstNumber { get; set; }
        // public string TIN { get; set; }
        // public string TAN { get; set; }
        // public string CSTNo { get; set; }
        // public int YearOfEstablishment { get; set; }
        // public string Website { get; set; }
        // public string Logo { get; set; }
        // public int EntityId { get; set; }
        // public byte IsActive { get; set; }
        // public int AddressId { get; set; }
        // public int ContactId { get; set; }
        public CompanyDTO Company { get; set; }
        public CompanyAddressDTO CompanyAddresses { get; set; } 
        public CompanyContactDTO CompanyContacts { get; set; } 
        public IFormFile File { get; set; }
    }
}