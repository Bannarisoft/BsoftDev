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
            // Use AutoMapper to map CreateUserCommand to User entity
            var userEntity = _mapper.Map<User>(request);
            // userEntity.Id = Guid.NewGuid(); // Assign a new GUID for the User ID
            // userEntity.UserId = 0;

            // Save the user to the repository
            var createdUser = await _userRepository.CreateAsync(userEntity);

            // Map the created User entity to UserVm
            return _mapper.Map<UserVm>(createdUser);

            // var userEntity = new User
            // {
            //     Id = Guid.NewGuid(),
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
            //     RoleId = request.RoleId,
            //     Role = request.Role,
            //     CreatedBy = request.CreatedBy,
            //     CreatedAt = request.CreatedAt,
            //     CreatedByName = request.CreatedByName,
            //     CreatedIP = request.CreatedIP,
            //     ModifiedBy = request.ModifiedBy,
            //     ModifiedAt = request.ModifiedAt,
            //     ModifiedByName = request.ModifiedByName,    
            //     ModifiedIP = request.ModifiedIP       
            // };

            // var result = await _userRepository.CreateAsync(userEntity);
            // return _mapper.Map<UserVm>(result);
        }
    }
}
