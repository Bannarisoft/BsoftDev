using Core.Application.Common.Interfaces;
using Core.Application.UserRole.Queries.GetRole;
using Core.Domain.Entities;
using MediatR;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IUserRole;
using Core.Application.Common.HttpResponse;
using Core.Domain.Events;
using Microsoft.Extensions.Logging;
using Core.Domain.Enums.Common;

namespace Core.Application.UserRole.Commands.UpdateRole
{
    public class UpdateRoleCommandHandler  : IRequestHandler<UpdateRoleCommand ,ApiResponseDTO<UserRoleDto>> 
    {


        public readonly IUserRoleCommandRepository _IUserRoleRepository;
        private readonly IMapper _Imapper;      
    
        private readonly IUserRoleQueryRepository _IUserRoleQueryRepository;
        private readonly IMediator _mediator; 
           private readonly ILogger<UpdateRoleCommandHandler> _logger;
        public UpdateRoleCommandHandler(IUserRoleCommandRepository roleRepository,IUserRoleQueryRepository userRoleQueryRepository ,IMapper mapper,IMediator mediator,ILogger<UpdateRoleCommandHandler> logger)
        {
            _IUserRoleRepository = roleRepository;
            _Imapper = mapper;
            _IUserRoleQueryRepository = userRoleQueryRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<ApiResponseDTO<UserRoleDto>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            
            _logger.LogInformation($"Starting UpdateUserRoleCommandHandler for request: {request}" );

         
            var userrole = await _IUserRoleQueryRepository.GetByIdAsync(request.Id);
            if (userrole == null)
            {
                _logger.LogWarning($"User Role with ID {request.Id} not found.");
                return new ApiResponseDTO<UserRoleDto>
                {
                    IsSuccess = false,
                    Message = "User Role not found"
                };
            }

            _logger.LogInformation($"User Role with ID {request.Id} retrieved successfully.");
            
            var userroleMap  = _Imapper.Map<Core.Domain.Entities.UserRole>(request);                            

            // Save updates to the repository
            var result = await _IUserRoleRepository.UpdateAsync(request.Id, userroleMap);

            if (result <= 0)
            {
                _logger.LogWarning($"Failed to update User Role with ID {request.Id}.");
                return new ApiResponseDTO<UserRoleDto>
                {
                    IsSuccess = false,
                    Message = "Failed to update User Role"
                };
            }

               var duplicateCheck = await _IUserRoleRepository.ExistsByNameupdateAsync(request.RoleName, request.Id);
                if (duplicateCheck)
                {
                    return new ApiResponseDTO<UserRoleDto>
                    {
                    IsSuccess = false,
                    Message = " User Role Name  Already Exists "
                    };
                }

            _logger.LogInformation($"User Role with ID {request.Id} updated successfully." );

            // Map the updated entity to DTO
            var role = await _IUserRoleQueryRepository.GetByIdAsync(request.Id);
            var roleDto = _Imapper.Map<UserRoleDto>(role);
         

            // Publish domain event for audit logs
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Update",
                actionCode: userrole.RoleName,
                actionName: userrole.RoleName,
                details: $"User Role '{userrole.RoleName}' was updated. User Role ID: {request.Id}",
                module: "User Role"
            );

            await _mediator.Publish(domainEvent, cancellationToken);
            _logger.LogInformation($"AuditLogsDomainEvent published for User Role ID {userrole.Id}." );

            return new ApiResponseDTO<UserRoleDto>
            {
                IsSuccess = true,
                Message = "User Role updated successfully"
               
            };             
       }
        


    }
}