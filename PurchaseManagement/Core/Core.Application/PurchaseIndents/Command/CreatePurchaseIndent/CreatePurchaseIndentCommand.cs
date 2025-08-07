using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Core.Application.PurchaseIndents.Command.CreatePurchaseIndent
{
    public class CreatePurchaseIndentCommand : IRequest<int>
    {
        
        public DateOnly IndentDate { get; set; }
        public int IndentTypeId { get; set; }
        public int UnitId { get; set; }
        public string Purpose { get; set; }
        public ICollection<IndentDetailDto> IndentDetails { get; set; }
        public ICollection<IndentDepartmentDto> IndentDepartments { get; set; }
    }
}