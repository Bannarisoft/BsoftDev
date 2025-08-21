using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.PurchaseIndents.Queries.GetPendingIndentById
{
    public class PendingIndentDepartmentByIdDto
    {
        public int Id { get; set; }
        public int IndentHeaderId { get; set; }
        public int DepartmentId { get; set; }
    }
}