using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.Modules.Commands.DeleteModule
{
    public class DeleteModuleCommand : IRequest<ApiResponseDTO<bool>>
    {
        public int ModuleId { get; set; }
    }
}