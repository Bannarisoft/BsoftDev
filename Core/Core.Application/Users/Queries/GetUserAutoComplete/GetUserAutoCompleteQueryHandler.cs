using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using System.Text;
using Core.Domain.Entities;
using Core.Application.Users.Queries.GetUsers;
using Core.Application.Common.Interfaces.IUser;
using Core.Domain.Events;
namespace Core.Application.Users.Queries.GetUserAutoComplete
{
    public class GetUserAutoCompleteQueryHandler: IRequestHandler<GetUserAutoCompleteQuery,List<UserDto>>
    {

        private readonly IUserQueryRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

		public GetUserAutoCompleteQueryHandler(IUserQueryRepository userRepository, IMapper mapper, IMediator mediator)
        {
           _userRepository =userRepository;
           _mapper =mapper;
           _mediator = mediator;

           
        }  
        
       public async Task<List<UserDto>> Handle(GetUserAutoCompleteQuery request, CancellationToken cancellationToken)
        {
            if (request == null || string.IsNullOrEmpty(request.SearchPattern))
            {
                throw new ArgumentNullException(nameof(request.SearchPattern), "SearchPattern is null or empty");
            }

            var users = await _userRepository.GetByUsernameAsync(request.SearchPattern);
            //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAutoComplete",
                    actionCode:"",        
                    actionName: request.SearchPattern,                
                    details: $"User '{request.SearchPattern}' was searched",
                    module:"User"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
            return new List<UserDto> { _mapper.Map<UserDto>(users) };
            // var user = await _userRepository.GetByUsernameAsync(request.SearchPattern);
            // var userList = new List<UserAutoCompleteDto>
            // {
            //     new UserAutoCompleteDto
            //     {
            //         UserId = user.UserId,
            //         UserName = user.UserName
            //     }
            // };

            // return userList;
        }
    }
}