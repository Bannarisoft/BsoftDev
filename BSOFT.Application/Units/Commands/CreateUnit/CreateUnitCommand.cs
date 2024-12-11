using BSOFT.Application.Units.Queries.GetUnits;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace BSOFT.Application.Units.Commands.CreateUnit
{
    public class CreateUnitCommand : IRequest<UnitVm>
    {
    public int UnitId { get; set; }
    public string Name { get; set; }
    public string ShortName { get; set; }
    public string Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? Address3 { get; set; }
    public int CoId { get; set; }
    public int DivId { get; set; }
    public string UnitHeadName { get; set; }
    public string Mobile { get; set; }
    public string Email { get; set; }
    public byte IsActive { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedByName { get; set; }
    public string CreatedIP { get; set; }
    public int ModifiedBy { get; set; } 
    public string ModifiedByName { get; set; }    
    public DateTime ModifiedAt { get; set; }
    public string ModifiedIP { get; set; }
    }
}