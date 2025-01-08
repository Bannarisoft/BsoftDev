using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Interfaces.IUserSession;
using Core.Application.UserSession.Queries.GetUserSession;
using MediatR;

namespace Core.Application.UserSession.Command
{
    public class CreateUserSessionCommandHandler : IRequestHandler<CreateUserSessionCommand, UserSessionDto>
    {
        private readonly IUserSessionCommandRepository _IUserSessionRepository;
        private readonly IMapper _Imapper;
        private readonly IMediator _Imediator;
        public CreateUserSessionCommandHandler(IUserSessionCommandRepository iUserSessionRepository, IMapper Imapper,IMediator Imediator)
        {
            _IUserSessionRepository = iUserSessionRepository;
            _Imapper = Imapper;
            _Imediator=Imediator;
        }

        public async Task<UserSessionDto> Handle(CreateUserSessionCommand request, CancellationToken cancellationToken)
        {
            var usersession = _Imapper.Map<Core.Domain.Entities.UserSession>(request);
            await _IUserSessionRepository.CreateAsync(usersession);
            var usersessionDto = _Imapper.Map<UserSessionDto>(usersession);
            return usersessionDto;
        }
        
    }

}
