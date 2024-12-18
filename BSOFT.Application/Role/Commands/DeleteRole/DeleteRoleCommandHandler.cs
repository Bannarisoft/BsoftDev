using MediatR;
using BSOFT.Application.Common.Interfaces;
using BSOFT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOFT.Application.Role.Commands.DeleteRole
{
    public class DeleteRoleCommandHandler  :IRequestHandler<DeleteRoleCommand ,int>
    {
    
         private readonly IRoleRepository _roleRepository;  
      
      public DeleteRoleCommandHandler (IRoleRepository roleRepository)
      {
        _roleRepository =roleRepository;
      }

      public async Task<int>Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
      {
        return await _roleRepository.DeleteAsync(request.RoleId);
      }
    }
}