using MediatR;
using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IUserRole;

namespace Core.Application.UserRole.Commands.DeleteRole
{
    public class DeleteRoleCommandHandler  :IRequestHandler<DeleteRoleCommand ,int>
    {
    
        private readonly IUserRoleCommandRepository _IuserroleRepository;  
        private readonly IMapper _mapper;
      
      public DeleteRoleCommandHandler (IUserRoleCommandRepository roleRepository , IMapper mapper)
      {
        _IuserroleRepository =roleRepository;
         _mapper = mapper;
      }

       public async Task<int>Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
      {       
          // Map the command to the UserRole entity
            var updatedRole = _mapper.Map<Core.Domain.Entities.UserRole>(request);

            // Ensure the IsActive property is correctly updated
            updatedRole.IsActive = request.IsActive;

            // Pass the entity to the repository for deletion or updating
            return await _IuserroleRepository.DeleteAsync(request.UserRoleId, updatedRole);           
      }


    }
}