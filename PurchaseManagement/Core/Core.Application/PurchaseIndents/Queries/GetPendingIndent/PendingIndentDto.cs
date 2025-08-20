using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.PurchaseIndents.Queries.GetPendingIndent
{
    public class PendingIndentDto
    {
        public int Id { get; set; }
        public string IndentNumber { get; set; }
        public DateOnly IndentDate { get; set; }
        public int IndentTypeId { get; set; }
        public string IndentType { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public string Purpose { get; set; }
        public string Status { get; set; }
        public byte IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string CreatedByName { get; set; }
        public int ModifiedBy { get; set; }
        public DateTimeOffset ModifiedDate { get; set; }
        public string ModifiedByName { get; set; }
    }
}