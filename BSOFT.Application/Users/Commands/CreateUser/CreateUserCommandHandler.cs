using BSOFT.Application.Users.Queries.GetUsers;
using BSOFT.Application.UserLogin.Commands.UserLogin;
using BSOFT.Domain.Entities;
using BSOFT.Domain.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BSOFT.Application.Users.Commands.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserVm>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public CreateUserCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserVm> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var userEntity = new User
            {
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
                RoleId = request.RoleId,
                CreatedBy = request.CreatedBy,
                CreatedAt = request.CreatedAt,
                CreatedByName = request.CreatedByName,
                ModifiedBy = request.ModifiedBy,
                ModifiedAt = request.ModifiedAt,
                ModifiedByName = request.ModifiedByName           
            };

            var result = await _userRepository.CreateAsync(userEntity);
            return _mapper.Map<UserVm>(result);
        }
    }
}
