using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using BSOFT.Domain.Common;


namespace BSOFT.Domain.Entities
{
    [Table("Company", Schema = "AppData")]
    public class Company : BaseEntity
    {
        [Key]
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
        public string? Logo { get; set; }
        public int EntityId { get; set; }
        public byte IsActive { get; set; }
    }
}