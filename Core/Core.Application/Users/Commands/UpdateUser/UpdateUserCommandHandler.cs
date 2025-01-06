using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IUser;

namespace Core.Application.Users.Commands.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, int>
    {
        private readonly IUserCommandRepository _userRepository;
        private readonly IMapper _mapper;

        public UpdateUserCommandHandler(IUserCommandRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<int> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            // Fetch the existing user
            var existingUser = await _userRepository.GetByIdAsync(request.UserId);

            if (existingUser == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

             _mapper.Map(request, existingUser);

            // Hash the password if it's provided in the request
            if (!string.IsNullOrWhiteSpace(request.PasswordHash))
            {
                existingUser.SetPassword(request.PasswordHash); // Ensure SetPassword handles hashing
            }

            // Update the user in the repository
            await _userRepository.UpdateAsync(request.UserId,existingUser);

            // Return the updated user's ID
            return existingUser.UserId;

            // // Use AutoMapper to map UpdateUserCommand to User entity
            // var updateUserEntity = _mapper.Map<User>(request);

            // // Update the user in the repository
            // return await _userRepository.UpdateAsync(request.UserId, updateUserEntity);
         }
    }
}
