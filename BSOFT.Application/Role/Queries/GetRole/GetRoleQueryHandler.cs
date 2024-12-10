using AutoMapper;
using BSOFT.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSOFT.Application.Role.Queries.GetRole
{
    public class GetRoleQueryHandler :IRequestHandler<GetRoleQuery,List<RoleVm>>
    {
     private readonly IRoleRepository _roleRepository;
     private readonly IMapper _mapper;
   


       public GetRoleQueryHandler(IRoleRepository roleRepository,IMapper mapper)
        {
            _roleRepository=roleRepository;
            _mapper =mapper;
        }

        public async Task<List<RoleVm>> Handle(GetRoleQuery request ,CancellationToken cancellationToken )
        {
            Console.WriteLine("Hello Handler");
            var role=await _roleRepository.GetAllRoleAsync();
            var roleList= _mapper.Map<List<RoleVm>>(role);
            return roleList;
        }
  }
}