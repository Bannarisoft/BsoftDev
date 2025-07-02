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
        private readonly IAssetGroupQueryRepository _iAssetGroupQueryRepository;
        private readonly IMapper _Imapper;
        private readonly ILogger<UpdateAssetGroupCommandHandler> _logger;
        private readonly IMediator _mediator; 

        public UpdateAssetGroupCommandHandler(IAssetGroupCommandRepository iAssetGroupCommandRepository, IMapper imapper, ILogger<UpdateAssetGroupCommandHandler> logger, IMediator mediator, IAssetGroupQueryRepository iAssetGroupQueryRepository)
        {
            _iAssetGroupCommandRepository = iAssetGroupCommandRepository;
            _Imapper = imapper;
            _logger = logger;
            _mediator = mediator;
            _iAssetGroupQueryRepository = iAssetGroupQueryRepository;
        }

        public async  Task<ApiResponseDTO<int>> Handle(UpdateAssetGroupCommand request, CancellationToken cancellationToken)
        {
        _logger.LogInformation($"Starting UpdateAssetGroupCommandHandler for request: {request}");
        // 🔹 First, check if the ID exists in the database
        var existingassetGroup = await _iAssetGroupQueryRepository.GetByIdAsync(request.Id);
        if (existingassetGroup is null)
        {
        _logger.LogWarning($"AssetGroup ID {request.Id} not found.");
        return new ApiResponseDTO<int>
        {
            IsSuccess = false,
            Message = "AssetGroup Id not found / AssetGroup is deleted ."
        };
        }
         // Check for duplicate GroupName or SortOrder
       var (isNameDuplicate, isSortOrderDuplicate) = await _iAssetGroupCommandRepository
                                .CheckForDuplicatesAsync(request.GroupName, request.SortOrder, request.Id, request.GroupPercentage ?? 0);

        if (isNameDuplicate || isSortOrderDuplicate)
        {
            string errorMessage = isNameDuplicate && isSortOrderDuplicate
            ? "Both Group Name and Sort Order already exist."
            : isNameDuplicate
            ? "AssetGroup with the same Name already exists."
            : "AssetGroup with the same Sort Order already exists.";

             _logger.LogWarning($"Duplicate detected: {errorMessage}");

            return new ApiResponseDTO<int>
            {
                IsSuccess = false,
                Message = errorMessage
            };
        }
        var assetGroup = _Imapper.Map<Core.Domain.Entities.AssetGroup>(request);
        var result = await _iAssetGroupCommandRepository.UpdateAsync(request.Id, assetGroup);
        if (result <= 0) // AssetGroup not found
        {
            _logger.LogInformation($"AssetGroup {request.Id} not found.");
            return new ApiResponseDTO<int> { IsSuccess = false, Message = "AssetGroup not found." };
        }
        //Domain Event
        var domainEvent = new AuditLogsDomainEvent(
            actionDetail: "Update",
            actionCode: assetGroup.Code,
            actionName: assetGroup.GroupName,
            details: $"AssetGroup details was updated",
            module: "AssetGroup");
        await _mediator.Publish(domainEvent, cancellationToken);
        _logger.LogInformation($"AssetGroupId {result} Updated successfully.");
        return new ApiResponseDTO<int> { IsSuccess = true, Message = "AssetGroup Updated Successfully.", Data = result };   
        }
    }
}