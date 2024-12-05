using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using BSOFT.Domain.Entities;
using BSOFT.Application.Common.Mappings;

namespace BSOFT.Application.Companies.Queries.GetCompanies
{
    public class CompanyVm : IMapFrom<Company>
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string LegalName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string GstNumber { get; set; }
        public string TIN { get; set; }
        public string TAN { get; set; }
        public string CSTNo { get; set; }
        public string YearofEstablishment { get; set; }
        public string Website { get; set; }
        public string? Logo { get; set; }
        public int Entity { get; set; }
        public byte Status { get; set; }
        public int CreatedBy { get; set; }
        public DateTime Created_Time { get; set; }
        public string? CreatedByName { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime Modified_Time { get; set; }
        public string ModifiedByName { get; set; }
    }
}