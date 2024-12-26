using MediatR;

namespace BSOFT.Application.Modules.Queries.GetModules
{
    public class GetModulesQuery: IRequest<List<ModuleDto>>
    {
        
    }

    public class ModuleDto
    {
        public string ModuleName { get; set; }
        public string IsDeleted { get; set; }

        public List<string> Menus { get; set; }
    }
}