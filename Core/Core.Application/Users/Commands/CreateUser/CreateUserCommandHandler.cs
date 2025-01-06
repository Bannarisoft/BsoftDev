using Core.Application.Users.Queries.GetUsers;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IUser;

namespace Core.Application.Users.Commands.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
    {
        private readonly IUserCommandRepository _userRepository;
        private readonly IMapper _mapper;

        public CreateUserCommandHandler(IUserCommandRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
             // Generate a new GUID for the user
            var userId = Guid.NewGuid();

            // Use AutoMapper to map CreateUserCommand to User entity
            var userEntity = _mapper.Map<User>(request);
            userEntity.Id = userId; // Assign the new GUID to the user entity

            // Hash and set the password
            userEntity.SetPassword(request.Password);

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
