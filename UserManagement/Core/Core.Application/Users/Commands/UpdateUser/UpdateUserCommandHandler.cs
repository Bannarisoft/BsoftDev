using MediatR;
using AutoMapper;
using Core.Application.Common.Interfaces.IUser;
using Core.Domain.Events;
using Core.Application.Common.HttpResponse;
using Microsoft.Extensions.Logging;


namespace Core.Application.Users.Commands.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ApiResponseDTO<bool>>
    {
        private readonly IUserCommandRepository _userRepository;
        private readonly IUserQueryRepository _userQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 
        private readonly ILogger<UpdateUserCommandHandler> _logger;



        public UpdateUserCommandHandler(IUserCommandRepository userRepository, IUserQueryRepository userQueryRepository,IMapper mapper, IMediator mediator,ILogger<UpdateUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _userQueryRepository = userQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            
        }

        public async Task<ApiResponseDTO<bool>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
             _logger.LogInformation("Starting user update process for UserId: {UserId}", request.UserId);
            

            var existingUser = await _userQueryRepository.GetByIdAsync(request.UserId);
            if (existingUser == null)
            {
                _logger.LogWarning("User with UserId: {UserId} not found.", request.UserId);
                return new ApiResponseDTO<bool>
                {
                    IsSuccess = false,
                    Message = "User not found.",
                    Data = false
                };
            }

            var OldUserName = existingUser.UserName;
            existingUser.UserName = request.UserName;
            _logger.LogInformation("Updating user details for UserId: {UserId}. Old UserName: {OldUserName}, New UserName: {NewUserName}", 
                request.UserId, OldUserName, existingUser.UserName);

             _mapper.Map(request, existingUser);
            
            //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Update",
                    actionCode: existingUser.UserName,
                    actionName: existingUser.FirstName + " " + existingUser.LastName,
                    details: $"User '{OldUserName}' was updated to '{existingUser.UserName}'.  FirstName: {existingUser.FirstName}",
                    module:"User"
                );            
                await _mediator.Publish(domainEvent, cancellationToken);



            // Update the user in the repository
            var RowsUpdated = await _userRepository.UpdateAsync(request.UserId, existingUser);
            bool isUpdated = RowsUpdated > 0; 

            if (isUpdated)
            {
                _logger.LogInformation("User with UserId: {UserId} updated successfully.", request.UserId);
                return new ApiResponseDTO<bool>
                {
                    IsSuccess = true,
                    Message = "User updated successfully.",
                    Data = true
                };
            }

            _logger.LogWarning("Failed to update user with UserId: {UserId}.", request.UserId);
            return new ApiResponseDTO<bool>
            {
                IsSuccess = false,
                Message = "User update failed.",
                Data = false
            };

         }
    }
}
