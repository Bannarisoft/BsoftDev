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
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        public CreatePreventiveSchedulerCommandHandler(IPreventiveSchedulerCommand preventiveSchedulerCommand, IMapper mapper, IMediator mediator)
        {
            _preventiveSchedulerCommand = preventiveSchedulerCommand;
            _mapper = mapper;
            _mediator = mediator;
        }
        public async Task<ApiResponseDTO<int>> Handle(CreatePreventiveSchedulerCommand request, CancellationToken cancellationToken)
        {
            var preventiveScheduler  = _mapper.Map<PreventiveSchedulerHeader>(request);

                var response = await _preventiveSchedulerCommand.CreateAsync(preventiveScheduler);
                
                
                if (response > 0)
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
                         Data = response
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