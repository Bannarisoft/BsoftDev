using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetMaster.AssetDisposal.Queries.GetAssetDisposal;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetDisposal;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetDisposal.Command.CreateAssetDisposal
{
    public class CreateAssetDisposalCommandHandler : IRequestHandler<CreateAssetDisposalCommand, ApiResponseDTO<int>>
    {
        private readonly IAssetDisposalCommandRepository _iassetdisposalcommandrepository;
        private readonly IMediator _imediator;
        private readonly IMapper _imapper;
        public CreateAssetDisposalCommandHandler(IAssetDisposalCommandRepository iassetdisposalcommandrepository, IMediator imediator, IMapper imapper)
        {
            _iassetdisposalcommandrepository = iassetdisposalcommandrepository;
            _imediator = imediator;
            _imapper = imapper;
          
        }

        public async Task<ApiResponseDTO<int>> Handle(CreateAssetDisposalCommand request, CancellationToken cancellationToken)
        {
            var assetDisposal = _imapper.Map<Core.Domain.Entities.AssetMaster.AssetDisposal>(request);
            
            var result = await _iassetdisposalcommandrepository.CreateAsync(assetDisposal);

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: assetDisposal.AssetId.ToString(),
                actionName: assetDisposal.Id.ToString(),
                details: $"AssetDisposal details was created",
                module: "AssetDisposal");
            await _imediator.Publish(domainEvent, cancellationToken);
            var assetGroupDtoDto = _imapper.Map<AssetDisposalDto>(assetDisposal);
            if (result > 0)
                  {
                     
                        return new ApiResponseDTO<int>
                       {
                           IsSuccess = true,
                           Message = "AssetDisposal created successfully",
                           Data = result
                      };
                 }
            return new ApiResponseDTO<int>
            {
                IsSuccess = true,
                Message = "AssetDisposal Creation Failed",
                Data = result
            };
        }
    }
}