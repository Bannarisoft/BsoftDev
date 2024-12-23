using BSOFT.Application.UserRole.Queries.GetRole;
using BSOFT.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace BSOFT.Application.UserRole.Queries.GetRoleById
{
    public class GetRoleByIdQueryHandler :IRequestHandler<GetRoleByIdQuery,UserRoleVm>
    {

        private readonly IUserRoleRepository _roleRepository;
        private readonly IMapper _mapper;
          public GetRoleByIdQueryHandler(IUserRoleRepository roleRepository, IMapper mapper)
          {
            _roleRepository = roleRepository;
            _mapper=mapper;
          }

          public async Task<UserRoleVm> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
          {
            var userrole =await _roleRepository.GetByIdAsync(request.Id);
            return _mapper.Map<UserRoleVm>(userrole);
          }

        
    }
}