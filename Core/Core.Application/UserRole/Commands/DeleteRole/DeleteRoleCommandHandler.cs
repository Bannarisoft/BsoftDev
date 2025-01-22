using MediatR;
using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IUserRole;
using Microsoft.Extensions.Logging;
using Core.Application.Departments.Commands.DeleteDepartment;
using Core.Application.Common.HttpResponse;
using Core.Domain.Events;

namespace Core.Application.UserRole.Commands.DeleteRole
{
    public class DeleteRoleCommandHandler :IRequestHandler<DeleteRoleCommand ,ApiResponseDTO<int>>
  
    {
    
        private readonly IUserRoleCommandRepository _IuserroleRepository;  
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 
        private readonly IUserRoleQueryRepository _userRoleQueryRepository;
         private readonly ILogger<DeleteRoleCommandHandler> _logger;
      
      public DeleteRoleCommandHandler (IUserRoleCommandRepository roleRepository , IMapper mapper,ILogger<DeleteRoleCommandHandler> logger,IMediator mediator, IUserRoleQueryRepository userRoleQueryRepository)
      {
        _IuserroleRepository =roleRepository;
         _mapper = mapper;
         _logger = logger;
         _mediator = mediator;
         _userRoleQueryRepository = userRoleQueryRepository;
      }

       public async Task<ApiResponseDTO<int>>Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
      {       
         _logger.LogInformation("DeleteUserroleCommandHandler started for User Role ID: {Id}", request.UserRoleId);

            // Check if department exists
            var userRole = await _userRoleQueryRepository.GetByIdAsync(request.UserRoleId);
          
            if (userRole == null)
            {
                _logger.LogWarning("User Role with ID {Id} not found.", request.UserRoleId);
                return new ApiResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = "User Role  not found",
                    Data = 0
                };
            }

            _logger.LogInformation("User Role  with ID {Id} found. Proceeding with deletion.", request.UserRoleId);

            // Map request to entity and delete
          //    var updateUserrole = _mapper.Map<Core.Domain.Entities.UserRole>(request.UserRoleId);
              var updateUserrole = _mapper.Map<Core.Domain.Entities.UserRole>(request.userRoleStatusDto);
           var userrole = await _IuserroleRepository.DeleteAsync(request.UserRoleId, updateUserrole);

                         
            if (userrole <= 0)
            {
                _logger.LogWarning("Failed to delete UserRole   with ID {Id}.", request.UserRoleId);
                return new ApiResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = "Failed to delete UserRole"
                   
                };
            }
                _logger.LogInformation("UserRole with ID {Id} deleted successfully.", request.UserRoleId);

            // Publish domain event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Delete",
                actionCode: updateUserrole.Id.ToString(),
                actionName: "",
                details: $"UserRole ID: {request.UserRoleId} was changed to status inactive.",
                module: "UserRole"
            );

            await _mediator.Publish(domainEvent, cancellationToken);
            _logger.LogInformation("AuditLogsDomainEvent published for UserRole ID {Id}.", request.UserRoleId);

            return new ApiResponseDTO<int>
            {
                IsSuccess = true,
                Message = "User Role deleted successfully"
            
            };  
          // // Map the command to the UserRole entity
          //   var updatedRole = _mapper.Map<Core.Domain.Entities.UserRole>(request);

          //   // Ensure the IsActive property is correctly updated
          //   updatedRole.IsActive = request.IsActive;

          //   // Pass the entity to the repository for deletion or updating
          //   return await _IuserroleRepository.DeleteAsync(request.UserRoleId, updatedRole);           
      }


    }
}