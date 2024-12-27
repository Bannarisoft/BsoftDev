using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.UserRole.Commands.UpdateRole
{
    public class UpdateRoleCommandHandler  : IRequestHandler<UpdateRoleCommand ,int>  
    {

         public readonly IUserRoleRepository _roleRepository;
          public UpdateRoleCommandHandler(IUserRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<int>Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var UpdateRoleEntity = new Core.Domain.Entities.UserRole()
            {

                Id=request.Id,
                RoleName =request.RoleName,
                Description =request.Description,
                CompanyId =request.CompanyId,
                IsActive =request.IsActive
            };
            return await _roleRepository.UpdateAsync(request.Id,UpdateRoleEntity);
        
        }
        


    }
}