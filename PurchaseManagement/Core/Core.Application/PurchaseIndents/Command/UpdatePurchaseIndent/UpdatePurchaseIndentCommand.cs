using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Core.Application.PurchaseIndents.Command.UpdatePurchaseIndent
{
    public class UpdatePurchaseIndentCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public DateOnly IndentDate { get; set; }
        public int IndentTypeId { get; set; }
        public int UnitId { get; set; }
        public string Purpose { get; set; }
        public byte IsActive { get; set; }
        public ICollection<IndentDetailUpdateDto> IndentDetails { get; set; }
        public ICollection<IndentDepartmentUpdateDto> IndentDepartments { get; set; }
    }
}