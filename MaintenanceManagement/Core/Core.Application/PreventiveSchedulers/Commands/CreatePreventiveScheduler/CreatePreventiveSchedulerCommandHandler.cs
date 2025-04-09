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

namespace Core.Application.PreventiveSchedulers.Commands.CreatePreventiveScheduler
{
    public class CreatePreventiveSchedulerCommandHandler : IRequestHandler<CreatePreventiveSchedulerCommand, ApiResponseDTO<int>>
    {
        private readonly IPreventiveSchedulerCommand _preventiveSchedulerCommand;
        private readonly IMapper _imapper;
        private readonly IMediator _mediator;
        public CreatePreventiveSchedulerCommandHandler(IPreventiveSchedulerCommand preventiveSchedulerCommand, IMapper imapper, IMediator mediator)
        {
            _preventiveSchedulerCommand = preventiveSchedulerCommand;
            _imapper = imapper;
            _mediator = mediator;
        }
        public async Task<ApiResponseDTO<int>> Handle(CreatePreventiveSchedulerCommand request, CancellationToken cancellationToken)
        {
            var preventiveScheduler  = _imapper.Map<PreventiveSchedulerHdr>(request);

                var preventiveSchedulerresult = await _preventiveSchedulerCommand.CreateAsync(preventiveScheduler);
                
                
                if (preventiveSchedulerresult > 0)
                {
                    var domainEvent = new AuditLogsDomainEvent(
                     actionDetail: "Create",
                     actionCode: "Create preventive scheduler",
                     actionName: "Create",
                     details: $"Preventive scheduler created",
                     module:"Preventive scheduler"
                 );
                 await _mediator.Publish(domainEvent, cancellationToken);
                 
                    return new ApiResponseDTO<int>
                    {
                        IsSuccess = true, 
                        Message = "Preventive scheduler created successfully",
                         Data = preventiveSchedulerresult
                    };
                }
               
                    return new ApiResponseDTO<int>
                    {
                        IsSuccess = false, 
                        Message = "Preventive scheduler not created"
                    };
        }
    }
}