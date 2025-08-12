using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.PurchaseIndents.Command.UpdatePurchaseIndent
{
    public class IndentDepartmentUpdateDto
    {
        public int IndentHeaderId { get; set; }
        public int DepartmentId { get; set; }
    }
}