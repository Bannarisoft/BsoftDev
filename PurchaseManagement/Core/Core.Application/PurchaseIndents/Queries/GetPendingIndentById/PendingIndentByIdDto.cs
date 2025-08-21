using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.PurchaseIndents.Queries.GetPendingIndentById
{
    public class PendingIndentByIdDto
    {
        public int Id { get; set; }
        public string IndentNumber { get; set; }
        public DateOnly IndentDate { get; set; }
        public int IndentTypeId { get; set; }
        public int UnitId { get; set; }
        public string Purpose { get; set; }
        public ICollection<PendingIndentDepartmentByIdDto> IndentDepartments { get; set; }
        public ICollection<PendingIndentDetailByIdDto> IndentDetails { get; set; }
    }
}