using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IShiftMaster;
using Core.Application.ShiftMasterCQRS.Queries.GetShiftMaster;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.ShiftMasterCQRS.Queries.GetShiftMasterById
{
    public class GetShiftMasterByIdQueryHandler : IRequestHandler<GetShiftMasterByIdQuery, ApiResponseDTO<ShiftMasterDTO>>
    {
        private readonly IShiftMasterQuery _shiftMasterQuery;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        public GetShiftMasterByIdQueryHandler(IShiftMasterQuery shiftMasterQuery, IMapper mapper, IMediator mediator)
        {
            _shiftMasterQuery = shiftMasterQuery;
            _mapper = mapper;
            _mediator = mediator;
        }
        public async Task<ApiResponseDTO<ShiftMasterDTO>> Handle(GetShiftMasterByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _shiftMasterQuery.GetByIdAsync(request.Id);
            var shiftMaster = _mapper.Map<ShiftMasterDTO>(result);

          //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: "",        
                    actionName: "",
                    details: $"Shift master details was fetched.",
                    module:"Shift master"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
          return new ApiResponseDTO<ShiftMasterDTO> 
          { 
            IsSuccess = true, 
            Message = "Success", 
            Data = shiftMaster 
            };
        }
    }
}