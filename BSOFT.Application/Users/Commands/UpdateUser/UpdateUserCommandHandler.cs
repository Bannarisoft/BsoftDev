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
                Id = request.Id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                UserPassword = request.UserPassword
            };

            return await _userRepository.UpdateAsync(request.Id, UpdateuserEntity);
        }
    }
}
