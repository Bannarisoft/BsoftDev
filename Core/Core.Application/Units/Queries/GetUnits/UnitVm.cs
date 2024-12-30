using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.Application.Common.Mappings;
using Microsoft.AspNetCore.Http;
using Core.Application.Common;


namespace Core.Application.Units.Queries.GetUnits
{
    public class UnitVm :BaseEntity,IMapFrom<Unit>
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