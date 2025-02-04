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
         _logger.LogInformation("DeleteUserroleCommandHandler started for User Role ID: {Id}", request.Id);
            var updateduserrolemap = _mapper.Map<Core.Domain.Entities.UserRole>(request);
           
            _logger.LogInformation("User Role  with ID {Id} found. Proceeding with deletion.", request.Id);

            
           var userrole = await _IuserroleRepository.DeleteAsync(request.Id, updateduserrolemap);
                         
            if (userrole <= 0)
            {
                _logger.LogWarning("Failed to delete UserRole   with ID {Id}.", request.Id);
                return new ApiResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = "Failed to delete UserRole"
                   
                };
            }
                _logger.LogInformation("UserRole with ID {Id} deleted successfully.", request.Id);

            // Publish domain event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Delete",
                actionCode: updateduserrolemap.Id.ToString(),
                actionName: "",
                details: $"UserRole ID: {request.Id} was changed to status inactive.",
                module: "UserRole"
            );

            await _mediator.Publish(domainEvent, cancellationToken);
            _logger.LogInformation("AuditLogsDomainEvent published for UserRole ID {Id}.", request.Id);

            return new ApiResponseDTO<int>
            {
                IsSuccess = true,
                Message = "User Role deleted successfully"
            
            };  
           
      }


    }
}