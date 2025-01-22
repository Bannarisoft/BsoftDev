using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.Exceptions;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IUnit;
using Core.Application.Units.Queries.GetUnits;
using Core.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Core.Application.Units.Queries.GetUnitById
{
    //public class GetUnitByIdQueryHandler : IRequestHandler<GetUnitByIdQuery,UnitDto>
    public class GetUnitByIdQueryHandler : IRequestHandler<GetUnitByIdQuery,ApiResponseDTO<List<UnitDto>>>
    {
         private readonly IUnitQueryRepository _unitRepository;        
        private readonly IMapper _mapper;

        private readonly IMediator _mediator;

         private readonly ILogger<GetUnitByIdQueryHandler> _logger;

        public GetUnitByIdQueryHandler(IUnitQueryRepository unitRepository, IMapper mapper, IMediator mediator,ILogger<GetUnitByIdQueryHandler> logger)
        {
            _unitRepository = unitRepository;
            _mapper = mapper;
            _mediator = mediator;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

         public async Task<ApiResponseDTO<List<UnitDto>>> Handle(GetUnitByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching Unit Request started: {request}", request.Id);
            var units = await _unitRepository.GetByIdAsync(request.Id);    

              if (units == null || !units.Any())
                {
                    _logger.LogWarning("No Unit Record {Unit} not found in DB.", request.Id);
                     return new ApiResponseDTO<List<UnitDto>>
                     {
                         IsSuccess = false,
                         Message = "Unit not found."

                     };
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
            _logger.LogInformation("Fetching Unit Request Completed: {request}", request.Id);
            return new ApiResponseDTO<List<UnitDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = unitList
            };
     
          
        }
    }
}   