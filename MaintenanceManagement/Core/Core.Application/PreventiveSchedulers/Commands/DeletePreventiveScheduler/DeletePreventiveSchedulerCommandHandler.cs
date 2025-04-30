using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using Core.Domain.Entities;
using Core.Domain.Events;
using Hangfire;
using MediatR;

namespace Core.Application.PreventiveSchedulers.Commands.DeletePreventiveScheduler
{
    public class DeletePreventiveSchedulerCommandHandler : IRequestHandler<DeletePreventiveSchedulerCommand, ApiResponseDTO<bool>>
    {
        private readonly IPreventiveSchedulerCommand _preventiveSchedulerCommand;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IPreventiveSchedulerQuery _preventiveSchedulerQuery;
        public DeletePreventiveSchedulerCommandHandler(IPreventiveSchedulerCommand preventiveSchedulerCommand, IMapper mapper, IMediator mediator, IPreventiveSchedulerQuery preventiveSchedulerQuery)
        {
            _preventiveSchedulerCommand = preventiveSchedulerCommand;
            _mapper = mapper;
            _mediator = mediator;
            _preventiveSchedulerQuery = preventiveSchedulerQuery;
        }
        public async Task<ApiResponseDTO<bool>> Handle(DeletePreventiveSchedulerCommand request, CancellationToken cancellationToken)
        {
             var preventiveScheduler  = _mapper.Map<PreventiveSchedulerHeader>(request);
            var response = await _preventiveSchedulerCommand.DeleteAsync(request.Id,preventiveScheduler);
            var DetailResult = await _preventiveSchedulerQuery.GetPreventiveSchedulerDetail(request.Id);

            foreach (var detail in DetailResult)
            {
                     if (!string.IsNullOrEmpty(detail.HangfireJobId))
                     {
                         BackgroundJob.Delete(detail.HangfireJobId); 
                     }
        
            }
                   

                  //Domain Event  
                    var domainEvent = new AuditLogsDomainEvent(
                        actionDetail: "Delete",
                        actionCode: "delete",
                        actionName: "Delete Preventive Scheduler",
                        details: $"Delete Preventive Scheduler",
                        module:"Preventive Scheduler"
                    );               
                    await _mediator.Publish(domainEvent, cancellationToken);  

                 if(response)
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