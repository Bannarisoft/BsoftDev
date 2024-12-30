using Core.Domain.Entities;
using Core.Application.Common.Interfaces;
using FluentValidation;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.RoleEntitlements.Commands.CreateRoleEntitlement
{
    public class CreateRoleEntitlementCommandHandler : IRequestHandler<CreateRoleEntitlementCommand, int>
    {
        private readonly IRoleEntitlementRepository _repository;
        private readonly IMapper _mapper;

        public CreateRoleEntitlementCommandHandler(IRoleEntitlementRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<int> Handle(CreateRoleEntitlementCommand request, CancellationToken cancellationToken)
        {
        // Validate the role
        var role = await _repository.GetRoleByNameAsync(request.RoleName, cancellationToken);
        if (role == null)
        {
            throw new InvalidOperationException("Role not found.");
        }
            // Validate Modules and Menus
           foreach (var moduleMenu in request.ModuleMenus)
    {
        if (moduleMenu.ModuleId <= 0)
        {
            throw new ValidationException("Module ID must be greater than 0.");
        }

        var moduleExists = await _repository.ModuleExistsAsync(moduleMenu.ModuleId, cancellationToken);
        if (!moduleExists)
        {
            throw new ValidationException($"Module with ID '{moduleMenu.ModuleId}' does not exist.");
        }

        foreach (var menu in moduleMenu.Menus)
        {
            if (menu.MenuId <= 0)
            {
                throw new ValidationException("Menu ID must be greater than 0.");
            }

            var menuExists = await _repository.MenuExistsAsync(menu.MenuId, cancellationToken);
            if (!menuExists)
            {
                throw new ValidationException($"Menu with ID '{menu.MenuId}' does not exist.");
            }
        }
        }
        // Map ModuleMenuPermissionDto to RoleEntitlement
        var roleEntitlements = request.ModuleMenus
            .SelectMany(moduleMenu => moduleMenu.Menus
                .Select(menu => 
                {
                    var entitlement = _mapper.Map<RoleEntitlement>(menu);
                    entitlement.RoleId = role.Id;
                    entitlement.ModuleId = moduleMenu.ModuleId;
                    return entitlement;
                }))
            .ToList();

        // Save RoleEntitlements
        await _repository.AddRoleEntitlementsAsync(roleEntitlements, cancellationToken);

        return roleEntitlements.Count;
        }

    }

}