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

namespace Core.Application.AssetGroup.Command.DeleteAssetGroup
{
    public class DeleteAssetGroupCommandHandler : IRequestHandler<DeleteAssetGroupCommand, ApiResponseDTO<int>>
    {
        private readonly IAssetGroupCommandRepository _iAssetGroupCommandRepository;
        private readonly IAssetGroupQueryRepository _iAssetGroupQueryRepository;
        private readonly IMapper _Imapper;
        private readonly ILogger<DeleteAssetGroupCommandHandler> _logger;
        private readonly IMediator _mediator; 

        public DeleteAssetGroupCommandHandler(IAssetGroupCommandRepository iAssetGroupCommandRepository, IMapper imapper, ILogger<DeleteAssetGroupCommandHandler> logger, IMediator mediator, IAssetGroupQueryRepository iAssetGroupQueryRepository)
        {
            _iAssetGroupCommandRepository = iAssetGroupCommandRepository;
            _Imapper = imapper;
            _logger = logger;
            _mediator = mediator;
            _iAssetGroupQueryRepository = iAssetGroupQueryRepository;
        }

        public async Task<ApiResponseDTO<int>> Handle(DeleteAssetGroupCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Starting DeleteAssetGroupCommandHandler for request: {request}");

            // 🔹 First, check if the ID exists in the database
            var existingAssetGroup = await _iAssetGroupQueryRepository.GetByIdAsync(request.Id);
            if (existingAssetGroup is null)
            {
                _logger.LogWarning($"AssetGroup ID {request.Id} not found.");
                return new ApiResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = "AssetGroup Id not found / AssetGroup is deleted ."
                };
            }

            var assetGroup = _Imapper.Map<Core.Domain.Entities.AssetGroup>(request);
            var result = await _iAssetGroupCommandRepository.DeleteAsync(request.Id,assetGroup);
            if (result == -1) 
            {
            _logger.LogInformation($"AssetGroup {request.Id} not found.");
             return new ApiResponseDTO<int> { IsSuccess = false, Message = "AssetGroupId not found."};
            }

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Delete",
                actionCode: assetGroup.Code,
                actionName: assetGroup.GroupName,
                details: $"AssetGroup details was deleted",
                module: "AssetGroup");
            await _mediator.Publish(domainEvent);
            _logger.LogInformation($"AssetGroup {assetGroup.GroupName} Deleted successfully.");

            return new ApiResponseDTO<int>
            {
                IsSuccess = true,   
                Data = result,
                Message = "AssetGroup deleted successfully."
    
            };
        }
    }
}