using MediatR;
using AutoMapper;
using Core.Application.Common.Interfaces.IUser;
using Core.Domain.Events;
using Core.Application.Common.HttpResponse;


namespace Core.Application.Users.Commands.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ApiResponseDTO<bool>>
    {
        private readonly IUserCommandRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 


        public UpdateUserCommandHandler(IUserCommandRepository userRepository, IMapper mapper, IMediator mediator)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _mediator = mediator;
            
        }

        public async Task<ApiResponseDTO<bool>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
             // Fetch the existing user
            var existingUser = await _userRepository.GetByIdAsync(request.UserId);
            if (existingUser == null)
            {
                return new ApiResponseDTO<bool>
                {
                    IsSuccess = false,
                    Message = "User not found.",
                    Data = false
                };
            }

            var OldUserName = existingUser.UserName;
            existingUser.UserName = request.UserName;

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

            // Hash the password if it's provided in the request
            if (!string.IsNullOrWhiteSpace(request.PasswordHash))
            {
                existingUser.SetPassword(request.PasswordHash); // Ensure SetPassword handles hashing
            }

            // Update the user in the repository
            var RowsUpdated = await _userRepository.UpdateAsync(request.UserId, existingUser);
            bool isUpdated = RowsUpdated > 0; 

            if (isUpdated)
            {
                return new ApiResponseDTO<bool>
                {
                    IsSuccess = true,
                    Message = "User updated successfully.",
                    Data = true
                };
            }

            return new ApiResponseDTO<bool>
            {
                IsSuccess = false,
                Message = "User update failed.",
                Data = false
            };

         }
    }
}
