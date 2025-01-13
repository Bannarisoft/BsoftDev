using MediatR;
using AutoMapper;
using Core.Application.Common.Interfaces.IUser;
using Core.Domain.Events;


namespace Core.Application.Users.Commands.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, int>
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

        public async Task<int> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            // Fetch the existing user
            var existingUser = await _userRepository.GetByIdAsync(request.UserId);

            if (existingUser == null)
            {
                throw new KeyNotFoundException("User not found.");
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
            await _userRepository.UpdateAsync(request.UserId,existingUser);

            // Return the updated user's ID
            return existingUser.UserId;

         }
    }
}
