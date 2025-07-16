using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetMaster.AssetPurchase.Queries.GetAssetPurchase;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetPurchase;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetPurchase.Commands.CreateAssetPurchaseDetails
{
    public class CreateAssetPurchaseDetailCommandHandler  : IRequestHandler<CreateAssetPurchaseDetailCommand, ApiResponseDTO<int>>
    {
        private readonly IAssetPurchaseCommandRepository _iassetPurchaseCommandRepository;
        private readonly IMediator _imediator;
        private readonly IMapper _imapper;

          public CreateAssetPurchaseDetailCommandHandler(IAssetPurchaseCommandRepository iassetPurchaseCommandRepository, IMediator imediator, IMapper imapper)
        {
            _iassetPurchaseCommandRepository = iassetPurchaseCommandRepository;
            _imediator = imediator;
            _imapper = imapper;
            
        }

        public async Task<ApiResponseDTO<int>> Handle(CreateAssetPurchaseDetailCommand request, CancellationToken cancellationToken)
        {
            var assetPurchaseDetails = _imapper.Map<Core.Domain.Entities.AssetPurchase.AssetPurchaseDetails>(request);
             
             // Ensure CapitalizationDate is properly handled
            assetPurchaseDetails.CapitalizationDate = request.CapitalizationDate ?? null;

            var result = await _iassetPurchaseCommandRepository.CreateAsync(assetPurchaseDetails);

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: assetPurchaseDetails.Id.ToString(),
                actionName: assetPurchaseDetails.ItemName,
                details: $"AssetPurchase details was created",
                module: "AssetPurchaseDetails");
            await _imediator.Publish(domainEvent, cancellationToken);
            
            var assetPurchaseDetailsDto = _imapper.Map<AssetPurchaseDetailsDto>(assetPurchaseDetails);
            if (result > 0)
                  {
                    
                        return new ApiResponseDTO<int>
                       {
                           IsSuccess = true,
                           Message = "AssetPurchase created successfully",
                           Data = result
                      };
                 }
            return new ApiResponseDTO<int>
            {
                IsSuccess = true,
                Message = "AssetPurchase Creation Failed",
                Data = result
            };
        }
    }
}