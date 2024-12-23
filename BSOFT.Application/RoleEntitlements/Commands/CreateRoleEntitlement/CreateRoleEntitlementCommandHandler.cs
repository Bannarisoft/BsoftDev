using FluentValidation;
using BSOFT.Domain.Entities;
using BSOFT.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOFT.Application.RoleEntitlements.Commands.CreateRoleEntitlement
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
        if (string.IsNullOrEmpty(request.RoleName) || !request.MenuPermissions.Any())
        {
            throw new ValidationException("Role Name and Menu Permissions are mandatory.");
        }

        var roleEntitlements = _mapper.Map<List<RoleEntitlement>>(request.MenuPermissions);
        roleEntitlements.ForEach(e => e.RoleName = request.RoleName);

        await _repository.AddRoleEntitlementsAsync(roleEntitlements);
        return roleEntitlements.Count;
    }
        
    }
}