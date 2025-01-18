using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AdminSecuritySettings.Queries.GetAdminSecuritySettings;
using Core.Application.Common;
using Core.Application.Common.Interfaces.IAdminSecuritySettings;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AdminSecuritySettings.Queries.GetAdminSecuritySettingsById
{
    public class GetAdminSecuritySettingsByIdQueryHandler :IRequestHandler<GetAdminSecuritySettingsByIdQuery, Result<AdminSecuritySettingsDto>>
    {

    
          private readonly IAdminSecuritySettingsQueryRepository _IAdminSecuritySettingsQueryRepository;        
        private readonly IMapper _mapper;
         private readonly IMediator _mediator;

    

    public GetAdminSecuritySettingsByIdQueryHandler(IAdminSecuritySettingsQueryRepository  adminSecuritySettingsQueryRepository,IMapper mapper , IMediator mediator)
         {
            _IAdminSecuritySettingsQueryRepository = adminSecuritySettingsQueryRepository;
            _mapper =mapper;
            _mediator = mediator;
        } 

           public async Task<Result<AdminSecuritySettingsDto>> Handle(GetAdminSecuritySettingsByIdQuery request, CancellationToken cancellationToken)
        {
                  var adminsettings = await _IAdminSecuritySettingsQueryRepository.GetAdminSecuritySettingsByIdAsync(request.AdminsettingsId);
                if (adminsettings == null)
                {
                    return Result<AdminSecuritySettingsDto>.Failure($"Admin Security Settings with ID {request.AdminsettingsId} not found.");
                }
                
                var adminsettingDto = _mapper.Map<AdminSecuritySettingsDto>(adminsettings);
                  
                //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: adminsettingDto.Id.ToString(),        
                    actionName: adminsettingDto.Id.ToString(),                
                    details: $"Admin Settings '{adminsettingDto.Id}' was created.",
                    module:"Admin Security Settings"
                );

                await _mediator.Publish(domainEvent, cancellationToken);
                return Result<AdminSecuritySettingsDto>.Success(adminsettingDto);

          
        }


    }
}