using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMachineMaster;
using Core.Application.Common.Interfaces.IMaintenanceCategory;
using Core.Application.MachineMaster.Queries.GetMachineMaster;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MachineMaster.Queries.GetMachineMasterById
{
    public class GetMachineMasterByIdQueryHandler : IRequestHandler<GetMachineMasterByIdQuery, ApiResponseDTO<MachineMasterDto>>
    {
        
        private readonly IMachineMasterQueryRepository _imachineMasterQueryRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        public GetMachineMasterByIdQueryHandler(IMachineMasterQueryRepository imachineMasterQueryRepository, IMapper mapper, IMediator mediator)
        {
            _imachineMasterQueryRepository = imachineMasterQueryRepository;            
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<MachineMasterDto>> Handle(GetMachineMasterByIdQuery request, CancellationToken cancellationToken)
        {
             var result = await _imachineMasterQueryRepository.GetByIdAsync(request.Id);
            // Check if the entity exists
            if (result is null)
            {
                return new ApiResponseDTO<MachineMasterDto> { IsSuccess = false, Message =$"Machine ID {request.Id} not found." };
            }
            // Map a single entity
            var machineMaster = _mapper.Map<MachineMasterDto>(result);

          //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: "GetMachineMasterByIdQuery",        
                    actionName: machineMaster.Id.ToString(),
                    details: $"MachineMaster details {machineMaster.Id} was fetched.",
                    module:"MachineMaster"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
          return new ApiResponseDTO<MachineMasterDto> { IsSuccess = true, Message = "Success", Data = machineMaster };
        }
    }
}