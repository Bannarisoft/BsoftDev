using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetDetailsById
{
    public class GetAsstDetailsByIdQueryHandler : IRequestHandler<GetAsstDetailsByIdQuery, ApiResponseDTO<AssetDetailsResponse>>
    {
        private readonly IAssetDetailsQueryRepository _assetRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

        public GetAsstDetailsByIdQueryHandler(IAssetDetailsQueryRepository assetRepository, IMapper mapper, IMediator mediator)
        {
            _assetRepository = assetRepository;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<AssetDetailsResponse>> Handle(GetAsstDetailsByIdQuery request, CancellationToken cancellationToken)
        {
            var assetId = request.AssetId;

            var response = new AssetDetailsResponse
            {
               // AssetMaster = await _assetRepository.GetAssetMasterByIdAsync(assetId),
                AssetLocation = await _assetRepository.GetAssetLocationByIdAsync(assetId),
                AssetPurchase = await _assetRepository.GetAssetPurchaseByIdAsync(assetId),
                AssetAMC = await _assetRepository.GetAssetAMCByIdAsync(assetId),
                AssetWarranty = await _assetRepository.GetAssetWarrantyByIdAsync(assetId),
                AssetSpecification = await _assetRepository.GetAssetSpecificationByIdAsync(assetId),
                AssetDisposal = await _assetRepository.GetAssetDisposalByIdAsync(assetId),
                AssetInsurance = await _assetRepository.GetAssetInsuranceByIdAsync(assetId),
                AssetAdditionalCost = await _assetRepository.GetAssetAdditionalCostByIdAsync(assetId)
            };    

            // Log fetched data for debugging purposes
            Console.WriteLine($"Fetched Data for AssetId = {assetId}: {System.Text.Json.JsonSerializer.Serialize(response)}");

            // Check if any data was retrieved
            if (response.AssetMaster == null && response.AssetLocation == null && response.AssetPurchase == null &&
                response.AssetAMC == null && response.AssetWarranty == null && response.AssetSpecification == null &&
                response.AssetDisposal == null && response.AssetInsurance == null && response.AssetAdditionalCost == null)
            {                
                return new ApiResponseDTO<AssetDetailsResponse>
                {
                    IsSuccess = false,
                    Message = $"Asset with ID {request.AssetId} not found.",
                    Data = null
                };
            }       
            
            // Publish domain event if data is found
            if (response.AssetMaster != null)
            {
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: response.AssetMaster.AssetCode ?? string.Empty,        
                    actionName: response.AssetMaster.AssetName ?? string.Empty,                
                    details: $"Asset '{response.AssetMaster.AssetName}' was retrieved successfully. Code: {response.AssetMaster.AssetCode}",
                    module: "AssetDetail"
                );

                await _mediator.Publish(domainEvent, cancellationToken);
            }

            return new ApiResponseDTO<AssetDetailsResponse>
            {
                IsSuccess = true,
                Message = "Success",
                Data = response
            };
        }      
    }
}
