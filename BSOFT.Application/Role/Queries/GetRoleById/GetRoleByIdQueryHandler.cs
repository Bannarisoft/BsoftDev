using BSOFT.Application.Role.Queries.GetRole;
using BSOFT.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BSOFT.Application.Role.Queries.GetRoleById
{
    public class GetRoleByIdQueryHandler :IRequestHandler<GetRoleByIdQuery,RoleVm>
    {

        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
          public GetRoleByIdQueryHandler(IRoleRepository roleRepository, IMapper mapper)
          {
            _roleRepository = roleRepository;
            _mapper=mapper;
          }

          public async Task<RoleVm> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
          {
            var role =await _roleRepository.GetByIdAsync(request.RoleId);
            return _mapper.Map<RoleVm>(role);
          }

        
    }
}