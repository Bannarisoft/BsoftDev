using AutoMapper;
using BSOFT.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSOFT.Application.RoleEntitlements.Queries.GetRoles
{
    public class GetAllRoleEntitlementsQueryHandler : IRequestHandler<GetAllRoleEntitlementsQuery, List<RoleEntitlementVm>>
    {
    
    private readonly IRoleEntitlementRepository _repository;
    private readonly IMapper _mapper;

    public GetAllRoleEntitlementsQueryHandler(IRoleEntitlementRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<RoleEntitlementVm>> Handle(GetAllRoleEntitlementsQuery request, CancellationToken cancellationToken)
    {
        var roleEntitlements = await _repository.GetAllAsync();
        return _mapper.Map<List<RoleEntitlementVm>>(roleEntitlements);
    }
    }
}