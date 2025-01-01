using MediatR;
using Core.Application.Common.Interfaces;
using Core.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Application.UserRole.Commands.DeleteRole
{
    public class DeleteRoleCommandHandler  :IRequestHandler<DeleteRoleCommand ,int>
    {
    
         private readonly IUserRoleRepository _IuserroleRepository;  
             private readonly IMapper _mapper;
      
      public DeleteRoleCommandHandler (IUserRoleRepository roleRepository , IMapper mapper)
      {
        _IuserroleRepository =roleRepository;
         _mapper = mapper;
      }

       public async Task<int>Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
      {       
          var updatedRole = _mapper.Map<Core.Domain.Entities.UserRole>(request.roleStatusDto);
            return await _IuserroleRepository.DeleteAsync(request.Id, updatedRole);              
      }

      // public async Task<int>Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
      // {
      //   return await _IuserroleRepository.DeleteAsync(request.Id);
      // }
    }
}