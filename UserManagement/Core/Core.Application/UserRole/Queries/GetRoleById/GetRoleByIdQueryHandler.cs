using Core.Application.UserRole.Queries.GetRole;
using Core.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Core.Application.Common.Interfaces.IUserRole;
using Core.Application.Common.HttpResponse;
using Core.Domain.Events;
using Microsoft.Extensions.Logging;


namespace Core.Application.UserRole.Queries.GetRoleById
{
    public class GetRoleByIdQueryHandler :IRequestHandler<GetRoleByIdQuery,ApiResponseDTO<GetUserRoleDto>>
    {
    private readonly IUserRoleQueryRepository _userRoleRepository;
     private readonly IMapper _mapper;
   private readonly IMediator _mediator;
   private readonly ILogger<GetRoleByIdQueryHandler> _logger;
      
          public GetRoleByIdQueryHandler(IUserRoleQueryRepository userRoleRepository, IMapper mapper ,IMediator mediator,ILogger<GetRoleByIdQueryHandler> logger)
          {
            _userRoleRepository = userRoleRepository;
            _mapper =mapper;
            _mediator =mediator;
            _logger = logger;
        
          }

          public async Task<ApiResponseDTO<GetUserRoleDto>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
          {
              _logger.LogInformation($"Processing GetRoleByIdQuery for ID: { request.Id}.");


              // Fetch the user role by ID
                var userRole = await _userRoleRepository.GetByIdAsync(request.Id);
                if (userRole is null)
                {
                    _logger.LogWarning($"No user role found with ID: { request.Id}.");
                    return new ApiResponseDTO<GetUserRoleDto>
                    {
                        IsSuccess = false,
                        Message = $"No user role found with ID: {request.Id}.",
                        Data = null
                    };
                }

                _logger.LogInformation($"User role found with ID: { request.Id}. Mapping to DTO.");
                var roleDto = _mapper.Map<GetUserRoleDto>(userRole);

                // Publish domain event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: roleDto.RoleName,
                    actionName: roleDto.RoleName,
                    details: $"UserRole '{roleDto.RoleName}' was fetched. RoleID: {roleDto.Id}.",
                    module: "UserRole"
                );

                _logger.LogInformation($"Publishing AuditLogsDomainEvent for UserRole ID: { request.Id}.");
                await _mediator.Publish(domainEvent, cancellationToken);

                _logger.LogInformation($"Returning success response for UserRole ID: {roleDto.Id}." );
                return new ApiResponseDTO<GetUserRoleDto>
                {
                    IsSuccess = true,
                    Message = "Success",
                    Data = roleDto
                };
         


          }

        
    }
}