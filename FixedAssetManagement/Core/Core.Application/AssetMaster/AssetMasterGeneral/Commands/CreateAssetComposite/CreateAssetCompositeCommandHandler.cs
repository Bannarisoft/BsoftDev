using AutoMapper;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral;
using Core.Domain.Entities;
using Core.Domain.Entities.AssetPurchase;
using Core.Domain.Entities.AssetMaster;
using MediatR;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Commands.CreateAssetComposite
{
    public class CreateAssetCompositeCommandHandler : IRequestHandler<CreateAssetCompositeCommand, ApiResponseDTO<AssetMasterGeneralDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IAssetMasterGeneralCommandRepository _assetMasterGeneralRepository;
        private readonly IAssetMasterGeneralQueryRepository _assetMasterQueryGeneralRepository;
        private readonly IMediator _mediator;

        public CreateAssetCompositeCommandHandler(
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

        public async Task<ApiResponseDTO<AssetMasterGeneralDTO>> Handle(CreateAssetCompositeCommand request, CancellationToken cancellationToken)
        {
            var response = new ApiResponseDTO<AssetMasterGeneralDTO>();
            int assetId = 0;
            try
            {
                // Execute the composite operation in a transaction.
                await _assetMasterGeneralRepository.ExecuteInTransactionAsync(async () =>
                {
                    // Fetch Company Name from DTO or Database
                    var UnitName = request.AssetComposite.AssetMaster.UnitName;
                    var assetGroupName = await _assetMasterGeneralRepository.GetAssetGroupNameById(request.AssetComposite.AssetMaster.AssetGroupId);
                    var assetCategoryName = await _assetMasterGeneralRepository.GetAssetCategoryNameById(request.AssetComposite.AssetMaster.AssetSubCategoryId);

                    // Instead of returning a value, throw an exception for invalid data.
                    if (string.IsNullOrWhiteSpace(UnitName) || string.IsNullOrWhiteSpace(assetGroupName) || string.IsNullOrWhiteSpace(assetCategoryName))
                    {
                        throw new Exception("Invalid data: Company, Asset Group, or Asset SubCategory is missing.");
                    }

                    // Get latest AssetCode
                    var latestAssetCode = await _assetMasterGeneralRepository.GetLatestAssetCode(
                        request.AssetComposite.AssetMaster.CompanyId,
                        request.AssetComposite.AssetMaster.UnitId,
                        request.AssetComposite.AssetMaster.AssetGroupId,
                        request.AssetComposite.AssetMaster.AssetSubCategoryId);

                    // Extract sequence number
                    int sequence = 1;
                    if (!string.IsNullOrEmpty(latestAssetCode))
                    {
                        var parts = latestAssetCode.Split('/');
                        if (parts.Length == 4 && int.TryParse(parts[3], out int lastSeq))
                        {
                            sequence = lastSeq + 1;
                        }
                    }

                    // Generate Asset Code
                    var assetCode = $"{UnitName}-{assetGroupName}-{assetCategoryName}-{sequence}";

                    // Map asset master from composite command DTO.
                    var assetMaster = _mapper.Map<AssetMasterGenerals>(request.AssetComposite.AssetMaster);
                    assetMaster.AssetCode = assetCode;

                    // Initialize child collections.
                    assetMaster.AssetPurchase = new List<AssetPurchaseDetails>();
                    assetMaster.AssetLocations = new List<Core.Domain.Entities.AssetMaster.AssetLocation>(); 

                    // Map purchase details if provided.
                    if (request.AssetComposite.AssetPurchaseDetails is not null)
                    {
                        foreach (var purchaseDto in request.AssetComposite.AssetPurchaseDetails)
                        {
                            var purchaseDetail = _mapper.Map<AssetPurchaseDetails>(purchaseDto);
                            assetMaster.AssetPurchase.Add(purchaseDetail);
                        }
                    }

                    // If needed, map location details here similarly.
                     if (request.AssetComposite.AssetLocation is not null)
                     {
                         foreach (var locationDto in request.AssetComposite.AssetLocation)
                         {
                             var locationDetail = _mapper.Map<Core.Domain.Entities.AssetMaster.AssetLocation>(locationDto);
                             assetMaster.AssetLocations.Add(locationDetail);
                         }
                    }

                    // Persist the asset master (with its child entities).
                    var createdAsset = await _assetMasterGeneralRepository.CreateAsync(assetMaster, cancellationToken);
                    assetId = createdAsset.Id;

                   /*  // After persisting, assign the generated assetId to each purchase detail.
                    foreach (var purchase in assetMaster.AssetPurchase)
                    {
                        purchase.AssetId = assetId;
                    } */
                }, cancellationToken);

                if (assetId > 0)
                {
                    // Retrieve the created asset via the query repository and map to a DTO.
                    var createdAssetEntity = await _assetMasterQueryGeneralRepository.GetByIdAsync(assetId);
                    var assetDTO = _mapper.Map<AssetMasterGeneralDTO>(createdAssetEntity);

                    response.IsSuccess = true;
                    response.Message = "AssetMasterGeneral created successfully.";
                    response.Data = assetDTO;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "AssetMasterGeneral not created.";
                }
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
