using MediatR;
using BSOFT.Application.Common.Interfaces;
using BSOFT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOFT.Application.UserRole.Commands.DeleteRole
{
    public class DeleteRoleCommandHandler  :IRequestHandler<DeleteRoleCommand ,int>
    {
    
         private readonly IUserRoleRepository _IuserroleRepository;  
      
      public DeleteRoleCommandHandler (IUserRoleRepository roleRepository)
      {
        _IuserroleRepository =roleRepository;
      }

      public async Task<int>Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
      {
        return await _IuserroleRepository.DeleteAsync(request.Id);
      }
    }
}