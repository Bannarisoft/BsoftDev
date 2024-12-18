using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BSOFT.Application.Units.Commands.UpdateUnit
{
    public class UpdateUnitCommand : IRequest<int>
    {
    public int UnitId { get; set; }
    public string UnitName { get; set; }
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

    }
}