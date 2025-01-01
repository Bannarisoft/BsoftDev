using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Companies.Queries.GetCompanies
{
    public class GetCompanyDTO
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string LegalName { get; set; }
        public string GstNumber { get; set; }
        public string TIN { get; set; }
        public string TAN { get; set; }
        public string CSTNo { get; set; }
        public int YearOfEstablishment { get; set; }
        public string Website { get; set; }
        public string Logo { get; set; }
        public int EntityId { get; set; }
        public byte IsActive { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string PinCode { get; set; }
        public int CountryId { get; set; }
        public int StateId { get; set; }
        public int CityId { get; set; }
        public string AddressPhone { get; set; }
        public string AlternatePhone { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Email { get; set; }
        public string ContactPhone { get; set; }
        public string Remarks { get; set; }
    }
}