using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Core.Application.AdminSecuritySettings.Queries.GetAdminSecuritySettings;
using Core.Application.Common.Interfaces.IAdminSecuritySettings;

namespace Core.Application.AdminSecuritySettings.Queries.GetAdminSecuritySettings
{
    public class GetAdminSecuritySettingsQueryHandler :IRequestHandler<GetAdminSecuritySettingsQuery,List<AdminSecuritySettingsDto>>
    {
           private readonly IAdminSecuritySettingsQueryRepository   _adminSecuritySettingsQueryRepository;
        private readonly IMapper _mapper; 


         public GetAdminSecuritySettingsQueryHandler(IAdminSecuritySettingsQueryRepository adminSecuritySettingsQueryRepository,IMapper mapper)
        {
            _mapper =mapper;
            _adminSecuritySettingsQueryRepository = adminSecuritySettingsQueryRepository;                
        }

         public async Task<List<AdminSecuritySettingsDto>>Handle(GetAdminSecuritySettingsQuery request ,CancellationToken cancellationToken )
        {
           /*  const string query = @"SELECT  * FROM AppData.Department";
            var department = await _dbConnection.QueryAsync<DepartmentDto>(query);
           return department.AsList(); */
          
              var result = await _adminSecuritySettingsQueryRepository.GetAllAdminSecuritySettingsAsync();
                Console.WriteLine(result.Count());
                //return _mapper.Map<List<DivisionDTO>>(result);
                return _mapper.Map<List<AdminSecuritySettingsDto>>(result);  
                
           
        }



    }
}