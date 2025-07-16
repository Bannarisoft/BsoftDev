using AutoMapper;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral;
using Core.Application.DepreciationGroup.Queries.GetDepreciationGroup;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneralAutoComplete
{
    public class GetAssetMasterGeneralAutoCompleteQueryHandler : IRequestHandler<GetAssetMasterGeneralAutoCompleteQuery, ApiResponseDTO<List<AssetMasterGeneralAutoCompleteDTO>>>
    {
        private readonly IAssetMasterGeneralQueryRepository _assetMasterRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

        public GetAssetMasterGeneralAutoCompleteQueryHandler(IAssetMasterGeneralQueryRepository assetMasterRepository,  IMapper mapper, IMediator mediator)
        {
            _assetMasterRepository =assetMasterRepository;
            _mapper =mapper;
            _mediator = mediator;
        }
        public async Task<ApiResponseDTO<List<AssetMasterGeneralAutoCompleteDTO>>> Handle(GetAssetMasterGeneralAutoCompleteQuery request, CancellationToken cancellationToken)
        {
            var result = await _assetMasterRepository.GetByAssetNameAsync(request.SearchPattern ?? string.Empty);
            if (result is null || result.Count is 0)
            {
                return new ApiResponseDTO<List<AssetMasterGeneralAutoCompleteDTO>>
                {
                    IsSuccess = false,
                    Message = "No Asset found matching the search pattern."
                };
            }
            var assetMasterDto = _mapper.Map<List<AssetMasterGeneralAutoCompleteDTO>>(result);
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetAutoComplete",
                actionCode:"",        
                actionName: request.SearchPattern ?? string.Empty,                
                details: $"Asset '{request.SearchPattern}' was searched",
                module:"AssetMasterGeneral"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<AssetMasterGeneralAutoCompleteDTO>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = assetMasterDto
            };  
        }
    }
}