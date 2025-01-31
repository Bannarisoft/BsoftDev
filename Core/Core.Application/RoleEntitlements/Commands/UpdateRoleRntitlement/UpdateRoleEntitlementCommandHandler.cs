using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using MediatR;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IRoleEntitlement;
using Core.Domain.Events;
using Core.Application.Common.HttpResponse;
using Microsoft.Extensions.Logging;

namespace Core.Application.RoleEntitlements.Commands.UpdateRoleRntitlement
{
    public class UpdateRoleEntitlementCommandHandler : IRequestHandler<UpdateRoleEntitlementCommand, ApiResponseDTO<bool>>
    {
     private readonly IRoleEntitlementCommandRepository _roleEntitlementCommanderepository;
     private readonly IRoleEntitlementQueryRepository _roleEntitlementQueryrepository;
     private readonly IMapper _mapper;
     private readonly IMediator _mediator; 
    private readonly ILogger<UpdateRoleEntitlementCommandHandler> _logger;


    public UpdateRoleEntitlementCommandHandler(IRoleEntitlementCommandRepository roleEntitlementCommanderepository, IRoleEntitlementQueryRepository roleEntitlementQueryrepository,IMapper mapper, IMediator mediator,ILogger<UpdateRoleEntitlementCommandHandler> logger)
    {
        _roleEntitlementCommanderepository = roleEntitlementCommanderepository;
        _roleEntitlementQueryrepository = roleEntitlementQueryrepository;
        _mapper = mapper;
        _mediator = mediator;    
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    }

    public async Task<ApiResponseDTO<bool>> Handle(UpdateRoleEntitlementCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            _logger.LogError("UpdateRoleEntitlementCommand request is null.");
            throw new ArgumentNullException(nameof(request));
        }
         _logger.LogInformation("Starting role entitlement update process for RoleName: {RoleName}", request.RoleName);

        // Validate role existence
        var role = await _roleEntitlementQueryrepository.GetRoleByNameAsync(request.RoleName, cancellationToken);
        if (role == null)
        {
            _logger.LogWarning("Role not found: {RoleName}", request.RoleName);
            return new ApiResponseDTO<bool>
            {
                IsSuccess = false,
                Message = "Role not found."
            };
        }
        // Fetch existing role entitlements
            var existingEntitlements = await _roleEntitlementQueryrepository.GetRoleEntitlementsByRoleNameAsync(request.RoleName, cancellationToken);
            if (existingEntitlements == null || !existingEntitlements.Any())
            {
                _logger.LogWarning("No existing role entitlements found for RoleName: {RoleName}", request.RoleName);
            }

        // Map the new entitlements
        var updatedEntitlements = request.ModuleMenus
            .SelectMany(moduleMenu => moduleMenu.Menus
                .Select(menu => 
                {
                    var entitlement = _mapper.Map<RoleEntitlement>(menu);
                    entitlement.UserRoleId = role.Id;
                    entitlement.ModuleId = moduleMenu.ModuleId;
                    return entitlement;
                }))
            .ToList();

        // Update or replace existing entitlements
        await _roleEntitlementCommanderepository.UpdateRoleEntitlementsAsync(role.Id, updatedEntitlements, cancellationToken);
                //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Update",
                    actionCode: role.RoleName,
                    actionName: role.RoleName,
                    details: $"RoleEntitlements for Role '{role.RoleName}' were updated.",
                    module:"RoleEntitlement"
                );
                await _mediator.Publish(domainEvent, cancellationToken);

            _logger.LogInformation("Successfully updated role entitlements for RoleName: {RoleName}", request.RoleName);

            return new ApiResponseDTO<bool>
            {
                IsSuccess = true,
                Message = "Role entitlements updated successfully.",
                Data = true
            };
    }
    }
}