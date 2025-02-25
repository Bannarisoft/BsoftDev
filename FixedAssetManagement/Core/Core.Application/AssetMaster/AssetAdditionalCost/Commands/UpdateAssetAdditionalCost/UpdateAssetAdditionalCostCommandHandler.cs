using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetAdditionalCost;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetAdditionalCost.Commands.UpdateAssetAdditionalCost
{
    public class UpdateAssetAdditionalCostCommandHandler : IRequestHandler<UpdateAssetAdditionalCostCommand, ApiResponseDTO<int>>
    {
        private readonly IAssetAdditionalCostCommandRepository _iAssetAdditionalCostCommandRepository;
        private readonly IMediator _imediator;
        private readonly IMapper _imapper;

        public UpdateAssetAdditionalCostCommandHandler(IAssetAdditionalCostCommandRepository iAssetAdditionalCostCommandRepository, IMediator imediator, IMapper imapper)
        {
            _iAssetAdditionalCostCommandRepository = iAssetAdditionalCostCommandRepository;
            _imediator = imediator;
            _imapper = imapper;
          
        }

        public async Task<ApiResponseDTO<int>> Handle(UpdateAssetAdditionalCostCommand request, CancellationToken cancellationToken)
        {
        var assetAdditionalCost = _imapper.Map<Core.Domain.Entities.AssetPurchase.AssetAdditionalCost>(request);
        var result = await _iAssetAdditionalCostCommandRepository.UpdateAsync(request.Id, assetAdditionalCost);
        if (result <= 0) // AssetGroup not found
        {
            return new ApiResponseDTO<int> { IsSuccess = false, Message = "AssetMasterId not found." };
        }
        //Domain Event
        var domainEvent = new AuditLogsDomainEvent(
            actionDetail: "Update",
            actionCode: assetAdditionalCost.Id.ToString(),
            actionName: assetAdditionalCost.CostType.ToString(),
            details: $"AssetAdditionalCost details was updated",
            module: "AssetAdditionalCost");
        await _imediator.Publish(domainEvent, cancellationToken);
        return new ApiResponseDTO<int> { IsSuccess = true, Message = "AssetAdditionalCost Updated Successfully.", Data = result };  
        }
    }
}