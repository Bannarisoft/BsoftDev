using BSOFT.Domain.Interfaces;
using BSOFT.Domain.Entities;
using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;

namespace BSOFT.Application.Users.Commands.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, int>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UpdateUserCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<int> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            // Use AutoMapper to map UpdateUserCommand to User entity
            var updateUserEntity = _mapper.Map<User>(request);

            // Update the user in the repository
            return await _userRepository.UpdateAsync(request.Id, updateUserEntity);
            // var UpdateuserEntity = new User()
            // {
            //     FirstName = request.FirstName,
            //     LastName = request.LastName,
            //     UserName = request.UserName,
            //     IsActive = request.IsActive,
            //     PasswordHash = request.PasswordHash,
            //     UserType = request.UserType,
            //     Mobile = request.Mobile,
            //     EmailId = request.EmailId,
            //     CoId = request.CoId,
            //     UnitId = request.UnitId,
            //     DivId = request.DivId,
            //     RoleId = request.RoleId

            // };

            // return await _userRepository.UpdateAsync(request.UserId, UpdateuserEntity);
        }
    }
}
