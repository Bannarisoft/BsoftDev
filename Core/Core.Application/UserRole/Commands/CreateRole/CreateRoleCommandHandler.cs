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
    public class CreateRoleCommandHandler :IRequestHandler<CreateRoleCommand, UserRoleVm >
    {
        
        private readonly IUserRoleRepository _roleRepository;
        private readonly IMapper _mapper;

          public CreateRoleCommandHandler(IUserRoleRepository roleRepository,IMapper mapper)
        {
             _roleRepository=roleRepository;
            _mapper=mapper;
        }

        public async Task<UserRoleVm>Handle(CreateRoleCommand request,CancellationToken cancellationToken)
        {
            var roleEntity=new Core.Domain.Entities.UserRole
            {
            RoleName=request.RoleName,
            Description =request.Description,
            CompanyId=request.CompanyId,            
            IsActive=request.IsActive
           
            };
             var result=await _roleRepository.CreateAsync(roleEntity);
            return _mapper.Map<UserRoleVm>(result);
         }



    }
}