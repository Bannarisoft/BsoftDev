using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IDepreciationCalculation;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.DepreciationCalculation.Queries.GetDepreciationCalculation
{
    public class GetDepreciationCalculationQueryHandler : IRequestHandler<GetDepreciationCalculationQuery, ApiResponseDTO<List<DepreciationDto>>>
    {
        private readonly IDepreciationCalculationQueryRepository _depreciationCalculationRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

        public GetDepreciationCalculationQueryHandler(IDepreciationCalculationQueryRepository depreciationCalculationRepository , IMapper mapper, IMediator mediator)
        {
            _depreciationCalculationRepository = depreciationCalculationRepository;
            _mapper = mapper;
            _mediator = mediator;
        }        
        public async Task<ApiResponseDTO<List<DepreciationDto>>> Handle(GetDepreciationCalculationQuery request, CancellationToken cancellationToken)
        {
            var (assetSpecification, totalCount) = await _depreciationCalculationRepository.CalculateDepreciationAsync(request.CompanyId,request.UnitId, request.StartDate,request.EndDate,request.PageNumber, request.PageSize, request.SearchTerm);
            var assetSpecificationList = _mapper.Map<List<DepreciationDto>>(assetSpecification);

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetAll",
                actionCode: "",        
                actionName: "",
                details: $"Asset Specification details was fetched.",
                module:"Asset Specification"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<DepreciationDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = assetSpecificationList,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };            
        }
    }
}