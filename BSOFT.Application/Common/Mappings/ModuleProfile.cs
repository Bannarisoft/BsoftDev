using AutoMapper;
using BSOFT.Domain.Entities;
using BSOFT.Application.Modules.Queries.GetModules;

namespace BSOFT.Application.Common.Mappings
{
    public class ModuleProfile : Profile
    {
    public ModuleProfile()
    {
        CreateMap<BSOFT.Domain.Entities.Modules, ModuleDto>()
            .ForMember(dest => dest.Menus, opt => opt.MapFrom(src => src.Menus.Select(m => m.MenuName).ToList()));
    }
    }
}