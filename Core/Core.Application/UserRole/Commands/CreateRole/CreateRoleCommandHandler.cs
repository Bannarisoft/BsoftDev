using Core.Application.UserRole.Queries.GetRole;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.UserRole.Commands.CreateRole
{
    public class CreateRoleCommandHandler :IRequestHandler<CreateRoleCommand, UserRoleDto>
    {
        
        private readonly IUserRoleRepository _roleRepository;
        private readonly IMapper _mapper;

          public CreateRoleCommandHandler(IUserRoleRepository roleRepository,IMapper mapper)
        {
             _roleRepository=roleRepository;
            _mapper=mapper;
        }

        public async Task<UserRoleDto>Handle(CreateRoleCommand request,CancellationToken cancellationToken)
        {
            // var roleEntity=new Core.Domain.Entities.UserRole
            // {
            // RoleName=request.RoleName,
            // Description =request.Description,
            // CompanyId=request.CompanyId,            
            // IsActive=request.IsActive
           
            // };
            //  var result=await _roleRepository.CreateAsync(roleEntity);
            // return _mapper.Map<UserRoleDto>(result);
             
               var roleEntity = _mapper.Map<Core.Domain.Entities.UserRole>(request);
          //  departmentEntity.Id = ID; // Assign the new GUID to the user entity

            // Save the user to the repository
            var createdUserRole = await _roleRepository.CreateAsync(roleEntity);
            
            if (createdUserRole == null)
            {
                throw new InvalidOperationException("Failed to create user");
            }

            // Map the created User entity to UserDto
            return _mapper.Map<UserRoleDto>(createdUserRole);


         }



    }
}