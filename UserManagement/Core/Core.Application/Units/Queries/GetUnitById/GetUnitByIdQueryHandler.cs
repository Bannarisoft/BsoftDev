using AutoMapper;
using Core.Application.Common;

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
    public class GetUnitByIdQueryHandler : IRequestHandler<GetUnitByIdQuery,ApiResponseDTO<GetUnitsByIdDto>>
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

         public async Task<ApiResponseDTO<GetUnitsByIdDto>> Handle(GetUnitByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Fetching Unit Request started: {request.Id}");
            var units = await _unitRepository.GetByIdAsync(request.Id);    

              if (units is null)
                {
                    _logger.LogWarning($"No Unit Record {request.Id} not found in DB.");
                     return new ApiResponseDTO<GetUnitsByIdDto>
                     {
                         IsSuccess = false,
                         Message = "Unit not found."

                     };
                }

            var unitList = _mapper.Map<GetUnitsByIdDto>(units);
            //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetUnitByIdQuery",
                    actionCode: unitList.Id.ToString(),        
                    actionName: unitList.UnitName,
                    details: $"Unit '{unitList.UnitName}' was Fetched. UnitId: {unitList.Id}",
                    module:"Unit"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
            _logger.LogInformation($"Fetching Unit Request Completed: {request.Id}");
            return new ApiResponseDTO<GetUnitsByIdDto>
            {
                IsSuccess = true,
                Message = "Success",
                Data = unitList
            };
     
          
        }
    }
}   