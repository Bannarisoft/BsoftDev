    using AutoMapper;
    using Core.Application.AssetMaster.AssetSpecification.Queries.GetAssetSpecification;
    using Core.Application.AssetMaster.AssetSpecification.Queries.GetAssetSpecificationAutoComplete;
    using Core.Application.Common.HttpResponse;
    using Core.Application.Common.Interfaces.IAssetMaster.IAssetSpecification;
    using Core.Domain.Events;
    using MediatR;

    namespace Core.Application.AssetMaster.AssetSpecification.Queries.GetAssetSpecificationAutoComplete
    {
        public class GetAssetSpecificationAutoCompleteQueryHandler : IRequestHandler<GetAssetSpecificationAutoCompleteQuery, ApiResponseDTO<List<AssetSpecificationAutoCompleteDTO>>>
        {
            private readonly IAssetSpecificationQueryRepository _assetSpecificationRepository;
            private readonly IMapper _mapper;
            private readonly IMediator _mediator; 

            public GetAssetSpecificationAutoCompleteQueryHandler(IAssetSpecificationQueryRepository assetSpecificationRepository,  IMapper mapper, IMediator mediator)
            {
                _assetSpecificationRepository =assetSpecificationRepository;
                _mapper =mapper;
                _mediator = mediator;
            }

            public async Task<ApiResponseDTO<List<AssetSpecificationAutoCompleteDTO>>> Handle(GetAssetSpecificationAutoCompleteQuery request, CancellationToken cancellationToken)
            {
                var result = await _assetSpecificationRepository.GetByAssetSpecificationNameAsync(request.SearchPattern ?? string.Empty);
                if (result is null || result.Count is 0)
                {
                    return new ApiResponseDTO<List<AssetSpecificationAutoCompleteDTO>>
                    {
                        IsSuccess = false,
                        Message = "No SpecificationMaster found matching the search pattern."
                    };
                }
                var specificationMasterDto = _mapper.Map<List<AssetSpecificationAutoCompleteDTO>>(result);
                //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAutoComplete",
                    actionCode:"",        
                    actionName: request.SearchPattern ?? string.Empty,                
                    details: $"Asset Specification '{request.SearchPattern}' was searched",
                    module:"Asset Specification"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
                return new ApiResponseDTO<List<AssetSpecificationAutoCompleteDTO>>
                {
                    IsSuccess = true,
                    Message = "Success",
                    Data = specificationMasterDto
                };          
            }      
        }
    }