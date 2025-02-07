using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetGroup.Queries.GetAssetGroup;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetGroup;
using Core.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.AssetGroup.Command.CreateAssetGroup
{
    public class CreateAssetGroupCommandHandler : IRequestHandler<CreateAssetGroupCommand, ApiResponseDTO<int>>
    {
        private readonly IAssetGroupCommandRepository _iAssetGroupCommandRepository;
        private readonly IMediator _imediator;
        private readonly IMapper _imapper;
        private readonly ILogger<CreateAssetGroupCommandHandler> _logger;

        public CreateAssetGroupCommandHandler(IAssetGroupCommandRepository IAssetGroupCommandRepository, IMediator imediator, IMapper imapper, ILogger<CreateAssetGroupCommandHandler> logger)
        {
            _iAssetGroupCommandRepository = IAssetGroupCommandRepository;
            _imediator = imediator;
            _imapper = imapper;
            _logger = logger;
        }
        public async Task<ApiResponseDTO<int>> Handle(CreateAssetGroupCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Starting creation process for AssetGroup: {request}");
             // Check if AssetGroup code already exists
            var exists = await _iAssetGroupCommandRepository.ExistsByCodeAsync(request.Code);
            if (exists)
            {
                 _logger.LogWarning($"AssetGroup Code {request.Code} already exists.");
                 return new ApiResponseDTO<int>
            {
            IsSuccess = false,
            Message = "AssetGroup Code already exists.",
            Data = 0
            };
            }
            var assetGroup = _imapper.Map<Core.Domain.Entities.AssetGroup>(request);
            
            var result = await _iAssetGroupCommandRepository.CreateAsync(assetGroup);

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: assetGroup.Code,
                actionName: assetGroup.GroupName,
                details: $"AssetGroup details was created",
                module: "AssetGroup");
            await _imediator.Publish(domainEvent, cancellationToken);
            _logger.LogInformation($"AssetGroup {assetGroup.GroupName} Created successfully.");
            var assetGroupDtoDto = _imapper.Map<AssetGroupDto>(assetGroup);

            return new ApiResponseDTO<int>()
            {
                IsSuccess = true,
                Message = "AssetGroup Created Successfully",
                Data = assetGroupDtoDto.Id
            };
        }
    }
}