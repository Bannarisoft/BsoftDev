using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetSubGroup;
using Core.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.AssetSubGroup.Command.UpdateAssetSubGroup
{
    public class UpdateAssetSubGroupCommandHandler : IRequestHandler<UpdateAssetSubGroupCommand, ApiResponseDTO<int>>
    {
        private readonly IAssetSubGroupCommandRepository _iAssetSubGroupCommandRepository;
        private readonly IAssetSubGroupQueryRepository _iAssetSubGroupQueryRepository;
        private readonly IMapper _IMapper;
        private readonly ILogger<UpdateAssetSubGroupCommandHandler> _logger;
        private readonly IMediator _mediator; 

        public UpdateAssetSubGroupCommandHandler(IAssetSubGroupCommandRepository iAssetSubGroupCommandRepository, IMapper iMapper, ILogger<UpdateAssetSubGroupCommandHandler> logger, IMediator mediator, IAssetSubGroupQueryRepository iAssetSubGroupQueryRepository)
        {
            _iAssetSubGroupCommandRepository = iAssetSubGroupCommandRepository;
            _IMapper = iMapper;
            _logger = logger;
            _mediator = mediator;
            _iAssetSubGroupQueryRepository = iAssetSubGroupQueryRepository;
        }

        public async  Task<ApiResponseDTO<int>> Handle(UpdateAssetSubGroupCommand request, CancellationToken cancellationToken)
        {
        _logger.LogInformation($"Starting UpdateAssetSubGroupCommandHandler for request: {request}");
        // 🔹 First, check if the ID exists in the database
        var existingAssetSubGroup = await _iAssetSubGroupQueryRepository.GetByIdAsync(request.Id);
        if (existingAssetSubGroup is null)
        {
        _logger.LogWarning($"AssetSubGroup ID {request.Id} not found.");
        return new ApiResponseDTO<int>
        {
            IsSuccess = false,
            Message = "AssetSubGroup Id not found / AssetSubGroup is deleted ."
        };
        }
         // Check for duplicate GroupName or SortOrder
       var (isNameDuplicate, isSortOrderDuplicate) = await _iAssetSubGroupCommandRepository
                                .CheckForDuplicatesAsync(request.SubGroupName??string.Empty, request.SortOrder, request.Id);

        if (isNameDuplicate || isSortOrderDuplicate)
        {
            string errorMessage = isNameDuplicate && isSortOrderDuplicate
            ? "Both Group Name and Sort Order already exist."
            : isNameDuplicate
            ? "AssetSubGroup with the same Name already exists."
            : "AssetSubGroup with the same Sort Order already exists.";

             _logger.LogWarning($"Duplicate detected: {errorMessage}");

            return new ApiResponseDTO<int>
            {
                IsSuccess = false,
                Message = errorMessage
            };
        }
        var assetSubGroup = _IMapper.Map<Core.Domain.Entities.AssetSubGroup>(request);
        var result = await _iAssetSubGroupCommandRepository.UpdateAsync(request.Id, assetSubGroup);
        if (result <= 0) 
        {
            _logger.LogInformation($"AssetSubGroup {request.Id} not found.");
            return new ApiResponseDTO<int> { IsSuccess = false, Message = "AssetSubGroup not found." };
        }
        //Domain Event
        var domainEvent = new AuditLogsDomainEvent(
            actionDetail: "Update",
            actionCode: assetSubGroup.Code??string.Empty,
            actionName: assetSubGroup.SubGroupName??string.Empty,
            details: $"AssetSubGroup details was updated",
            module: "AssetSubGroup");
        await _mediator.Publish(domainEvent, cancellationToken);
        _logger.LogInformation($"AssetSubGroupId {result} Updated successfully.");
        return new ApiResponseDTO<int> { IsSuccess = true, Message = "AssetSubGroup Updated Successfully.", Data = result };   
        }
    }
}