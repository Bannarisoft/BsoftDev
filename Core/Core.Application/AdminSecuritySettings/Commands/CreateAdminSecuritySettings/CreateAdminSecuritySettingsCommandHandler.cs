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

namespace Core.Application.AdminSecuritySettings.Commands.CreateAdminSecuritySettings
{
    public class CreateAdminSecuritySettingsCommandHandler  :IRequestHandler<CreateAdminSecuritySettingsCommand, Result<AdminSecuritySettingsDto >>
    {
        private readonly IAdminSecuritySettingsCommandRepository _adminSecuritySettingsCommandRepository;
        private readonly IMapper _mapper;
             
          private readonly IMediator _mediator; 

          public CreateAdminSecuritySettingsCommandHandler(IAdminSecuritySettingsCommandRepository adminSecuritySettingsCommandRepository,IMapper mapper,IMediator mediator)
        {
             _adminSecuritySettingsCommandRepository=adminSecuritySettingsCommandRepository;
            _mapper=mapper;
            _mediator=mediator;
        }

        public async Task<Result<AdminSecuritySettingsDto>> Handle(CreateAdminSecuritySettingsCommand request, CancellationToken cancellationToken)
        {            
       
           var adminsettingsEntity = _mapper.Map<Core.Domain.Entities.AdminSecuritySettings>(request);

             var result = await _adminSecuritySettingsCommandRepository.CreateAsync(adminsettingsEntity);
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: result.Id.ToString(),
                actionName: "",
                details: $"New Admin settings  was created. CountryCode: {result.Id}",
                module:"Admin Security Settings"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            
            var DeptDto = _mapper.Map<AdminSecuritySettingsDto>(result);
            return Result<AdminSecuritySettingsDto>.Success(DeptDto);
        }




    }


}