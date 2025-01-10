using AutoMapper;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IUser;
using Core.Domain.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.Users.Commands.DeleteUser
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, int>
    {
        private readonly IUserCommandRepository _userRepository;
        private readonly IMapper _mapper;

        public DeleteUserCommandHandler(IUserCommandRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;

        }
        public async Task<int> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var UserDelete = _mapper.Map<User>(request);

            UserDelete.IsActive = request.IsActive;

            return await _userRepository.DeleteAsync(request.UserId, UserDelete);           

        }
    }
}