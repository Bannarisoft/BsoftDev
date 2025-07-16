using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetMaster.AssetAdditionalCost.Queries.GetAssetAdditionalCost;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetAdditionalCost;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetAdditionalCost.Commands.CreateAssetAdditionalCost
{
    public class CreateAssetAdditionalCostCommandHandler : IRequestHandler<CreateAssetAdditionalCostCommand, ApiResponseDTO<int>>
    {
        private readonly IAssetAdditionalCostCommandRepository _iAssetAdditionalCostCommandRepository;
        private readonly IMediator _imediator;
        private readonly IMapper _imapper;
        public CreateAssetAdditionalCostCommandHandler(IAssetAdditionalCostCommandRepository iAssetAdditionalCostCommandRepository, IMediator imediator, IMapper imapper)
        {
            _iAssetAdditionalCostCommandRepository = iAssetAdditionalCostCommandRepository;
            _imediator = imediator;
            _imapper = imapper;
          
        }
        public async Task<ApiResponseDTO<int>> Handle(CreateAssetAdditionalCostCommand request, CancellationToken cancellationToken)
        {
             var assetAdditionalCost = _imapper.Map<Core.Domain.Entities.AssetPurchase.AssetAdditionalCost>(request);
            
            var result = await _iAssetAdditionalCostCommandRepository.CreateAsync(assetAdditionalCost);

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: assetAdditionalCost.AssetId.ToString(),
                actionName: assetAdditionalCost.CostType.ToString(),
                details: $"AssetAdditionalCost details was created",
                module: "AssetAdditionalCost");
            await _imediator.Publish(domainEvent, cancellationToken);
            var assetGroupDtoDto = _imapper.Map<AssetAdditionalCostDto>(assetAdditionalCost);
            if (result > 0)
                  {
                     
                        return new ApiResponseDTO<int>
                       {
                           IsSuccess = true,
                           Message = "AssetAdditionalCost created successfully",
                           Data = result
                      };
                 }
            return new ApiResponseDTO<int>
            {
                IsSuccess = true,
                Message = "AssetAdditionalCost Creation Failed",
                Data = result
            };
        }
    }
}