using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IWdvDepreciation;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.WDVDepreciation.Queries.CalculateDepreciation
{
    public class CalculateDepreciationQueryHandler : IRequestHandler<CalculateDepreciationQuery, ApiResponseDTO<List<CalculationDepreciationDto>>>
    {        
        private readonly IWdvDepreciationQueryRepository _WdvQueryRepository;
        private readonly IMapper _mapper;

        private readonly IMediator _mediator; 

        public CalculateDepreciationQueryHandler(IWdvDepreciationQueryRepository WdvQueryRepository, IMapper mapper, IMediator mediator)
        {            
            _WdvQueryRepository = WdvQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
        }
        public async Task<ApiResponseDTO<List<CalculationDepreciationDto>>> Handle(CalculateDepreciationQuery request, CancellationToken cancellationToken)
        {
              var depreciationLocked = await _WdvQueryRepository.ExistDataLockedAsync(request.FinYearId);
            if (depreciationLocked==true)
            {
              return new ApiResponseDTO<List<CalculationDepreciationDto>>
                {
                    IsSuccess = false,
                    Message = "Already depreciation details Locked",
                    Data = new List<CalculationDepreciationDto>()
                };
            }
            var depreciationGroups = await _WdvQueryRepository.ExistDataAsync( request.FinYearId);
            if (depreciationGroups==true)
            {
               return new ApiResponseDTO<List<CalculationDepreciationDto>>
                    {
                        IsSuccess = false,
                        Message = "Already depreciation details exist",
                        Data = new List<CalculationDepreciationDto>()
                    };
            }
            var WDVCalculation = await _WdvQueryRepository.CalculateWDVAsync(request.FinYearId);
            var WDVCalculationList = _mapper.Map<List<CalculationDepreciationDto>>(WDVCalculation);

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "WDVCalculation",
                actionCode: "",        
                actionName: "",
                details: $"WDV Depreciation Calculation.",
                module:"WDV Calculation"
            );
            
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<CalculationDepreciationDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = WDVCalculationList                
            };            
        }
    }
  
}