using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetGroup;
using Core.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.AssetGroup.Command.UpdateAssetGroup
{
    public class UpdateAssetGroupCommandHandler : IRequestHandler<UpdateAssetGroupCommand, ApiResponseDTO<int>>
    {
        private readonly IAssetGroupCommandRepository _iAssetGroupCommandRepository;
        private readonly IMapper _Imapper;
        private readonly ILogger<UpdateAssetGroupCommandHandler> _logger;
        private readonly IMediator _mediator; 

        public UpdateAssetGroupCommandHandler(IAssetGroupCommandRepository iAssetGroupCommandRepository, IMapper imapper, ILogger<UpdateAssetGroupCommandHandler> logger, IMediator mediator)
        {
            _iAssetGroupCommandRepository = iAssetGroupCommandRepository;
            _Imapper = imapper;
            _logger = logger;
            _mediator = mediator;
        }

        public Task<ApiResponseDTO<int>> Handle(UpdateAssetGroupCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        // public  Task<ApiResponseDTO<int>> Handle(UpdateAssetGroupCommand request, CancellationToken cancellationToken)
        // {
        //     // _logger.LogInformation($"Starting UpdateAssetGroupCommandHandler for request: {request}");
        // var assetGroup = _Imapper.Map<Core.Domain.Entities.AssetGroup>(request);
        // var result = await _iAssetGroupCommandRepository.UpdateAsync(request.Id, assetGroup);
        // if (result <= 0) // AssetGroup not found
        // {
        //     _logger.LogInformation($"AssetGroup {request.Id} not found.");
        //     return new ApiResponseDTO<int> { IsSuccess = false, Message = "AssetGroup not found." };
        // }
        // //Domain Event
        // var domainEvent = new AuditLogsDomainEvent(
        //     actionDetail: "Update",
        //     actionCode: assetGroup.Code,
        //     actionName: assetGroup.GroupName,
        //     details: $"AssetGroup details was updated",
        //     module: "AssetGroup");
        // await _mediator.Publish(domainEvent, cancellationToken);
        // _logger.LogInformation($"AssetGroup {assetGroup.GroupName} Updated successfully.");
        // return new ApiResponseDTO<int> { IsSuccess = true, Message = "AssetGroup Updated successfully.", Data = result };   
        // }
    }
}