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

         public CompanyAddressDTO CompanyAddress { get; set; }
        public CompanyContactDTO CompanyContact { get; set; }
    }
}