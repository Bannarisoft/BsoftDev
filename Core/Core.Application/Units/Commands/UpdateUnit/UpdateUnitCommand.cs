using Core.Application.Units.Queries.GetUnits;
using Core.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Units.Commands.UpdateUnit
{
    public class UpdateUnitCommand : IRequest<int>
    {
    
    public int UnitId  { get; set; }
    public UnitDto UpdateUnitDto { get; set; }
    
    }

    // public class UpdateUnitDto
    // {
  
    // public string UnitName { get; set; }
    // public string ShortName { get; set; }
    // public int CompanyId { get; set; }
    // public int DivisionId { get; set; }
    // public string UnitHeadName { get; set; }
    // public string CINNO { get; set; }
    // public bool IsActive { get; set; }
    // }

// public class UpdateUnitAddressDto
// {
//     public int  CountryId { get; set; }
//     public int StateId { get; set; }
//     public int CityId { get; set; }
//     public string AddressLine1 { get; set; }
//     public string? AddressLine2 { get; set; }
//     public string PinCode { get; set; }
//     public string ContactNumber { get; set; }
//     public string? AlternateNumber { get; set; }
// }

// public class UpdateUnitContactsDto
// {
   
//     public string Name { get; set; }
//     public string Designation { get; set; }
//     public string Email { get; set; }
//     public string PhoneNo { get; set; }
//     public string? Remarks { get; set; }
// }


}