using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BSOFT.Application.Units.Commands.DeleteUnit;
using BSOFT.Application.Units.Commands.UpdateUnit;
using BSOFT.Application.Units.Queries.GetUnits;
using BSOFT.Domain.Entities;
namespace BSOFT.Application.Common.Mappings
{
    public class UpdateUnitProfile : Profile
    {
    public UpdateUnitProfile()
    {
         CreateMap<UpdateUnitCommand, Unit>();
         CreateMap<DeleteUnitCommand, Unit>();
         
        // CreateMap<UnitAddressDto, UnitAddress>();
        // CreateMap<UnitContactsDto, UnitContacts>();
    }
     
    }
}