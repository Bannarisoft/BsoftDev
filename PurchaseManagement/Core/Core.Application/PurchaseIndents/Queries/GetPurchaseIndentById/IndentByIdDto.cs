using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.PurchaseIndents.Queries.GetPurchaseIndentById
{
    public class IndentByIdDto
    {
        public int Id { get; set; }
        public string IndentNumber { get; set; }
        public DateOnly IndentDate { get; set; }
        public int IndentTypeId { get; set; }
        public int UnitId { get; set; }
        public string Purpose { get; set; }
        public ICollection<IndentDepartmentByIdDto> IndentDepartments { get; set; }
        public ICollection<IndentDetailByIdDto> IndentDetails { get; set; }
    }
}