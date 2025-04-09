using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using Core.Application.PreventiveSchedulers.Commands.CreatePreventiveScheduler;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.PreventiveSchedulers.Commands.UpdatePreventiveScheduler
{
    public class UpdatePreventiveSchedulerCommandHandler : IRequestHandler<UpdatePreventiveSchedulerCommand, ApiResponseDTO<bool>>
    {
        private readonly IPreventiveSchedulerCommand _preventiveSchedulerCommand;
        private readonly IMapper _imapper;
        private readonly IMediator _mediator;
        public UpdatePreventiveSchedulerCommandHandler(IPreventiveSchedulerCommand preventiveSchedulerCommand, IMapper imapper, IMediator mediator)
        {
            _preventiveSchedulerCommand = preventiveSchedulerCommand;
            _imapper = imapper;
            _mediator = mediator;
        }
        public async Task<ApiResponseDTO<bool>> Handle(UpdatePreventiveSchedulerCommand request, CancellationToken cancellationToken)
        {
            var preventiveScheduler  = _imapper.Map<PreventiveSchedulerHdr>(request);
         
                var preventiveSchedulerresult = await _preventiveSchedulerCommand.UpdateAsync(preventiveScheduler);

                
                    var domainEvent = new AuditLogsDomainEvent(
                        actionDetail: "Update",
                        actionCode: "update",
                        actionName: "Update Preventive Scheduler",
                        details: $"Update Preventive Scheduler",
                        module:"Preventive Scheduler"
                    );               
                    await _mediator.Publish(domainEvent, cancellationToken); 
              
                if(preventiveSchedulerresult)
                {
                    return new ApiResponseDTO<bool>
                    {
                        IsSuccess = true, 
                        Message = "Preventive Scheduler updated successfully."
                    };
                }

                return new ApiResponseDTO<bool>
                {
                    IsSuccess = false, 
                    Message = "Preventive Scheduler not updated."
                };
        }
    }
}