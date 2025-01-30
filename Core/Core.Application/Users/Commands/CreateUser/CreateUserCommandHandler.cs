using Core.Application.Users.Queries.GetUsers;
using Core.Domain.Entities;
using AutoMapper;
using MediatR;
using Core.Application.Common.Interfaces.IUser;
using Core.Domain.Events;
using Core.Application.Common.HttpResponse;
using Serilog;
using Microsoft.Extensions.Logging;



namespace Core.Application.Users.Commands.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand,ApiResponseDTO<UserDto>>
    {
        private readonly IUserCommandRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 
        private readonly ILogger<CreateUserCommandHandler> _logger;
        private readonly IUserQueryRepository _userQueryRepository;


        public CreateUserCommandHandler(IUserCommandRepository userRepository, IMapper mapper, IMediator mediator,ILogger<CreateUserCommandHandler> logger,IUserQueryRepository userQueryRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _mediator = mediator;    
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userQueryRepository = userQueryRepository;

        }

        public async Task<ApiResponseDTO<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting user creation process for Username: {Username}", request.UserName);
            
           var existingUser = await _userQueryRepository.GetByUsernameAsync(request.UserName);
            if (existingUser != null)
            {
                return new ApiResponseDTO<UserDto>
                {
                    IsSuccess = false,
                    Message = "User already exists."
                    
                };
            }
             // Generate a new GUID for the user
            var userId = Guid.NewGuid();

            // Use AutoMapper to map CreateUserCommand to User entity
            var userEntity = _mapper.Map<User>(request);
            userEntity.Id = userId; // Assign the new GUID to the user entity

            // Hash and set the password
            userEntity.SetPassword(request.Password);

            // Save the user to the repository
            var createdUser = await _userRepository.CreateAsync(userEntity);

            if (createdUser == null)
            {
                _logger.LogError("Failed to create user for Username: {Username}", request.UserName);
                return new ApiResponseDTO<UserDto>
                {
                    IsSuccess = false,
                    Message = "Failed to create user. Please try again."
                };
            }                
            _logger.LogInformation("User successfully created for Username: {Username}", createdUser.UserName);
                
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: createdUser.UserName,
                actionName: createdUser.FirstName + " " + createdUser.LastName,
                details: $"User '{createdUser.UserName}' was created. FirstName: {createdUser.FirstName}, {createdUser.LastName}",
                module:"User"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            
            // Map the created user entity to DTO
            var userDto = _mapper.Map<UserDto>(createdUser);
            _logger.LogError("An exception occurred while creating user for Username: {Username}", request.UserName);
            return new ApiResponseDTO<UserDto>
            {
                IsSuccess = true,
                Message = "User created successfully",
                Data = userDto
            };
                    

        }
    }
}
