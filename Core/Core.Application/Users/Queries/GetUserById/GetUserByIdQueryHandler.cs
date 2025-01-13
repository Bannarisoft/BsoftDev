using Core.Application.Users.Queries.GetUsers;
using Core.Domain.Entities;
using AutoMapper;
using MediatR;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IUser;
using Core.Domain.Events;

namespace Core.Application.Users.Queries.GetUserById
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery,UserDto>
    {
        private readonly IUserQueryRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;


        public GetUserByIdQueryHandler(IUserQueryRepository userRepository, IMapper mapper, IMediator mediator)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _mediator = mediator;

        }
        public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
          var user = await _userRepository.GetByIdAsync(request.UserId);
           //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: user.UserName,        
                    actionName: user.FirstName + " " + user.LastName,                
                    details: $"User '{user.UserName}' was created. FirstName and LastName: {user.FirstName}, {user.LastName}",
                    module:"User"
                );

                await _mediator.Publish(domainEvent, cancellationToken);
          return _mapper.Map<UserDto>(user);

        }
    }
}