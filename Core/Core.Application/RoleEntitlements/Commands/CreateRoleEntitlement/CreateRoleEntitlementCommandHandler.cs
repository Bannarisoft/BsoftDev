using Core.Domain.Entities;
using Core.Application.Common.Interfaces;
using FluentValidation;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IRoleEntitlement;
using Core.Domain.Events;
using Core.Application.Common.HttpResponse;
using Microsoft.Extensions.Logging;
using static Core.Domain.Enums.Common.Enums;

namespace Core.Application.RoleEntitlements.Commands.CreateRoleEntitlement
{
    public class CreateRoleEntitlementCommandHandler : IRequestHandler<CreateRoleEntitlementCommand, ApiResponseDTO<int>>
    {
        private readonly IRoleEntitlementCommandRepository _roleEntitlementCommandrepository;
        private readonly IRoleEntitlementQueryRepository _roleEntitlementQueryrepository;

        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 
        private readonly ILogger<CreateRoleEntitlementCommandHandler> _logger;



        public CreateRoleEntitlementCommandHandler(IRoleEntitlementCommandRepository roleEntitlementCommandrepository,IRoleEntitlementQueryRepository roleEntitlementQueryrepository, IMapper mapper, IMediator mediator,ILogger<CreateRoleEntitlementCommandHandler> logger)
        {
            _roleEntitlementCommandrepository = roleEntitlementCommandrepository;
            _roleEntitlementQueryrepository = roleEntitlementQueryrepository;
            _mapper = mapper;
            _mediator = mediator;    
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        }

        public async Task<ApiResponseDTO<int>> Handle(CreateRoleEntitlementCommand request, CancellationToken cancellationToken)
        {

            if (request == null)
                {
                    _logger.LogError("CreateRoleEntitlementCommand request is null.");
                    throw new ArgumentNullException(nameof(request));
                }

                _logger.LogInformation("Starting role entitlement creation process for RoleName: {RoleName}", request.RoleName);
                
                // Validate the role
                var role = await _roleEntitlementQueryrepository.GetRoleByNameAsync(request.RoleName, cancellationToken);
                if (role == null)
                {
                    throw new ValidationException("Role not found.");
                }

                // Validate Modules and Menus
                foreach (var moduleMenu in request.ModuleMenus)
                {
                    if (moduleMenu.ModuleId <= 0)
                    {
                        throw new ValidationException("Module ID must be greater than 0.");
                    }

                    var moduleExists = await _roleEntitlementCommandrepository.ModuleExistsAsync(moduleMenu.ModuleId, cancellationToken);
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

                        var menuExists = await _roleEntitlementCommandrepository.MenuExistsAsync(menu.MenuId, cancellationToken);
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
                        entitlement.UserRoleId = role.Id;
                        entitlement.ModuleId = moduleMenu.ModuleId;
                        return entitlement;
                    }))
                .ToList();

                // Fetch Existing RoleEntitlements to Prevent Duplicates
                var existingEntitlements = await _roleEntitlementQueryrepository.GetExistingRoleEntitlementsAsync(
                    roleEntitlements.Select(e => e.UserRoleId).Distinct().ToList(),
                    roleEntitlements.Select(e => e.ModuleId).Distinct().ToList(),
                    roleEntitlements.Select(e => e.MenuId).Distinct().ToList(),
                    cancellationToken
                );

                // Convert to HashSet for fast lookup
                var existingSet = new HashSet<(int UserRoleId, int ModuleId, int MenuId)>(
                    existingEntitlements.Select(e => (e.UserRoleId, e.ModuleId, e.MenuId))
                );

                // Filter out already existing RoleEntitlements
                var newEntitlements = roleEntitlements
                    .Where(re => !existingSet.Contains((re.UserRoleId, re.ModuleId, re.MenuId)))
                    .ToList();

