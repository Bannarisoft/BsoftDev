using BSOFT.Domain.Interfaces;
using BSOFT.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BSOFT.Application.Users.Commands.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, int>
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<int> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var UpdateuserEntity = new User()
            {
                UserId = request.UserId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                IsActive = request.IsActive,
                PasswordHash = request.PasswordHash,
                UserType = request.UserType,
                Mobile = request.Mobile,
                EmailId = request.EmailId,
                CoId = request.CoId,
                UnitId = request.UnitId,
                DivId = request.DivId,
                RoleId = request.RoleId

            };

            return await _userRepository.UpdateAsync(request.UserId, UpdateuserEntity);
        }
    }
}
