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
using static Core.Domain.Enums.Common.Enums;

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
        
        var roleId = request.RoleId;

                     var roleModules = request.RoleModules.Select(dto => {
                         var entity = _mapper.Map<RoleModule>(dto);
                         entity.RoleId = roleId;  
                         return entity;
                     }).ToList();

                     var roleParents = request.RoleParents.Select(dto => {
                         var entity = _mapper.Map<RoleParent>(dto);
                         entity.RoleId = roleId; 
                         return entity;
                     }).ToList();

                     var roleChildren = request.RoleChildren.Select(dto => {
                         var entity = _mapper.Map<RoleChild>(dto);
                         entity.RoleId = roleId; 
                         return entity;
                     }).ToList();

                     var roleMenuPrivileges = request.RoleMenuPrivileges.Select(dto => {
                         var entity = _mapper.Map<RoleMenuPrivileges>(dto);
                         entity.RoleId = roleId; 
                         return entity;
                     }).ToList();
            
            var  role = await _roleEntitlementCommanderepository.UpdateRoleEntitlementsAsync(request.RoleId,roleModules,roleParents,roleChildren,roleMenuPrivileges, cancellationToken);

        if (!role)
        {
            return new ApiResponseDTO<bool>
            {
                IsSuccess = false,
                Message = "Role entitlements update failed."
            };
        }
        var domainEvent = new AuditLogsDomainEvent(
            actionDetail: "Update",
            actionCode: "RoleEntitlement",
            actionName: "Update",
            details: $"RoleEntitlements for Role '{request.RoleId}' were updated.",
            module: "RoleEntitlement"
        );
        await _mediator.Publish(domainEvent, cancellationToken);

        return new ApiResponseDTO<bool>
        {
            IsSuccess = true,
            Message = "Role entitlements updated successfully."
        };
  
    }
    }
}