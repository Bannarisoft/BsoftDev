using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetAmc;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetAmc.Command.UpdateAssetAmc
{
    public class UpdateAssetAmcCommandHandler : IRequestHandler<UpdateAssetAmcCommand, ApiResponseDTO<int>>
    {
        private readonly IAssetAmcCommandRepository _iassetamccommandrepository;
        private readonly IMediator _imediator;
        private readonly IMapper _imapper;
        public UpdateAssetAmcCommandHandler(IAssetAmcCommandRepository iassetamccommandrepository, IMediator imediator, IMapper imapper)
        {
            _iassetamccommandrepository = iassetamccommandrepository;
            _imediator = imediator;
            _imapper = imapper; 
        }

        public async Task<ApiResponseDTO<int>> Handle(UpdateAssetAmcCommand request, CancellationToken cancellationToken)
        {
                var assetamc = _imapper.Map<Core.Domain.Entities.AssetMaster.AssetAmc>(request);
                // Calculate EndDate based on StartDate and Period (in months)
                if (request.StartDate.HasValue && request.Period.HasValue)
                {
                assetamc.EndDate = request.StartDate.Value.AddMonths(request.Period.Value);
                }
            
                var result = await _iassetamccommandrepository.UpdateAsync(request.Id, assetamc);
                if (result <= 0) // AssetAmc not found
                {
                    return new ApiResponseDTO<int> { IsSuccess = false, Message = "AssetAmc not found." };
                }
                //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Update",
                    actionCode: assetamc.Id.ToString(),
                    actionName: assetamc.AssetId.ToString(),
                    details: $"AssetAmc details was updated",
                    module: "AssetAmc");
                await _imediator.Publish(domainEvent, cancellationToken);
                return new ApiResponseDTO<int> { IsSuccess = true, Message = "AssetAmc Updated Successfully.", Data = result }; 
        }
    }
}