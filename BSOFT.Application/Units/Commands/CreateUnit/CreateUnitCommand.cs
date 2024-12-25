using BSOFT.Application.Units.Queries.GetUnits;
using BSOFT.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace BSOFT.Application.Units.Commands.CreateUnit
{
    public class CreateUnitCommand : IRequest<int>
    {
         public UnitDto UnitDto { get; set; }

        // public string UnitName { get; set; }
        // public string ShortName { get; set; }
        // public int CompanyId { get; set; }
        // public int DivisionId { get; set; }
        // public string UnitHeadName { get; set; }
        // public string CINNO { get; set; }
        // public byte IsActive { get; set; }
        // public List<UnitAddress> UnitAddress { get; set; } = new List<UnitAddress>();
        // public List<UnitContacts> UnitContacts { get; set; } = new List<UnitContacts>();
    // public UnitAddressDto UnitAddressDto { get; set; }
    // public UnitContactsDto UnitContactsDto { get; set; }
    }

//     public class UnitDto
//     {
//     public string UnitName { get; set; }
//     public string ShortName { get; set; }
//     public int CompanyId { get; set; }
//     public int DivisionId { get; set; }
//     public string UnitHeadName { get; set; }
//     public string CINNO { get; set; }
//     public bool IsActive { get; set; }
//     public ICollection<UnitAddress> UnitAddressDto { get; set; }
//     public ICollection<UnitContacts> UnitContactsDto { get; set; }
// }

// public class UnitAddressDto
// {
//      public int  CountryId { get; set; }

//     public int StateId { get; set; }

//     public int CityId { get; set; }
//     public string AddressLine1 { get; set; }
//     public string? AddressLine2 { get; set; }
//     public string PinCode { get; set; }
//     public string ContactNumber { get; set; }
//     public string? AlternateNumber { get; set; }
// }

// public class UnitContactsDto
// {
//     public string Name { get; set; }
//     public string Designation { get; set; }
//     public string Email { get; set; }
//     public string PhoneNo { get; set; }
//     public string? Remarks { get; set; }
// }

}