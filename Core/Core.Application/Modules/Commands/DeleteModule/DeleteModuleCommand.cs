using MediatR;

namespace BSOFT.Application.Modules.Commands.DeleteModule
{
    public class DeleteModuleCommand : IRequest
    {
        public int ModuleId { get; set; }
    }
}