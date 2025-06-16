using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMiscMaster;
using Core.Application.Common.Interfaces.IPreventiveScheduler;
using MediatR;

namespace Core.Application.PreventiveSchedulers.Commands.ReschedulePreventive
{
    public class ReschedulePreventiveCommandHandler : IRequestHandler<ReshedulePreventiveCommand, ApiResponseDTO<bool>>
    {
        private readonly IPreventiveSchedulerCommand _preventiveSchedulerCommand;
         private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IMiscMasterQueryRepository _miscMasterQueryRepository;
        private readonly IPreventiveSchedulerQuery _preventiveSchedulerQuery;
        public ReschedulePreventiveCommandHandler(IPreventiveSchedulerCommand preventiveSchedulerCommand, IMapper mapper, IMediator mediator, IMiscMasterQueryRepository miscMasterQueryRepository, IPreventiveSchedulerQuery preventiveSchedulerQuery)
        {
            _preventiveSchedulerCommand = preventiveSchedulerCommand;
            _mapper = mapper;
            _mediator = mediator;
            _miscMasterQueryRepository = miscMasterQueryRepository;
            _preventiveSchedulerQuery = preventiveSchedulerQuery;
        }
        public async Task<ApiResponseDTO<bool>> Handle(ReshedulePreventiveCommand request, CancellationToken cancellationToken)
        {
            var result = await _preventiveSchedulerQuery.ExistWorkOrderBySchedulerDetailId(request.PreventiveScheduleDetailId);

            if (result == true)
            {
                await AuditLogPublisher.PublishAuditLogAsync(
                    _mediator,
                    actionDetail: $"Reschedule Update DueDate",
                    actionCode: "Update DueDate",
                    actionName: "Update DueDate",
                    module: "Preventive",
                    requestData: request,
                    cancellationToken
                   );
                var DetailResult = await _preventiveSchedulerCommand.UpdateRescheduleDate(request.PreventiveScheduleDetailId, request.RescheduleDate);
            }
            else
            {
                await AuditLogPublisher.PublishAuditLogAsync(
                    _mediator,
                    actionDetail: $"Reschedule",
                    actionCode: "Reschedule",
                    actionName: "Reschedule",
                    module: "Preventive",
                    requestData: request,
                    cancellationToken
                   );
                var Result = await _preventiveSchedulerCommand.AddReScheduleDetailAsync(request.PreventiveScheduleDetailId, request.RescheduleDate, cancellationToken);

                if (Result != null)
                {
                    await _preventiveSchedulerCommand.UpdateDetailAsync(Result.Id, Result.HangfireJobId);
                }
                 
            }

          
                return new ApiResponseDTO<bool>()
                {
                    IsSuccess = true,
                    Message = "Preventive Scheduler Rescheduled successfully."
                };
            
            
        }
    }
}