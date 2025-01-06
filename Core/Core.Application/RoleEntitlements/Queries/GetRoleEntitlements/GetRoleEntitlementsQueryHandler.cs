using AutoMapper;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.IRoleEntitlement;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.RoleEntitlements.Queries.GetRoleEntitlements
{
    public class GetRoleEntitlementsQueryHandler : IRequestHandler<GetRoleEntitlementsQuery, List<RoleEntitlementDto>>
    {
        private readonly IRoleEntitlementQueryRepository _repository;
        private readonly IMapper _mapper;
        
    public GetRoleEntitlementsQueryHandler(IRoleEntitlementQueryRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<RoleEntitlementDto>> Handle(GetRoleEntitlementsQuery request, CancellationToken cancellationToken)
    {
        // Fetch role entitlements from the repository
        var roleEntitlements = await _repository.GetRoleEntitlementsByRoleNameAsync(request.RoleName, cancellationToken);

        // Map the result to RoleEntitlementDto
        return _mapper.Map<List<RoleEntitlementDto>>(roleEntitlements);
    }

    }
}