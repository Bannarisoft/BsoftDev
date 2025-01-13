using Core.Application.Users.Queries.GetUsers;
using Core.Domain.Entities;
using AutoMapper;
using MediatR;
using Core.Application.Common.Interfaces.IUser;
using Core.Domain.Events;


namespace Core.Application.Users.Commands.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
    {
        private readonly IUserCommandRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 


        public CreateUserCommandHandler(IUserCommandRepository userRepository, IMapper mapper, IMediator mediator)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _mediator = mediator;    

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
                            
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: createdUser.UserName,
                actionName: createdUser.FirstName + " " + createdUser.LastName,
                details: $"User '{createdUser.UserName}' was created. FirstName: {createdUser.FirstName}, {createdUser.LastName}",
                module:"User"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            
            if (createdUser == null)
            {
                throw new InvalidOperationException("Failed to create user");
            }

            // Map the created User entity to UserDto
            return _mapper.Map<UserDto>(createdUser);

        }
    }
}
