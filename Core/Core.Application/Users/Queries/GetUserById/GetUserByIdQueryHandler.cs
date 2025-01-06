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

namespace Core.Application.Users.Queries.GetUserById
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery,UserDto>
    {
        private readonly IUserQueryRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserByIdQueryHandler(IUserQueryRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
          var user = await _userRepository.GetByIdAsync(request.UserId);
          return _mapper.Map<UserDto>(user);

        }
    }
}