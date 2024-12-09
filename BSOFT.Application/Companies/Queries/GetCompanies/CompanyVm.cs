using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using BSOFT.Domain.Entities;
using BSOFT.Application.Common.Mappings;
using Microsoft.AspNetCore.Http;

namespace BSOFT.Application.Companies.Queries.GetCompanies
{
    public class CompanyVm : IMapFrom<Company>
    {
        public int CoId { get; set; }
        public string CompanyName { get; set; }
        public string LegalName { get; set; }
        public string Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Address3 { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string GstNumber { get; set; }
        public string? TIN { get; set; }
        public string? TAN { get; set; }
        public string? CSTNo { get; set; }
        public int YearofEstablishment { get; set; }
        public string Website { get; set; }
        public string Logo { get; set; }
        public int EntityId { get; set; }
        public byte IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedByName { get; set; }
        public string CreatedIP { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? ModifiedByName { get; set; }
        public string? ModifiedIP { get; set; }
        public IFormFile File { get; set; }
    }
}