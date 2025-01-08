
using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Entities;
using System.Data;
using Core.Application.Common.Interfaces.IUserRole;


namespace Core.Application.UserRole.Queries.GetRole
{
    public class GetRoleQueryHandler :IRequestHandler<GetRoleQuery,List<UserRoleDto>>
    {
       
     private readonly IUserRoleQueryRepository _userRoleRepository;
     private readonly IMapper _mapper;
   


       public GetRoleQueryHandler(IUserRoleQueryRepository userRoleRepository, IMapper mapper)
        {
            _userRoleRepository = userRoleRepository;
            _mapper =mapper;
        }

        public async Task<List<UserRoleDto>> Handle(GetRoleQuery request ,CancellationToken cancellationToken )
        {
        /*     const string query = @"SELECT  * FROM AppSecurity.UserRole";
             var department = await _dbConnection.QueryAsync<UserRoleDto>(query);
           return department.AsList();
 */
           var users = await _userRoleRepository.GetAllRoleAsync();
            var userList = _mapper.Map<List<UserRoleDto>>(users);
            return userList;
        }



  }
}