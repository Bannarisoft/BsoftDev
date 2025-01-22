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
    public class GetAdminSecuritySettingsQueryHandler :IRequestHandler<GetAdminSecuritySettingsQuery,ApiResponseDTO<List<AdminSecuritySettingsDto>>>
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

         public async Task<ApiResponseDTO<List<AdminSecuritySettingsDto>>>Handle(GetAdminSecuritySettingsQuery request ,CancellationToken cancellationToken )
        {

              _logger.LogInformation("Handling GetAdminSecuritySettingsQuery to fetch admin security settings.");
             // Fetch admin security settings from the repository
                var adminSecuritySettings = await _adminSecuritySettingsQueryRepository.GetAllAdminSecuritySettingsAsync();

                // if (adminSecuritySettings == null || !adminSecuritySettings.Any())
                // {
                //     _logger.LogWarning("No admin security settings found.");
                //     return Result<List<AdminSecuritySettingsDto>>.Failure("No admin security settings found.");
                // }
                    if (adminSecuritySettings == null || !adminSecuritySettings.Any())
            {
               _logger.LogWarning("No adminSecuritySettings records found in the database. Total count: {Count}", adminSecuritySettings?.Count ?? 0);

                  return new ApiResponseDTO<List<AdminSecuritySettingsDto>> { IsSuccess = false, Message = "No Record Found" };
            }

                _logger.LogInformation("Admin security settings fetched successfully. Mapping to DTO.");

                // Map the result to DTO
                var adminSecuritySettingsList = _mapper.Map<List<AdminSecuritySettingsDto>>(adminSecuritySettings);

                // Publish domain event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAll",
                    actionCode: "",
                    actionName: "",
                    details: $"Admin Security Settings details were fetched.",
                    module: "AdminSecuritySettings"
                );
                 await _mediator.Publish(domainEvent, cancellationToken);
              
            _logger.LogInformation("Department {department} Listed successfully.", adminSecuritySettingsList.Count);
            return new ApiResponseDTO<List<AdminSecuritySettingsDto>> { IsSuccess = true, Message = "Success", Data = adminSecuritySettingsList };  
               
            
      
          

           
        }


        



    }
}