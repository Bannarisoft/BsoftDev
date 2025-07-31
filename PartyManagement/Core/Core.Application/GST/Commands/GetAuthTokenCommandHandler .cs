using Core.Application.GST.Commands;
using Core.Application.GST.DTOs;
using Core.Application.Interfaces.GST;
using MediatR;

namespace Core.Application.GST.Commands
{
   public class GetAuthTokenCommandHandler  : IRequestHandler<GetAuthTokenCommand, GSTAuthResponseDto>
    {
        private readonly IGSTAuthService _gstAuthService;
        public GetAuthTokenCommandHandler (IGSTAuthService gstAuthService)
        {
            _gstAuthService = gstAuthService;
        }

        public async Task<GSTAuthResponseDto> Handle(GetAuthTokenCommand request, CancellationToken cancellationToken)
        {
            return await _gstAuthService.GetAuthTokenAsync();
        }
    }
}
