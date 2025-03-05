using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetAmc;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetAmc.Command.DeleteAssetAmc
{
    public class DeleteAssetAmcCommandHandler : IRequestHandler<DeleteAssetAmcCommand, ApiResponseDTO<int>>
    {
        private readonly IAssetAmcCommandRepository _iassetamccommandrepository ;
        private readonly IAssetAmcQueryRepository _iassetamcqueryrepository ;
        private readonly IMapper _Imapper;
        private readonly IMediator _mediator; 
        public DeleteAssetAmcCommandHandler(IAssetAmcCommandRepository iassetamccommandrepository, IMapper imapper,  IMediator mediator, IAssetAmcQueryRepository iassetamcqueryrepository)
        {
            _iassetamccommandrepository = iassetamccommandrepository;
            _Imapper = imapper;
            _mediator = mediator;
            _iassetamcqueryrepository = iassetamcqueryrepository;
        }

        public async Task<ApiResponseDTO<int>> Handle(DeleteAssetAmcCommand request, CancellationToken cancellationToken)
        {
            // 🔹 First, check if the ID exists in the database
            var existingAssetamc = await _iassetamcqueryrepository.GetByIdAsync(request.Id);
            if (existingAssetamc is null)
            {
                
                return new ApiResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = "AssetAmc Id not found / AssetAmc is deleted ."
                };
            }

            var assetamc = _Imapper.Map<Core.Domain.Entities.AssetMaster.AssetAmc>(request);
            var result = await _iassetamccommandrepository.DeleteAsync(request.Id,assetamc);
            if (result == -1) 
            {
            
             return new ApiResponseDTO<int> { IsSuccess = false, Message = "AssetAmc not found."};
            }

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Delete",
                actionCode: assetamc.Id.ToString(),
                actionName: assetamc.AssetId.ToString(),
                details: $"AssetAmc details was deleted",
                module: "AssetAmc");
                await _mediator.Publish(domainEvent);
       

            return new ApiResponseDTO<int>
            {
                IsSuccess = true,   
                Data = result,
                Message = "AssetAmc deleted successfully."
    
            };
        }
    }
}