using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IUserSession;
using MediatR;

namespace Core.Application.UserLogin.Commands.DeactivateUserSession
{
    public class DeactivateUserSessionCommandHandler : IRequestHandler<DeactivateUserSessionCommand, ApiResponseDTO<bool>>
    {
        private readonly IUserSessionRepository _userSessionRepository;
        public DeactivateUserSessionCommandHandler(IUserSessionRepository userSessionRepository)
        {
            _userSessionRepository = userSessionRepository;
        }
        public async Task<ApiResponseDTO<bool>> Handle(DeactivateUserSessionCommand request, CancellationToken cancellationToken)
        {
            var session = await _userSessionRepository.DeactivateUserSessionsByUsername(request.Username);
            if (session)
            {
                return new ApiResponseDTO<bool> { IsSuccess = true, Message = "Session Deactivated Successfully." }; ;
            }

                return new ApiResponseDTO<bool> { IsSuccess = false, Message = "Session Deactivation Failed." }; 
            
        }
    }
}