using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.PurchaseIndents.Queries.GetPurchaseIndentById
{
    public class IndentDepartmentByIdDto
    {
        public int Id { get; set; }
        public int IndentHeaderId { get; set; }
        public int DepartmentId { get; set; }
    }
}