using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using MediatR;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IRoleEntitlement;

namespace Core.Application.RoleEntitlements.Commands.UpdateRoleRntitlement
{
    public class UpdateRoleEntitlementCommandHandler : IRequestHandler<UpdateRoleEntitlementCommand, bool>
    {
     private readonly IRoleEntitlementCommandRepository _repository;
     private readonly IMapper _mapper;

    public UpdateRoleEntitlementCommandHandler(IRoleEntitlementCommandRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
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

        return true;
    }
    }
}