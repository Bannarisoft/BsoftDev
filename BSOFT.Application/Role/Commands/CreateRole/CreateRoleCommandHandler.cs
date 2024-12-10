using BSOFT.Application.Role.Queries.GetRole;
using BSOFT.Domain.Entities;
using BSOFT.Domain.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSOFT.Application.Role.Commands.CreateRole
{
    public class CreateRoleCommandHandler :IRequestHandler<CreateRoleCommand, RoleVm >
    {
        
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;

          public CreateRoleCommandHandler(IRoleRepository roleRepository,IMapper mapper)
        {
             _roleRepository=roleRepository;
            _mapper=mapper;
        }

        public async Task<RoleVm>Handle(CreateRoleCommand request,CancellationToken cancellationToken)
        {
            var roleEntity=new BSOFT.Domain.Entities.Role
            {
            Name=request.Name,
            Description =request.Description,
            CoId=request.CoId,            
            IsActive=request.IsActive,

            CreatedBy=request.CreatedBy,
            CreatedAt=request.CreatedAt,
            CreatedByName=request.CreatedByName,
            CreatedIP=request.CreatedIP,

            ModifiedBy=request.ModifiedBy,
            ModifiedAt=request.ModifiedAt,
            ModifiedByName =request.ModifiedByName,
            ModifiedIP=request.ModifiedIP
            };
             var result=await _roleRepository.CreateAsync(roleEntity);
            return _mapper.Map<RoleVm>(result);
         }



    }
}