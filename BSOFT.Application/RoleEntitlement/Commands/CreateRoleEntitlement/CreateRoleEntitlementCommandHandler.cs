using BSOFT.Domain.Entities;
using BSOFT.Domain.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BSOFT.Application.RoleEntitlement.Commands.CreateRoleEntitlement
{
public class CreateRoleEntitlementCommandHandler : IRequestHandler<CreateRoleEntitlementCommand, int>
{
    private readonly IMapper _mapper;
    private readonly IRoleEntitlementRepository _repository;

    public CreateRoleEntitlementCommandHandler(IMapper mapper, IRoleEntitlementRepository repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    public async Task<int> Handle(CreateRoleEntitlementCommand request, CancellationToken cancellationToken)
    {
        var roleEntitlement = _mapper.Map<RoleEntitlement>(request.RoleEntitlementDto);
        roleEntitlement.CreatedAt = DateTime.UtcNow;
        roleEntitlement.CreatedBy = "System"; // Replace with the logged-in user

        await _repository.AddAsync(roleEntitlement);

        return roleEntitlement.Id;
    }
}

}