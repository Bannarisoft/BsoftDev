using BSOFT.Application.Common.Interfaces;
using BSOFT.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOFT.Application.Role.Commands.UpdateRole
{
    public class UpdateRoleCommandHandler  : IRequestHandler<UpdateRoleCommand ,int>  
    {

         public readonly IRoleRepository _roleRepository;
          public UpdateRoleCommandHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<int>Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var UpdateRoleEntity = new BSOFT.Domain.Entities.Role()
            {

                RoleId=request.RoleId,
                Name =request.Name,
                Description =request.Description,
                CoId =request.CoId,
                IsActive =request.IsActive,

                ModifiedBy     =request.ModifiedBy,
                ModifiedAt  =request.ModifiedAt,
                ModifiedByName=request.ModifiedByName,
                ModifiedIP=request.ModifiedIP

            };
            return await _roleRepository.UpdateAsync(request.RoleId,UpdateRoleEntity);
        
        }
        


    }
}