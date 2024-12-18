using AutoMapper;
using BSOFT.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSOFT.Application.Users.Queries.GetUsers
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery,List<UserVm>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserQueryHandler(IUserRepository userRepository , IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        public async Task<List<UserVm>> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllUsersAsync();
            var userList = _mapper.Map<List<UserVm>>(users);
            return userList;
        }
    }
}