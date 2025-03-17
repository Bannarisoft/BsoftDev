using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IUserGroup;
using Core.Application.Users.Queries.GetUserAutoComplete;
using Core.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.UserGroup.Queries.GetUserGroupAutoComplete
{    
    public class GetUserGroupAutoCompleteQueryHandler : IRequestHandler<GetUserGroupAutoCompleteQuery, ApiResponseDTO<List<UserGroupAutoCompleteDto>>>
    {

        private readonly IUserGroupQueryRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;         


		public GetUserGroupAutoCompleteQueryHandler(IUserGroupQueryRepository userRepository, IMapper mapper, IMediator mediator)
        {
           _userRepository =userRepository;
           _mapper =mapper;
           _mediator = mediator;
        }  
   
        public  async Task<ApiResponseDTO<List<UserGroupAutoCompleteDto>>> Handle(GetUserGroupAutoCompleteQuery request, CancellationToken cancellationToken)
        {
                           
            var result = await _userRepository.GetUserGroups(request.SearchPattern ?? string.Empty);
            if (result is null || result.Count is 0)
            {
                return new ApiResponseDTO<List<UserGroupAutoCompleteDto>>
                {
                    IsSuccess = false,
                    Message = "No countries found matching the search pattern."
                };
            }
            // Publish a domain event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetAutoComplete",
                actionCode:"",        
                actionName: request.SearchPattern ?? string.Empty,                
                details: $"User '{request.SearchPattern}' was searched",
                module:"User"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            
            var userDto = _mapper.Map<List<UserGroupAutoCompleteDto>>(result);
            return new ApiResponseDTO<List<UserGroupAutoCompleteDto>>
            {
                IsSuccess = true,
                Data = userDto
            };
        }
    }
}