using MediatR;
using Core.Application.Units.Queries.GetUnits;
using System.Data;
using Core.Application.Common.Interfaces.IUnit;
using AutoMapper;
using Core.Application.Common;
using Core.Domain.Events;
using Core.Application.Common.Exceptions;
using Core.Application.Common.HttpResponse;

namespace Core.Application.Units.Queries.GetUnitAutoComplete
{
    public class GetUnitAutoCompleteQueryHandler : IRequestHandler<GetUnitAutoCompleteQuery, ApiResponseDTO<List<UnitDto>>>
    {
         private readonly IUnitQueryRepository _unitRepository;        
        private readonly IMapper _mapper;

        private readonly IMediator _mediator; 


        public GetUnitAutoCompleteQueryHandler(IUnitQueryRepository unitRepository, IMapper mapper, IMediator mediator)
        {
             _unitRepository = unitRepository;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<List<UnitDto>>> Handle(GetUnitAutoCompleteQuery request, CancellationToken cancellationToken)
        {     
           
            var result = await _unitRepository.GetUnit(request.SearchPattern);
              if (result == null || !result.Any())
                {
                     return new ApiResponseDTO<List<UnitDto>>
                     {
                         IsSuccess = false,
                         Message = "Unit not found."
                     };
                }

            var unitDto = _mapper.Map<List<UnitDto>>(result);

            //Domain Event            
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetUnitAutoCompleteQuery",
                actionCode:"",        
                actionName: request.SearchPattern,                
                details: $"Unit '{request.SearchPattern}' was searched",
                module:"Unit"
            );
            await _mediator.Publish(domainEvent, cancellationToken);

            return new ApiResponseDTO<List<UnitDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = unitDto
            };                                    
        }
    }
}