using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IWdvDepreciation;
using Core.Application.WDVDepreciation.Queries.GetDepreciation;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.WDVDepreciation.Queries.CalculateDepreciation
{
    public class GetDepreciationQueryHandler : IRequestHandler<GetDepreciationQuery, ApiResponseDTO<List<CalculationDepreciationDto>>>
    {        
        private readonly IWdvDepreciationQueryRepository _WdvQueryRepository;
        private readonly IMapper _mapper;

        private readonly IMediator _mediator; 

        public GetDepreciationQueryHandler(IWdvDepreciationQueryRepository WdvQueryRepository,IMapper mapper, IMediator mediator)
        {     
            _WdvQueryRepository = WdvQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
        }
        public async Task<ApiResponseDTO<List<CalculationDepreciationDto>>> Handle(GetDepreciationQuery request, CancellationToken cancellationToken)
        {
            var depreciationGroups = await _WdvQueryRepository.ExistDataAsync( request.FinYearId);
            if (depreciationGroups==false)
            {
                return new ApiResponseDTO<List<CalculationDepreciationDto>>
                {
                    IsSuccess = false,
                    Message = "No details found for this period",
                    Data = new List<CalculationDepreciationDto>()
                };
            }           
            var WDVCalculation = await _WdvQueryRepository.GetWDVDepreciationAsync(request.FinYearId);
            var WDVCalculationList = _mapper.Map<List<CalculationDepreciationDto>>(WDVCalculation);

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Get WDVCalculation",
                actionCode: "",        
                actionName: "",
                details: $"Get WDV Depreciation Calculation.",
                module:"Get WDV Calculation"
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