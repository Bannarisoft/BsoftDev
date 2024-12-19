using BSOFT.Application.Users.Queries.GetUsers;
using BSOFT.Domain.Entities;
using BSOFT.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BSOFT.Application.Users.Commands.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public CreateUserCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            // Use AutoMapper to map CreateUserCommand to User entity
            var userEntity = _mapper.Map<User>(request);

            // Save the user to the repository
            var createdUser = await _userRepository.CreateAsync(userEntity);
            
            if (createdUser == null)
            {
                throw new InvalidOperationException("Failed to create user");
            }

            // Map the created User entity to UserDto
            return _mapper.Map<UserDto>(createdUser);

        }
    }
}
