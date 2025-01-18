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

namespace Core.Application.AdminSecuritySettings.Queries.GetAdminSecuritySettings
{
    public class GetAdminSecuritySettingsQueryHandler :IRequestHandler<GetAdminSecuritySettingsQuery,Result<List<AdminSecuritySettingsDto>>>
    {
           private readonly IAdminSecuritySettingsQueryRepository   _adminSecuritySettingsQueryRepository;
        private readonly IMapper _mapper; 
           private readonly IMediator _mediator; 


         public GetAdminSecuritySettingsQueryHandler(IAdminSecuritySettingsQueryRepository adminSecuritySettingsQueryRepository,IMapper mapper, IMediator mediator)
        {
            _mapper =mapper;
            _adminSecuritySettingsQueryRepository = adminSecuritySettingsQueryRepository;         
             _mediator = mediator;  

        }

         public async Task<Result<List<AdminSecuritySettingsDto>>>Handle(GetAdminSecuritySettingsQuery request ,CancellationToken cancellationToken )
        {
      
                 var adminSecuritySettings = await _adminSecuritySettingsQueryRepository.GetAllAdminSecuritySettingsAsync();
             var adminSecuritySettingsList = _mapper.Map<List<AdminSecuritySettingsDto>>(adminSecuritySettings);

                 //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAll",
                    actionCode: "",        
                    actionName: "",
                    details: $"Admin Security Settings details was fetched.",
                    module:"AdminSecuritySettings"
                );

                  await _mediator.Publish(domainEvent, cancellationToken);
                return Result<List<AdminSecuritySettingsDto>>.Success(adminSecuritySettingsList);

           
        }


        



    }
}