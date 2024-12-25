using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BSOFT.Application.Units.Commands.CreateUnit;
using BSOFT.Application.Units.Commands.DeleteUnit;
using BSOFT.Application.Units.Queries.GetUnits;
using BSOFT.Domain.Entities;

namespace BSOFT.Application.Common.Mappings
{
    public class CreateUnitProfile : Profile
    {
    public CreateUnitProfile()
    {
        CreateMap<CreateUnitCommand, Unit>();
        CreateMap<UnitAddressDto, UnitAddress>();
        CreateMap<UnitContactsDto, UnitContacts>();
    }
    }
}