                if (newEntitlements.Any())
                {
                    await _roleEntitlementCommandrepository.AddRoleEntitlementsAsync(newEntitlements, cancellationToken);

                    // Domain Event for Audit Logs
                    var domainEvent = new AuditLogsDomainEvent(
                        actionDetail: "Create",
                        actionCode: role.RoleName,
                        actionName: role.RoleName,
                        details: $"RoleEntitlement '{role.RoleName}' was created. RoleName: {role.RoleName}",
                        module: "RoleEntitlement"
                    );
                    await _mediator.Publish(domainEvent, cancellationToken);
                    _logger.LogInformation("Role entitlements successfully created for RoleName: {RoleName}", request.RoleName);

                    return new ApiResponseDTO<int>
                    {
                        IsSuccess = true,
                        Message = "Role entitlements created successfully.",
                        Data = newEntitlements.Count
                    };
                }

                _logger.LogInformation("No new role entitlements were added. All records already exist for RoleName: {RoleName}", request.RoleName);
                return new ApiResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = "No new role entitlements were added. All records already exist.",
                    Data = 0
                };
        //     if (request == null)
        //     {
        //         _logger.LogError("CreateRoleEntitlementCommand request is null.");
        //         throw new ArgumentNullException(nameof(request));
        //     }

        //     _logger.LogInformation("Starting role entitlement creation process for RoleName: {RoleName}", request.RoleName);
            
        //     // Validate the role
        //     var role = await _repository.GetRoleByNameAsync(request.RoleName, cancellationToken);
        //     if (role == null)
        //     {
        //         throw new ValidationException("Role not found.");
        //     }
        //    // Validate Modules and Menus
        //     foreach (var moduleMenu in request.ModuleMenus)
        //     {
        //         if (moduleMenu.ModuleId <= 0)
        //         {
        //             throw new ValidationException("Module ID must be greater than 0.");
        //         }

        //         var moduleExists = await _repository.ModuleExistsAsync(moduleMenu.ModuleId, cancellationToken);
        //         if (!moduleExists)
        //         {
        //             throw new ValidationException($"Module with ID '{moduleMenu.ModuleId}' does not exist.");
        //         }

        //         foreach (var menu in moduleMenu.Menus)
        //         {
        //             if (menu.MenuId <= 0)
        //             {
        //                 throw new ValidationException("Menu ID must be greater than 0.");
        //             }

        //             var menuExists = await _repository.MenuExistsAsync(menu.MenuId, cancellationToken);
        //             if (!menuExists)
        //             {
        //                 throw new ValidationException($"Menu with ID '{menu.MenuId}' does not exist.");
        //             }
        //         }
        //     }
        // // Map ModuleMenuPermissionDto to RoleEntitlement
        //     var roleEntitlements = request.ModuleMenus
        //         .SelectMany(moduleMenu => moduleMenu.Menus
        //         .Select(menu => 
        //         {
        //             var entitlement = _mapper.Map<RoleEntitlement>(menu);
        //             entitlement.UserRoleId = role.Id;
        //             entitlement.ModuleId = moduleMenu.ModuleId;
        //             return entitlement;
        //         }))
        //     .ToList();

        // // Save RoleEntitlements
        //     await _repository.AddRoleEntitlementsAsync(roleEntitlements, cancellationToken);
        // //Domain Event
        //         var domainEvent = new AuditLogsDomainEvent(
        //             actionDetail: "Create",
        //             actionCode: role.RoleName,
        //             actionName: role.RoleName,
        //             details: $"RoleEntitlement '{role.RoleName}' was created. RoleName: {role.RoleName}",
        //             module:"RoleEntitlement"
        //         );
        //     await _mediator.Publish(domainEvent, cancellationToken);
        //     _logger.LogInformation("Role entitlements successfully created for RoleName: {RoleName}", request.RoleName);

        //     return new ApiResponseDTO<int>
        //     {
        //         IsSuccess = true,
        //         Message = "Role entitlements created successfully.",
        //         Data = roleEntitlements.Count
        //     };
        }

    }

}