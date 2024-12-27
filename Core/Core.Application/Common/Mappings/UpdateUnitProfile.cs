using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Units.Commands.DeleteUnit;
using Core.Application.Units.Commands.UpdateUnit;
using Core.Application.Units.Queries.GetUnits;
using Core.Domain.Entities;
namespace Core.Application.Common.Mappings
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