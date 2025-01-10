using Core.Application.UserRole.Queries.GetRole;
using Core.Domain.Entities;
using Core.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.IUserRole;

namespace Core.Application.UserRole.Commands.CreateRole
{
    public class CreateRoleCommandHandler :IRequestHandler<CreateRoleCommand, UserRoleDto>
    {
        
        private readonly IUserRoleCommandRepository _roleRepository;
        private readonly IMapper _mapper;

        public CreateRoleCommandHandler(IUserRoleCommandRepository roleRepository,IMapper mapper)
        {
             _roleRepository=roleRepository;
            _mapper=mapper;
        }

        public async Task<UserRoleDto>Handle(CreateRoleCommand request,CancellationToken cancellationToken)
        {          
            // Validate input
            if (string.IsNullOrWhiteSpace(request.RoleName))
            {
                throw new ArgumentException("RoleName cannot be null or empty.");
            }

            // Map to domain entity
            var roleEntity = _mapper.Map<Core.Domain.Entities.UserRole>(request);

            // Save to repository
            var createdUserRole = await _roleRepository.CreateAsync(roleEntity);

            if (createdUserRole == null)
            {
                throw new InvalidOperationException("Failed to create the role.");
            }

            // Map to DTO
            return _mapper.Map<UserRoleDto>(createdUserRole);


         }



    }
}