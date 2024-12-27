using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.UserRole.Queries.GetRole
{
    public class GetRoleQueryHandler :IRequestHandler<GetRoleQuery,List<UserRoleVm>>
    {
     private readonly IUserRoleRepository _IuserRoleRepository;
     private readonly IMapper _Imapper;
   


       public GetRoleQueryHandler(IUserRoleRepository userroleRepository,IMapper mapper)
        {
            _IuserRoleRepository=userroleRepository;
            _Imapper =mapper;
        }

        public async Task<List<UserRoleVm>> Handle(GetRoleQuery request ,CancellationToken cancellationToken )
        {
            Console.WriteLine("Hello Handler");
            var userrole=await _IuserRoleRepository.GetAllRoleAsync();
            var roleList= _Imapper.Map<List<UserRoleVm>>(userrole);
            return roleList;
        }
  }
}