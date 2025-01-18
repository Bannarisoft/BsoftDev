using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using System.Text;
using Core.Domain.Entities;
using Core.Application.Users.Queries.GetUsers;
using Core.Application.Common.Interfaces.IUser;
using Core.Domain.Events;
using Microsoft.Extensions.Logging;
namespace Core.Application.Users.Queries.GetUserAutoComplete
{
    public class GetUserAutoCompleteQueryHandler: IRequestHandler<GetUserAutoCompleteQuery,List<UserDto>>
    {

        private readonly IUserQueryRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 
        private readonly ILogger<GetUserAutoCompleteQueryHandler> _logger;


		public GetUserAutoCompleteQueryHandler(IUserQueryRepository userRepository, IMapper mapper, IMediator mediator,ILogger<GetUserAutoCompleteQueryHandler> logger)
        {
           _userRepository =userRepository;
           _mapper =mapper;
           _mediator = mediator;
           _logger = logger ?? throw new ArgumentNullException(nameof(logger));
           
        }  
        
       public async Task<List<UserDto>> Handle(GetUserAutoCompleteQuery request, CancellationToken cancellationToken)
        {
            
            if (request == null || string.IsNullOrEmpty(request.SearchPattern))
            {
                 _logger.LogWarning("SearchPattern is null or empty in GetUserAutoCompleteQuery.");
                throw new ArgumentNullException(nameof(request.SearchPattern), "SearchPattern is null or empty");
            }
            _logger.LogInformation("Fetching users matching SearchPattern: {SearchPattern}", request.SearchPattern);

            // Fetch users matching the search pattern
            var users = await _userRepository.GetByUsernameAsync(request.SearchPattern);
            if (users == null || users.UserId == 0)
            {
                _logger.LogWarning("No users found for SearchPattern: {SearchPattern}", request.SearchPattern);
                return new List<UserDto>();
            }

            _logger.LogInformation("Found {UserCount} users matching SearchPattern: {SearchPattern}", users.UserId, request.SearchPattern);
            // Publish a domain event
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