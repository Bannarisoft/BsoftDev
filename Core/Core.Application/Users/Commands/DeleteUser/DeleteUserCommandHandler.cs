using AutoMapper;
using Core.Application.Common.Interfaces.IUser;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Users.Commands.DeleteUser
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, int>
    {
        private readonly IUserCommandRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 


        public DeleteUserCommandHandler(IUserCommandRepository userRepository, IMapper mapper, IMediator mediator)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _mediator = mediator;


        }
        public async Task<int> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var UserDelete = _mapper.Map<User>(request);
            //Domain Event  
                    var domainEvent = new AuditLogsDomainEvent(
                        actionDetail: "Delete",
                        actionCode: UserDelete.UserName,
                        actionName: UserDelete.FirstName + " " + UserDelete.LastName,
                        details: $"User '{UserDelete.UserName}' was created. FirstName: {UserDelete.FirstName}, {UserDelete.LastName}",

                        module:"User"
                    );               
                    await _mediator.Publish(domainEvent, cancellationToken);  

            UserDelete.IsActive = request.IsActive;

            return await _userRepository.DeleteAsync(request.UserId, UserDelete);           

        }
    }
}