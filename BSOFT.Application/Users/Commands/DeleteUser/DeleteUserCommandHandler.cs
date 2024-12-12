using BSOFT.Domain.Interfaces;
using BSOFT.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace BSOFT.Application.Users.Commands.DeleteUser
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, int>
    {
        private readonly IUserRepository _userRepository;

        public DeleteUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            
        }
        public async Task<int> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var UserUpdate = new User()
            {
                UserId = request.UserId,
                IsActive = request.IsActive 
            };
            return await _userRepository.DeleteAsync(request.UserId,UserUpdate);
        }
    }
}