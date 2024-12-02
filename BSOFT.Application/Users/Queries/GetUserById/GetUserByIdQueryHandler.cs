using BSOFT.Application.Users.Queries.GetUsers;
using BSOFT.Domain.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BSOFT.Application.Users.Queries.GetUserById
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery,UserVm>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserByIdQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        public async Task<UserVm> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
          var user = await _userRepository.GetByIdAsync(request.UserId);
          return _mapper.Map<UserVm>(user);
        }
    }
}