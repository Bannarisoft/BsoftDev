using AutoMapper;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetParentMaster
{
    public class GetAssetParentMasterQueryHandler : IRequestHandler<GetAssetParentMasterQuery, ApiResponseDTO<List<AssetMasterGeneralAutoCompleteDTO>>>
    {
        private readonly IAssetMasterGeneralQueryRepository _assetMasterRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 
        
        public GetAssetParentMasterQueryHandler(IAssetMasterGeneralQueryRepository assetMasterRepository,  IMapper mapper, IMediator mediator)
        {
            _assetMasterRepository = assetMasterRepository;
            _mapper = mapper;
            _mediator = mediator;
        }
  
        public async Task<ApiResponseDTO<List<AssetMasterGeneralAutoCompleteDTO>>> Handle(GetAssetParentMasterQuery request, CancellationToken cancellationToken)
        {
            if (request.AssetType == "Dependent Parent")
            {
                var result = await _assetMasterRepository.GetByAssetNameAsync("");
                if (result is null || result.Count == 0)
                {
                    return new ApiResponseDTO<List<AssetMasterGeneralAutoCompleteDTO>>
                    {
                        IsSuccess = false,
                        Message = "No Asset found matching the search pattern."
                    };
                }
                var assetMasterDto = _mapper.Map<List<AssetMasterGeneralAutoCompleteDTO>>(result);
                
                // Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAutoComplete",
                    actionCode: "",        
                    actionName: request.AssetType ?? string.Empty,                
                    details: $"Asset '{request.AssetType}' was searched",
                    module: "AssetMasterGeneral"
                );
                await _mediator.Publish(domainEvent, cancellationToken);

                return new ApiResponseDTO<List<AssetMasterGeneralAutoCompleteDTO>>
                {
                    IsSuccess = true,
                    Message = "Success",
                    Data = assetMasterDto
                };   
            } 

            return new ApiResponseDTO<List<AssetMasterGeneralAutoCompleteDTO>>
            {
                IsSuccess = false,
                Message = "Invalid AssetType. Only 'Dependent Parent' is supported."
            };
        }
    }
}
