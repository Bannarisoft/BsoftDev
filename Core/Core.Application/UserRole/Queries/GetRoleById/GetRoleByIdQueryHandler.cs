using Core.Application.UserRole.Queries.GetRole;
using Core.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Core.Application.Common.Interfaces.IUserRole;


namespace Core.Application.UserRole.Queries.GetRoleById
{
    public class GetRoleByIdQueryHandler :IRequestHandler<GetRoleByIdQuery,UserRoleDto>
    {
    private readonly IUserRoleQueryRepository _userRoleRepository;
     private readonly IMapper _mapper;
   
      
          public GetRoleByIdQueryHandler(IUserRoleQueryRepository userRoleRepository, IMapper mapper)
          {
            _userRoleRepository = userRoleRepository;
            _mapper =mapper;
        
          }

          public async Task<UserRoleDto> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
          {
             /* var query = "SELECT RoleName,Description,CompanyId,IsActive from  AppSecurity.UserRole WHERE Id = @Id";
        var userrole = await _dbConnection.QuerySingleOrDefaultAsync<UserRoleDto>(query, new { Id = request.Id });
        // Return null if the country is not found
        if (userrole == null)
          {
            return null;
          }

        // Map the country entity to a DTO
        return new UserRoleDto
         {
            Id = userrole.Id,
            CompanyId = userrole.CompanyId,
            RoleName = userrole.RoleName,
            Description = userrole.Description,
            IsActive = userrole.IsActive
          }; */

         var user = await _userRoleRepository.GetByIdAsync(request.Id);
          return _mapper.Map<UserRoleDto>(user);


          }

        
    }
}