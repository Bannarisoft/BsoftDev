using MediatR;
using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.UserRole.Commands.DeleteRole
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