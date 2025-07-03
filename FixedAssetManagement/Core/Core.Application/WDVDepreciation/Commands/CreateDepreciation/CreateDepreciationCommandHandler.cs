using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IDepreciationDetail;
using Core.Application.Common.Interfaces.IWdvDepreciation;
using Core.Application.WDVDepreciation.Queries.CalculateDepreciation;
using Core.Application.WDVDepreciation.Queries.GetDepreciation;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.WDVDepreciation.Commands.CreateDepreciation
{
    public class CreateDepreciationCommandHandler : IRequestHandler<CreateDepreciationCommand, ApiResponseDTO<CalculationDepreciationDto>>
    {        
        private readonly IWdvDepreciationQueryRepository _WdvQueryRepository;
        private readonly IMapper _mapper;

        private readonly IMediator _mediator; 

        public CreateDepreciationCommandHandler(IWdvDepreciationQueryRepository WdvQueryRepository, IMapper mapper, IMediator mediator)
        {            
            _WdvQueryRepository = WdvQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
        }
        public async Task<ApiResponseDTO<CalculationDepreciationDto>> Handle(CreateDepreciationCommand request, CancellationToken cancellationToken)
        {
            var depreciationLocked = await _WdvQueryRepository.ExistDataLockedAsync(request.FinYearId);
            if (depreciationLocked==true)
            {
                return new ApiResponseDTO<CalculationDepreciationDto>
                {
                    IsSuccess = false,
                    Message = "Already depreciation details Locked."
                };
            }
            var depreciationGroups = await _WdvQueryRepository.ExistDataAsync( request.FinYearId);
            if (depreciationGroups==true)
            {
               return new ApiResponseDTO<CalculationDepreciationDto>
                {
                    IsSuccess = false,
                    Message = "Already depreciation details exist"                        
                };
            }         

            var assetGroup = _mapper.Map<Core.Domain.Entities.WDVDepreciationDetail>(request);
            var result = await _WdvQueryRepository.CreateAsync(request.FinYearId);            
        
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "WDVCalculation",
                actionCode: "",
                actionName: "",
                details: $"WDV Depreciation Calculation.",
                module: "WDV Calculation"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            if (result != null && result.Any())
            {
                return new ApiResponseDTO<CalculationDepreciationDto>
                {
                    IsSuccess = true,
                    Message = "WDV Calculation created successfully.",
                    Data = null // you don't need to map or return data
                };
            }

            return new ApiResponseDTO<CalculationDepreciationDto>
            {
                IsSuccess = false,
                Message = "WDV Calculation not created.",
                Data = null
            };
        }
    }
  
}