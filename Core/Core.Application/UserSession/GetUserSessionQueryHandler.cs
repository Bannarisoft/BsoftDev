using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IUserSession;
using Core.Domain.Entities;
using MediatR;

namespace Core.Application.UserSession
{
    public class GetUserSessionQueryHandler : IRequestHandler<GetUserSessionQuery, UserSessions>
    {
        private readonly IUserSessionRepository _userSessionRepository;

        public GetUserSessionQueryHandler(IUserSessionRepository userSessionRepository)
        {
            _userSessionRepository = userSessionRepository;
        }

        public async Task<UserSessions> Handle(GetUserSessionQuery request, CancellationToken cancellationToken)
        {
            return await _userSessionRepository.GetSessionByUserIdAsync(request.UserId);
        }
    }
}