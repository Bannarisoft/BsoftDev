using Core.Application.Users.Queries.GetUsers;
using Core.Domain.Entities;
using AutoMapper;
using MediatR;
using Core.Application.Common.Interfaces.IUser;
using Core.Domain.Events;
using Core.Application.Common.HttpResponse;
using Serilog;
using Microsoft.Extensions.Logging;
using Core.Application.Common.Interfaces;
using MassTransit;
using Contracts.Events;


namespace Core.Application.Users.Commands.CreateUser
{

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ApiResponseDTO<UserDto>>
    {
        private readonly IUserCommandRepository _userRepository;
        private readonly IMapper _mapper;

        private readonly IMediator _mediator;
        private readonly ILogger<CreateUserCommandHandler> _logger;
        private readonly IUserQueryRepository _userQueryRepository;
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;
        private readonly IPublishEndpoint _publishEndpoint;  // MassTransit Publish Endpoint


        public CreateUserCommandHandler(IUserCommandRepository userRepository, IMapper mapper, IMediator mediator, ILogger<CreateUserCommandHandler> logger, IEmailService emailService, ISmsService smsService, IUserQueryRepository userQueryRepository
        , IPublishEndpoint publishEndpoint)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _mediator = mediator;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userQueryRepository = userQueryRepository;
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _smsService = smsService ?? throw new ArgumentNullException(nameof(smsService));
            _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));

        }

        public async Task<ApiResponseDTO<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting user creation process for Username: {Username}", request.UserName);



            // Use AutoMapper to map CreateUserCommand to User entity
            var userEntity = _mapper.Map<User>(request);



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

            // Publish UserCreatedEvent to RabbitMQ
            // var userCreatedEvent = new
            // {
            //     CorrelationId = Guid.NewGuid(),
            //     UserId = createdUser.UserId,  // Using generated UserId
            //     Email = createdUser.EmailId
            // };


            // await _publishEndpoint.Publish<IUserCreatedEvent>(userCreatedEvent, context =>
            // {
            //     context.Durable = true;  // Ensures message is persistent in RabbitMQ
            // });

            // _logger.LogInformation("UserCreatedEvent published successfully for UserId: {UserId}", createdUser.UserId);

            // Publish UserCreatedEvent to RabbitMQ
            // var userCreatedEvent = new UserCreatedEvent
            // {
            //     CorrelationId = Guid.NewGuid(),
            //     UserId = createdUser.UserId,
            //     Email = createdUser.EmailId
            // };
            // try
            // {
            //     // Publish UserCreatedEvent to RabbitMQ
            //     await _publishEndpoint.Publish<IUserCreatedEvent>(userCreatedEvent);
            //     _logger.LogInformation("UserCreatedEvent published successfully for UserId: {UserId}", createdUser.UserId);
            // }
            // catch (Exception ex)
            // {
            //     _logger.LogError("Error publishing UserCreatedEvent: {Message}", ex.Message);
            // }


            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: createdUser.UserName,
                actionName: createdUser.FirstName + " " + createdUser.LastName,
                details: $"User '{createdUser.UserName}' was created. FirstName: {createdUser.FirstName}, {createdUser.LastName}",

                module: "User"
            );
            await _mediator.Publish(domainEvent, cancellationToken);

            // 🔥 Publish UserCreatedEvent to RabbitMQ
            var correlationId = Guid.NewGuid();
            var userCreatedEvent = new UserCreatedEvent
            {
                CorrelationId = correlationId,
                UserId = createdUser.UserId,
                Email = createdUser.EmailId
            };

            try
            {
                await _publishEndpoint.Publish(userCreatedEvent, cancellationToken);
                _logger.LogInformation("UserCreatedEvent published successfully for UserId: {UserId} with CorrelationId: {CorrelationId}", createdUser.UserId, correlationId);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error publishing UserCreatedEvent: {Message}", ex.Message);
            }
            
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
