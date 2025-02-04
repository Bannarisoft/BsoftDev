using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Application.UserRole.Queries.GetRole;
using Core.Application.Common.Interfaces.IUserRole;
using Core.Application.Common.HttpResponse;
using Core.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Core.Application.UserRole.Queries.GetRolesAutocomplete
{
    public class GetRolesAutocompleteQueryHandler : IRequestHandler<GetRolesAutocompleteQuery, ApiResponseDTO<List<GetUserRoleAutocompleteDto>>>
    {
        private readonly IUserRoleQueryRepository _userRoleRepository;
     private readonly IMapper _mapper;
       private readonly ILogger<GetRolesAutocompleteQueryHandler> _logger;
     private readonly IMediator _mediator;

        public GetRolesAutocompleteQueryHandler(IUserRoleQueryRepository userRoleRepository, IMapper mapper, IMediator mediator,ILogger<GetRolesAutocompleteQueryHandler> logger)
        {
           _userRoleRepository = userRoleRepository;
            _mapper =mapper;

            _mediator=mediator;

            _logger = logger;


        }

        public async Task<ApiResponseDTO<List<GetUserRoleAutocompleteDto>>> Handle(GetRolesAutocompleteQuery request, CancellationToken cancellationToken)
        {

                  _logger.LogInformation("Handling GetDepartmentAutoCompleteSearchQuery with search pattern: {SearchPattern}", request.SearchTerm);

             // Fetch departments matching the search pattern
                var result = await _userRoleRepository.GetRolesAsync(request.SearchTerm);
                if (result == null || !result.Any())
                {
                    _logger.LogWarning("No departments found for search pattern: {SearchPattern}", request.SearchTerm);
                    return new ApiResponseDTO<List<GetUserRoleAutocompleteDto>>
                    {
                        IsSuccess = false,
                        Message = "No matching departments found",
                        Data = new List<GetUserRoleAutocompleteDto>()
                    };
                }

                _logger.LogInformation("Departments found for search pattern: {SearchPattern}. Mapping results to DTO.", request.SearchTerm);

                // Map the result to DTO
                var userRoleDto = _mapper.Map<List<GetUserRoleAutocompleteDto>>(result);

                // Publish domain event for audit logs
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAutoComplete",
                    actionCode: "",
                    actionName: request.SearchTerm,
                    details: $"User Role '{request.SearchTerm}' was searched",
                    module: "User Role"
                );
                await _mediator.Publish(domainEvent, cancellationToken);

                _logger.LogInformation("Domain event published for search pattern: {SearchPattern}", request.SearchTerm);

                return new ApiResponseDTO<List<GetUserRoleAutocompleteDto>>
                {
                    IsSuccess = true,
                    Message = "Success",
                    Data = userRoleDto
                };

               


    
        }
        
    }
}