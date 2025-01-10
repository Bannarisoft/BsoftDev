using Core.Application.Common.Interfaces;
using Core.Application.UserRole.Queries.GetRole;
using Core.Domain.Entities;
using MediatR;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IUserRole;

namespace Core.Application.UserRole.Commands.UpdateRole
{
    public class UpdateRoleCommandHandler  : IRequestHandler<UpdateRoleCommand ,UserRoleDto>  
    {


        public readonly IUserRoleCommandRepository _IUserRoleRepository;
        private readonly IMapper _Imapper;
        public UpdateRoleCommandHandler(IUserRoleCommandRepository roleRepository ,IMapper mapper)
        {
            _IUserRoleRepository = roleRepository;
            _Imapper = mapper;
        }

        public async Task<UserRoleDto> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            // Map the updated data from the request to the domain entity
            var updatedRole = _Imapper.Map<Core.Domain.Entities.UserRole>(request);

            // Update the role in the repository
            await _IUserRoleRepository.UpdateAsync(request.Id, updatedRole);

            // Map the updated entity back to a DTO
            var userRoleDto = _Imapper.Map<UserRoleDto>(updatedRole);

            return userRoleDto;

       }
        


    }
}