using Core.Application.Common.Interfaces;
using Core.Application.UserRole.Queries.GetRole;
using Core.Domain.Entities;
using MediatR;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.UserRole.Commands.UpdateRole
{
    public class UpdateRoleCommandHandler  : IRequestHandler<UpdateRoleCommand ,UserRoleDto>  
    {


        public readonly IUserRoleRepository _IUserRoleRepository;
         private readonly IMapper _Imapper;
         public readonly IUserRoleRepository _roleRepository;
          public UpdateRoleCommandHandler(IUserRoleRepository roleRepository ,IMapper mapper)
        {
            _IUserRoleRepository = roleRepository;
            _Imapper = mapper;
        }

        // public async Task<int>Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        // {
        //     var UpdateRoleEntity = new Core.Domain.Entities.UserRole()
        //     {

        //         Id=request.Id,
        //         RoleName =request.RoleName,
        //         Description =request.Description,
        //         CompanyId =request.CompanyId,
        //         IsActive =request.IsActive
        //     };
        //     return await _roleRepository.UpdateAsync(request.Id,UpdateRoleEntity);
        
        // }

         public async Task<UserRoleDto> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
       {

     
            var userrole = _Imapper.Map<Core.Domain.Entities.UserRole>(request);
            await _IUserRoleRepository.UpdateAsync(request.Id, userrole);
            var userroleDto = _Imapper.Map<UserRoleDto>(userrole);
          
            return userroleDto;

       }
        


    }
}