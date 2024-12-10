using AutoMapper;
using BSOFT.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSOFT.Application.RoleEntitlement.Queries.GetUsers
{
    public class GetAllRoleEntitlementsQueryHandler : IRequestHandler<GetAllRoleEntitlementsQuery, List<RoleEntitlementDto>>
    {
    
    private readonly IRoleEntitlementRepository _repository;
    private readonly IMapper _mapper;

    public GetAllRoleEntitlementsQueryHandler(IRoleEntitlementRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<RoleEntitlementDto>> Handle(GetAllRoleEntitlementsQuery request, CancellationToken cancellationToken)
    {
        var roleEntitlements = await _repository.GetAllAsync();
        return _mapper.Map<List<RoleEntitlementDto>>(roleEntitlements);
    }
    }
}