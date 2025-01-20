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
            // // Map the updated data from the request to the domain entity
            _logger.LogInformation("Starting UpdateUserRoleCommandHandler for request: {@Request}", request);

            // Fetch the department by ID
            var department = await _IUserRoleQueryRepository.GetByIdAsync(request.Id);
            if (department == null)
            {
                _logger.LogWarning("User Role with ID {Id} not found.", request.Id);
                return new ApiResponseDTO<UserRoleDto>
                {
                    IsSuccess = false,
                    Message = "User Role not found"
                };
            }

            _logger.LogInformation("User Role with ID {Id} retrieved successfully.", request.Id);

            // Update department properties
            department.RoleName = request.RoleName;
            department.Description = request.Description;
            department.CompanyId = request.CompanyId;
            department.IsActive = request.IsActive;

            // Save updates to the repository
            var result = await _IUserRoleRepository.UpdateAsync(request.Id, department);

            if (result == null)
            {
                _logger.LogWarning("Failed to update User Role with ID {Id}.", request.Id);
                return new ApiResponseDTO<UserRoleDto>
                {
                    IsSuccess = false,
                    Message = "Failed to update User Role"
                };
            }

            _logger.LogInformation("User Role with ID {Id} updated successfully.", request.Id);

            // Map the updated entity to DTO
            var role = await _IUserRoleQueryRepository.GetByIdAsync(request.Id);
            var departmentDto = _Imapper.Map<UserRoleDto>(role);
           // var departmentDto = _Imapper.Map<DepartmentDto>(result);

            // Publish domain event for audit logs
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Update",
                actionCode: department.Id.ToString(),
                actionName: department.RoleName,
                details: $"User Role '{department.RoleName}' was updated. User Role ID: {request.Id}",
                module: "User Role"
            );

            await _mediator.Publish(domainEvent, cancellationToken);
            _logger.LogInformation("AuditLogsDomainEvent published for User Role ID {DepartmentId}.", department.Id);

            return new ApiResponseDTO<UserRoleDto>
            {
                IsSuccess = true,
                Message = "User Role updated successfully"
               
            };
            
//              var userrole = await _IUserRoleQueryRepository.GetByIdAsync(request.Id);

// 			 userrole.RoleName = request.RoleName; // Update properties based on the request
//             userrole.Description= request.Description;
//             userrole.CompanyId= request.CompanyId;
//             userrole.IsActive= request.IsActive;

//             var result = await _IUserRoleRepository.UpdateAsync(request.Id, userrole);
            
//             var userroleDto = _Imapper.Map<UserRoleDto>(result);

//  // Publish a domain event for audit logs
//     var domainEvent = new AuditLogsDomainEvent(
//         actionDetail: "Update",
//         actionCode: userrole.Id.ToString(),
//         actionName: userrole.RoleName,
//         details: $"UserRole '{userrole.RoleName}' was updated. UserRole ID: {request.Id}",
//         module: "UserRole"
//     );
//     await _mediator.Publish(domainEvent, cancellationToken);
          
//             return new ApiResponseDTO<UserRoleDto> { IsSuccess = true, Message = "UserRole updated successfully", Data = userroleDto };


       }
        


    }
}