using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.Exceptions;
using Core.Application.Common.Interfaces.IUnit;
using Core.Application.Units.Queries.GetUnits;
using Core.Domain.Events;
using MediatR;
using System.Data;

namespace Core.Application.Units.Queries.GetUnitById
{
    //public class GetUnitByIdQueryHandler : IRequestHandler<GetUnitByIdQuery,UnitDto>
    public class GetUnitByIdQueryHandler : IRequestHandler<GetUnitByIdQuery,Result<List<UnitDto>>>
    {
         private readonly IUnitQueryRepository _unitRepository;        
        private readonly IMapper _mapper;

        private readonly IMediator _mediator;

        public GetUnitByIdQueryHandler(IUnitQueryRepository unitRepository, IMapper mapper, IMediator mediator)
        {
            _unitRepository = unitRepository;
            _mapper = mapper;
            _mediator = mediator;
        }

         public async Task<Result<List<UnitDto>>> Handle(GetUnitByIdQuery request, CancellationToken cancellationToken)
        {
          
            var units = await _unitRepository.GetByIdAsync(request.Id);    

              if (units == null || !units.Any())
                {
                return Result<List<UnitDto>>.Failure("Unit not found.");
                }

            var unitList = _mapper.Map<List<UnitDto>>(units);
            //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetUnitByIdQuery",
                    actionCode: unitList[0].Id.ToString(),        
                    actionName: unitList[0].UnitName,
                    details: $"Unit '{unitList[0].UnitName}' was Fetched. UnitId: {unitList[0].Id}",
                    module:"Unit"
                );
                await _mediator.Publish(domainEvent, cancellationToken);

            return Result<List<UnitDto>>.Success(unitList);;
     
          
        }
    }
}   