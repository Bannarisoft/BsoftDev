using AutoMapper;
using MediatR;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces.IUser;
using Core.Domain.Events;

namespace Core.Application.Users.Queries.GetUsers
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery,List<UserDto>>
    {
        private readonly IUserQueryRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 


        public GetUserQueryHandler(IUserQueryRepository userRepository , IMapper mapper, IMediator mediator)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _mediator = mediator;

        }
        public async Task<List<UserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllUsersAsync();
            var userList = _mapper.Map<List<UserDto>>(users);
                            //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAll",
                    actionCode: "",        
                    actionName: "",
                    details: $"User details was fetched.",
                    module:"User"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
            return userList;
        }
    }
}


