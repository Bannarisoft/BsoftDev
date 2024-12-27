using AutoMapper;
using Core.Domain.Entities;
using Core.Application.Modules.Queries.GetModules;

namespace Core.Application.Common.Mappings
{
    public class ModuleProfile : Profile
    {
    public ModuleProfile()
    {
        CreateMap<Core.Domain.Entities.Modules, ModuleDto>()
            .ForMember(dest => dest.Menus, opt => opt.MapFrom(src => src.Menus.Select(m => m.MenuName).ToList()));
    }
    }
}