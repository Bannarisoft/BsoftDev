using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Exceptions;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMiscMaster;
using Core.Application.MiscMaster.Queries.GetMiscMasterById;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MiscMaster.Command.UpdateMiscMaster
{
    public class UpdateMiscMasterCommandHandler  : IRequestHandler<UpdateMiscMasterCommand, bool>
    {

         private readonly IMiscMasterCommandRepository _miscMasterCommandRepository;
        private readonly IMiscMasterQueryRepository _miscMasterQueryRepository;

         private readonly IMapper _imapper;
        private readonly IMediator _mediator;

         public UpdateMiscMasterCommandHandler(IMiscMasterCommandRepository miscMasterCommandRepository ,IMiscMasterQueryRepository miscMasterQueryRepository ,IMapper imapper, IMediator mediator)
        {
            _miscMasterCommandRepository =miscMasterCommandRepository;
            _imapper =imapper;
            _mediator = mediator;
            _miscMasterQueryRepository = miscMasterQueryRepository;
        }


           public async Task<bool> Handle(UpdateMiscMasterCommand request, CancellationToken cancellationToken)
        {
                          
                 var miscmaster  = _imapper.Map<Core.Domain.Entities.MiscMaster>(request);         
                var MiscMasterresult = await _miscMasterCommandRepository.UpdateAsync(request.Id, miscmaster);                

                    var domainEvent = new AuditLogsDomainEvent(
                        actionDetail: "Update",
                        actionCode: miscmaster.Code,
                        actionName: miscmaster.Description,
                        details: $"MiscMaster '{miscmaster.Id}' was updated.",
                        module:"MiscMaster"
                    );               
                    await _mediator.Publish(domainEvent, cancellationToken); 
              
               
                    return MiscMasterresult ? true : throw new ExceptionRules("MiscMaster updation failed.");
                
           
        }
       
    }
        
}