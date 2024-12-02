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

namespace BSOFT.Application.Users.Commands.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand,UserVm>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public CreateUserCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        public async Tasks<UserVm> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var userEntity =new Users() { FirstName = request.FirstName,LastName = request.LastName,UserName = request.UserName,UserPassword = request.UserPassword };
            var result = await _userRepository.CreateAsync(userEntity);
            return _mapper.Map<UserVm>(result);

        }
    }
}