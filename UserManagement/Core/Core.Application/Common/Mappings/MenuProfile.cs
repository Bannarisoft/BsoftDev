using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Menu.Queries.GetChildMenuByModule;
using Core.Application.Menu.Queries.GetMenuByModule;

namespace Core.Application.Common.Mappings
{
    public class MenuProfile : Profile
    {
        public MenuProfile()
        {
            CreateMap<Core.Domain.Entities.Menu, MenuDTO>();
            CreateMap<Core.Domain.Entities.Menu, ChildMenuDTO>();
        }
    }
}