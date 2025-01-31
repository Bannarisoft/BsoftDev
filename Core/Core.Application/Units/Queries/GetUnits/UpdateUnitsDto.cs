using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.Units.Queries.GetUnits
{
    public class UpdateUnitsDto
    {
    public int Id { get; set; }
    public string UnitName { get; set; }
    public string ShortName { get; set; }
    public int CompanyId { get; set; }
    public int DivisionId { get; set; }
    public string UnitHeadName { get; set; }
    public string CINNO { get; set; }
    public byte IsActive { get; set; }
    public UnitAddressDto UnitAddressDto { get; set; } 
    public UnitContactsDto UnitContactsDto { get; set;} 
    }
}