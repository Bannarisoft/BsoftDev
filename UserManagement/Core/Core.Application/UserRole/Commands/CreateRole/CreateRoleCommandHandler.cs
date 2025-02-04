using Core.Application.UserRole.Queries.GetRole;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IUserRole;
using Core.Application.Common.HttpResponse;
using Core.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Core.Application.UserRole.Commands.CreateRole
{
    public class CreateRoleCommandHandler :IRequestHandler<CreateRoleCommand,  ApiResponseDTO<UserRoleDto>>
    {
        
        private readonly IUserRoleCommandRepository _roleRepository;
        private readonly IUserRoleQueryRepository _userRoleQueryRepository;
        private readonly IMapper _mapper;
        
        private readonly IMediator _mediator; 
        
          private readonly ILogger<CreateRoleCommandHandler> _logger;

        public CreateRoleCommandHandler(IUserRoleCommandRepository roleRepository ,IUserRoleQueryRepository userRoleQueryRepository  ,IMapper mapper,IMediator mediator,ILogger<CreateRoleCommandHandler> logger)
        {
             _roleRepository=roleRepository;
             _userRoleQueryRepository=  userRoleQueryRepository;
            _mapper=mapper;
            _mediator=mediator;
            _logger=logger;
        }

        public async Task<ApiResponseDTO<UserRoleDto>>Handle(CreateRoleCommand request,CancellationToken cancellationToken)
         {          
            _logger.LogInformation("Starting CreateUserRoleCommandHandler for request: {@Request}", request);
            
            var exists = await _roleRepository.ExistsByCodeAsync(request.RoleName);
              if (exists)
                 {
                     _logger.LogWarning("Entity Name {EntityName} already exists.", request.RoleName);
                        return new ApiResponseDTO<UserRoleDto>
                    {
                    IsSuccess = false,
                    Message = "UserRole Name already exists."
                    };
                }
            // Map the request to the entity
            var userRoleEntity = _mapper.Map<Core.Domain.Entities.UserRole>(request);
            _logger.LogInformation("Mapped CreateUserRoleCommand to userRole entity:{@userRoleEntity}", userRoleEntity);

            // Save the department
            var createdUserRole = await _roleRepository.CreateAsync(userRoleEntity);

            if (createdUserRole == null)
            {
                _logger.LogWarning("Failed to create department. UserRole entity: {@userRoleEntity}", userRoleEntity);
                return new ApiResponseDTO<UserRoleDto>
                {
                    IsSuccess = false,
                    Message = "UserRole not created"
                };
            }

            _logger.LogInformation("UserRole successfully created with ID: {Id}", createdUserRole.Id);

            // Publish the domain event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: createdUserRole.Id.ToString(),
                actionName: createdUserRole.RoleName,
                details: $"UserRole '{createdUserRole.RoleName}' was created. ID: {createdUserRole.Id}",
                module: "UserRole"
            );

            await _mediator.Publish(domainEvent, cancellationToken);
            _logger.LogInformation("AuditLogsDomainEvent published for UserRole ID: {Id}", createdUserRole.Id);

            // Map the result to DTO
            var userrolrDto = _mapper.Map<UserRoleDto>(createdUserRole);

            _logger.LogInformation("Returning success response for UserRole ID: {Id}", createdUserRole.Id);

            return new ApiResponseDTO<UserRoleDto>
            {
                IsSuccess = true,
                Message = "UserRole created successfully",
                Data = userrolrDto
            };
           
       


         }



    }
}