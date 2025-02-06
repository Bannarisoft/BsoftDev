using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Core.Application.AdminSecuritySettings.Queries.GetAdminSecuritySettings;
using Core.Application.Common.Interfaces.IAdminSecuritySettings;
using Core.Application.Common;
using Core.Domain.Events;
using Microsoft.Extensions.Logging;
using Core.Application.Common.HttpResponse;

namespace Core.Application.AdminSecuritySettings.Queries.GetAdminSecuritySettings
{
    public class GetAdminSecuritySettingsQueryHandler :IRequestHandler<GetAdminSecuritySettingsQuery,ApiResponseDTO<List<GetAdminSecuritySettingsDto>>>
    {
           private readonly IAdminSecuritySettingsQueryRepository   _adminSecuritySettingsQueryRepository;
        private readonly IMapper _mapper; 
           private readonly IMediator _mediator; 

        private readonly ILogger<GetAdminSecuritySettingsQueryHandler> _logger;


         public GetAdminSecuritySettingsQueryHandler(IAdminSecuritySettingsQueryRepository adminSecuritySettingsQueryRepository,IMapper mapper, IMediator mediator,ILogger<GetAdminSecuritySettingsQueryHandler> logger)
        {
            _mapper =mapper;
            _adminSecuritySettingsQueryRepository = adminSecuritySettingsQueryRepository;         
             _mediator = mediator;  

             _logger = logger;


        }

         public async Task<ApiResponseDTO<List<GetAdminSecuritySettingsDto>>>Handle(GetAdminSecuritySettingsQuery request ,CancellationToken cancellationToken )
        {

              _logger.LogInformation($"Handling GetAdminSecuritySettingsQuery to fetch admin security settings{request}");
             // Fetch admin security settings from the repository
                var adminSecuritySettings = await _adminSecuritySettingsQueryRepository.GetAllAdminSecuritySettingsAsync();
               
                    if (adminSecuritySettings is  null || !adminSecuritySettings.Any() || adminSecuritySettings.Count==0)
              {
               _logger.LogWarning($"No adminSecuritySettings records found in the database. Total count: { adminSecuritySettings?.Count ?? 0} ");

                  return new ApiResponseDTO<List<GetAdminSecuritySettingsDto>>                      
                     {
                         IsSuccess = false,
                         Message = "No entity found"
                     };
              }

                _logger.LogInformation("Admin security settings fetched successfully. Mapping to DTO.");

                // Map the result to DTO
                var adminSecuritySettingsList = _mapper.Map<List<GetAdminSecuritySettingsDto>>(adminSecuritySettings);

                // Publish domain event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAll",
                    actionCode: "",
                    actionName: "",
                    details: $"Admin Security Settings details were fetched.",
                    module: "AdminSecuritySettings"
                );
                 await _mediator.Publish(domainEvent, cancellationToken);
              
            _logger.LogInformation($"AdminSecuritySettings  {adminSecuritySettingsList.Count} Listed successfully.");
            return new ApiResponseDTO<List<GetAdminSecuritySettingsDto>> { IsSuccess = true, Message = "Success", Data = adminSecuritySettingsList };  
               
            
      
          

           
        }


        



    }
}