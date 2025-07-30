using Core.Application.GST.DTOs;
using MediatR;

namespace Core.Application.GST.Commands
{
   public record GetAuthTokenCommand : IRequest<GSTAuthResponseDto>;
}