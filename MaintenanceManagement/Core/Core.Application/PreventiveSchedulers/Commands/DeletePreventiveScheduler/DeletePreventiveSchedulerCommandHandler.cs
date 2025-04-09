using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.PreventiveSchedulers.Commands.DeletePreventiveScheduler
{
    public class DeletePreventiveSchedulerCommandHandler : IRequestHandler<DeletePreventiveSchedulerCommand, ApiResponseDTO<bool>>
    {
        private readonly IPreventiveSchedulerCommand _preventiveSchedulerCommand;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        public DeletePreventiveSchedulerCommandHandler(IPreventiveSchedulerCommand preventiveSchedulerCommand, IMapper imapper, IMediator mediator)
        {
            _preventiveSchedulerCommand = preventiveSchedulerCommand;
            _mapper = imapper;
            _mediator = mediator;
        }
        public async Task<ApiResponseDTO<bool>> Handle(DeletePreventiveSchedulerCommand request, CancellationToken cancellationToken)
        {
             var preventiveScheduler  = _mapper.Map<PreventiveSchedulerHdr>(request);
            var preventiveSchedulerresult = await _preventiveSchedulerCommand.DeleteAsync(request.Id,preventiveScheduler);


                  //Domain Event  
                    var domainEvent = new AuditLogsDomainEvent(
                        actionDetail: "Delete",
                        actionCode: "delete",
                        actionName: "Delete Preventive Scheduler",
                        details: $"Delete Preventive Scheduler",
                        module:"Preventive Scheduler"
                    );               
                    await _mediator.Publish(domainEvent, cancellationToken);  

                 if(preventiveSchedulerresult)
                {
                    return new ApiResponseDTO<bool>
                    {
                        IsSuccess = true, 
                        Message = "Preventive Scheduler deleted successfully."
                    };
                }

                return new ApiResponseDTO<bool>
                {
                    IsSuccess = false, 
                    Message = "Preventive Scheduler not deleted."
                };
        }
    }
}