using BSOFT.Application.Users.Queries.GetUsers;
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
                UserPassword = request.UserPassword,
                UserType = request.UserType,
                Mobile = request.Mobile,
                EmailId = request.EmailId,
                CreatedBy = request.CreatedBy,
                Created_Time = request.Created_Time,
                CreatedByName = request.CreatedByName,
                ModifiedBy = request.ModifiedBy,
                Modified_Time = request.Modified_Time,
                ModifiedByName = request.ModifiedByName           
            };

            var result = await _userRepository.CreateAsync(userEntity);
            return _mapper.Map<UserVm>(result);
        }
    }
}
