using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetPurchase;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetPurchase.Commands.UpdateAssetPurchaseDetails
{
    public class UpdateAssetPurchaseDetailCommandHandler : IRequestHandler<UpdateAssetPurchaseDetailCommand, ApiResponseDTO<int>>
    {
        private readonly  IAssetPurchaseCommandRepository _iassetPurchaseCommandRepository;
        private readonly IAssetPurchaseQueryRepository _iAssetPurchaseQueryRepository;
        private readonly IMapper _Imapper;
        private readonly IMediator _mediator;
        public UpdateAssetPurchaseDetailCommandHandler(IAssetPurchaseCommandRepository iassetPurchaseCommandRepository, IMapper imapper, IMediator mediator, IAssetPurchaseQueryRepository iAssetPurchaseQueryRepository)
        {
            _iassetPurchaseCommandRepository = iassetPurchaseCommandRepository;
            _Imapper = imapper;
            _mediator = mediator;
            _iAssetPurchaseQueryRepository = iAssetPurchaseQueryRepository;
        }

        public async Task<ApiResponseDTO<int>> Handle(UpdateAssetPurchaseDetailCommand request, CancellationToken cancellationToken)
        {
             // 🔹 First, check if the ID exists in the database
        var existingassetpurchaseId = await _iAssetPurchaseQueryRepository.GetByIdAsync(request.Id);
        if (existingassetpurchaseId is null)
        {
      
        return new ApiResponseDTO<int>
        {
            IsSuccess = false,
            Message = "AssetPurchaseDetails Id not found ."
        };
        }
         var assetPurchaseDetails = _Imapper.Map<Core.Domain.Entities.AssetPurchase.AssetPurchaseDetails>(request);
        var result = await _iassetPurchaseCommandRepository.UpdateAsync(request.Id, assetPurchaseDetails);
        if (result <= 0) // AssetGroup not found
        {
          
            return new ApiResponseDTO<int> { IsSuccess = false, Message = "AssetPurchaseDetails not found." };
        }
        //Domain Event
        var domainEvent = new AuditLogsDomainEvent(
            actionDetail: "Update",
            actionCode: assetPurchaseDetails.Id.ToString(),
            actionName: assetPurchaseDetails.ItemName,
            details: $"AssetPurchase details was updated",
            module: "AssetPurchaseDetails");
        await _mediator.Publish(domainEvent, cancellationToken);
        return new ApiResponseDTO<int> { IsSuccess = true, Message = "Success.", Data = result };   

        }
    }
}