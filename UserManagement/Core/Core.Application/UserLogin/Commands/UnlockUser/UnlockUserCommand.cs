using Core.Application.Common.HttpResponse;
using MediatR;

namespace Core.Application.UserLogin.Commands.UnlockUser
{
    public class UnlockUserCommand : IRequest<ApiResponseDTO<bool>>
    {
        public string? userName { get; set; }        
    }
}