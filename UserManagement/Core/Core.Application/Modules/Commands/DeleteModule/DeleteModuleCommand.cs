using Core.Application.Common.HttpResponse;
using MediatR;

namespace BSOFT.Application.Modules.Commands.DeleteModule
{
    public class DeleteModuleCommand : IRequest<ApiResponseDTO<bool>>
    {
        public int ModuleId { get; set; }
    }
}