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

namespace Core.Application.Users.Queries.GetUsers
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery,List<UserDto>>
    {
        private readonly IUserQueryRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserQueryHandler(IUserQueryRepository userRepository , IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        public async Task<List<UserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllUsersAsync();
            var userList = _mapper.Map<List<UserDto>>(users);
            return userList;
        }
    }
}


