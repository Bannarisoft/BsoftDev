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

namespace Core.Application.RoleEntitlements.Commands.UpdateRoleRntitlement
{
    public class UpdateRoleEntitlementCommandHandler : IRequestHandler<UpdateRoleEntitlementCommand, bool>
    {
     private readonly IRoleEntitlementCommandRepository _repository;
     private readonly IMapper _mapper;
     private readonly IMediator _mediator; 


    public UpdateRoleEntitlementCommandHandler(IRoleEntitlementCommandRepository repository, IMapper mapper, IMediator mediator)
    {
        _repository = repository;
        _mapper = mapper;
        _mediator = mediator;    

    }

    public async Task<bool> Handle(UpdateRoleEntitlementCommand request, CancellationToken cancellationToken)
    {
        // Validate role existence
        var role = await _repository.GetRoleByNameAsync(request.RoleName, cancellationToken);
        if (role == null)
        {
            throw new InvalidOperationException("Role not found.");
        }

        // Fetch existing role entitlements
        var existingEntitlements = await _repository.GetRoleEntitlementsByRoleNameAsync(request.RoleName, cancellationToken);

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
        await _repository.UpdateRoleEntitlementsAsync(role.Id, updatedEntitlements, cancellationToken);
                //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Create",
                    actionCode: role.RoleName,
                    actionName: role.RoleName,
                    details: $"RoleEntitlement '{role.RoleName}' was updated. RoleName: {role.RoleName}",
                    module:"RoleEntitlement"
                );
                await _mediator.Publish(domainEvent, cancellationToken);

        return true;
    }
    }
}