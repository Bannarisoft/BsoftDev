using AutoMapper;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral;
using Core.Domain.Entities;
using Core.Domain.Entities.AssetPurchase;
using Core.Domain.Entities.AssetMaster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Commands.UpdateAssetComposite
{    // Update composite command should return an ApiResponseDTO of the updated asset DTO.

    public class UpdateAssetCompositeCommandHandler : IRequestHandler<UpdateAssetCompositeCommand, ApiResponseDTO<AssetMasterGeneralDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IAssetMasterGeneralCommandRepository _assetMasterGeneralRepository;
        private readonly IAssetMasterGeneralQueryRepository _assetMasterQueryGeneralRepository;
        private readonly IMediator _mediator;

        public UpdateAssetCompositeCommandHandler(
            IMapper mapper,
            IAssetMasterGeneralCommandRepository assetMasterGeneralRepository,
            IAssetMasterGeneralQueryRepository assetMasterQueryGeneralRepository,
            IMediator mediator)
        {
            _mapper = mapper;
            _assetMasterGeneralRepository = assetMasterGeneralRepository;
            _assetMasterQueryGeneralRepository = assetMasterQueryGeneralRepository;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<AssetMasterGeneralDTO>> Handle(UpdateAssetCompositeCommand request, CancellationToken cancellationToken)
        {
            var response = new ApiResponseDTO<AssetMasterGeneralDTO>();
            try
            {
                // Retrieve the existing asset master using the ID from the update DTO.
                var existingAsset = await _assetMasterQueryGeneralRepository.GetByIdAsync(request.AssetComposite.UpdateAssetMaster.Id);
                if (existingAsset == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Asset not found.";
                    return response;
                }

                // Execute the update within a transaction.
                await _assetMasterGeneralRepository.ExecuteInTransactionAsync(async () =>
                {
                    // Map the update command's master data into the existing asset.
                    // This will update only the matching properties.
                    _mapper.Map(request.AssetComposite.UpdateAssetMaster, existingAsset);

                    // Handle child collections.
                    // Clear existing purchase details and re-add from the request.
                    if (existingAsset.AssetPurchase == null)
                        existingAsset.AssetPurchase = new List<AssetPurchaseDetails>();
                    else
                        existingAsset.AssetPurchase.Clear();

                    if (request.AssetComposite.UpdateAssetPurchaseDetail != null)
                    {
                        foreach (var purchaseDto in request.AssetComposite.UpdateAssetPurchaseDetail)
                        {
                            var purchaseDetail = _mapper.Map<AssetPurchaseDetails>(purchaseDto);
                            // Manually assign the asset Id
                            purchaseDetail.AssetId = existingAsset.Id;
                            existingAsset.AssetPurchase.Add(purchaseDetail);
                        }
                    }

                    // Clear and update asset locations similarly.
                    if (existingAsset.AssetLocations == null)
                        existingAsset.AssetLocations = new List<Core.Domain.Entities.AssetMaster.AssetLocation>();
                    else
                        existingAsset.AssetLocations.Clear();

                    if (request.AssetComposite.UpdateAssetLocation != null)
                    {
                        foreach (var locationDto in request.AssetComposite.UpdateAssetLocation)
                        {
                            var locationDetail = _mapper.Map<Core.Domain.Entities.AssetMaster.AssetLocation>(locationDto);
                            locationDetail.AssetId = existingAsset.Id;
                            existingAsset.AssetLocations.Add(locationDetail);
                        }
                    }
                var assetMasterGenerals = _mapper.Map<Core.Domain.Entities.AssetMasterGenerals>(existingAsset);
                    // Persist changes by calling the repository's update method.
                await _assetMasterGeneralRepository.UpdateAsync(existingAsset.Id, assetMasterGenerals);
                }, cancellationToken);

                // Retrieve updated asset and map to DTO.
                var updatedAsset = await _assetMasterQueryGeneralRepository.GetByIdAsync(existingAsset.Id);
                var assetDTO = _mapper.Map<AssetMasterGeneralDTO>(updatedAsset);

                response.IsSuccess = true;
                response.Message = "Asset updated successfully.";
                response.Data = assetDTO;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
