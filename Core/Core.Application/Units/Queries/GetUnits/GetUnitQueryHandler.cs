using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.Exceptions;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IUnit;
using Core.Domain.Events;
using MediatR;
using System.Data;

namespace Core.Application.Units.Queries.GetUnits
{
    public class GetUnitQueryHandler : IRequestHandler<GetUnitQuery,ApiResponseDTO<List<UnitDto>>>
    {
        private readonly IUnitQueryRepository _unitRepository;        
        private readonly IMapper _mapper;

        private readonly IMediator _mediator;
        public GetUnitQueryHandler( IUnitQueryRepository unitRepository, IMapper mapper, IMediator mediator)
        {
            _unitRepository = unitRepository;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<List<UnitDto>>> Handle(GetUnitQuery request, CancellationToken cancellationToken)
        {
        
            
            var units = await _unitRepository.GetAllUnitsAsync();
            var unitList = _mapper.Map<List<UnitDto>>(units);
            //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetUnitQuery",
                    actionCode: "",        
                    actionName: "",
                    details: $"Units details was fetched.",
                    module:"Unit"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
                return new ApiResponseDTO<List<UnitDto>>
                {
                    IsSuccess = true,
                    Message = "Success",
                    Data = unitList
                };
           
        
        }
    }
}