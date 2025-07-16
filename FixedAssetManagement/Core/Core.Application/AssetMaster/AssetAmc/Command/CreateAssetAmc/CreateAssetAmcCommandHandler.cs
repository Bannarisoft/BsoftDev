using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetMaster.AssetAmc.Queries.GetAssetAmc;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetAmc;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetAmc.Command.CreateAssetAmc
{
    public class CreateAssetAmcCommandHandler : IRequestHandler<CreateAssetAmcCommand, ApiResponseDTO<int>>
    {
        private readonly IAssetAmcCommandRepository _iassetamccommandrepository;
        private readonly IMediator _imediator;
        private readonly IMapper _imapper;

        public CreateAssetAmcCommandHandler(IAssetAmcCommandRepository iassetamccommandrepository, IMediator imediator, IMapper imapper)
        {
            _iassetamccommandrepository = iassetamccommandrepository;
            _imediator = imediator;
            _imapper = imapper;
          
        }

        public async Task<ApiResponseDTO<int>> Handle(CreateAssetAmcCommand request, CancellationToken cancellationToken)
        {
            var assetAmc = _imapper.Map<Core.Domain.Entities.AssetMaster.AssetAmc>(request);


            // Ensure RenewedDate is properly handled
            assetAmc.RenewedDate = request.RenewedDate ?? null;


              // Calculate EndDate based on StartDate and Period (in months)
            if (request.StartDate.HasValue && request.Period.HasValue)
            {
                assetAmc.EndDate = request.StartDate.Value.AddMonths(request.Period.Value);
            }
            
            var result = await _iassetamccommandrepository.CreateAsync(assetAmc);

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: assetAmc.AssetId.ToString(),
                actionName: assetAmc.Id.ToString(),
                details: $"AssetAmc details was created",
                module: "AssetAmc");
            await _imediator.Publish(domainEvent, cancellationToken);
            var assetamcDto = _imapper.Map<AssetAmcDto>(assetAmc);
            if (result > 0)
                  {
                     
                        return new ApiResponseDTO<int>
                       {
                           IsSuccess = true,
                           Message = "AssetAmc created successfully",
                           Data = result
                      };
                 }
            return new ApiResponseDTO<int>
            {
                IsSuccess = true,
                Message = "AssetAmc Creation Failed",
                Data = result
            };
        }
    }
}