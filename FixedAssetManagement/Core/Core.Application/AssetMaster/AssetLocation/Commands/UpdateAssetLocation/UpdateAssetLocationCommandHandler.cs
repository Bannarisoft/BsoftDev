using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetLocation.Queries.GetAssetLocation;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetLocation;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetLocation.Commands.UpdateAssetLocation
{
    public class UpdateAssetLocationCommandHandler : IRequestHandler<UpdateAssetLocationCommand, ApiResponseDTO<int>>
    {
    private readonly IAssetLocationCommandRepository _assetLocationRepository;
    private readonly IAssetLocationQueryRepository _assetLocationQueryRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public UpdateAssetLocationCommandHandler(IAssetLocationCommandRepository assetLocationRepository, IMapper mapper,
      IAssetLocationQueryRepository assetLocationQueryRepository, IMediator mediator)
    {
        _assetLocationRepository = assetLocationRepository;
        _mapper = mapper;
        _assetLocationQueryRepository = assetLocationQueryRepository;
        _mediator = mediator;
    }

    public async Task<ApiResponseDTO<int>> Handle(UpdateAssetLocationCommand request, CancellationToken cancellationToken)
    {
            var assetLocation = await _assetLocationQueryRepository.GetByIdAsync(request.AssetId);
                
                if (assetLocation == null)
                {
                    return new ApiResponseDTO<int>
                    {
                        IsSuccess = false,
                        Message = "AssetLocation not found."
                    };
                }
                // ✅ Correct AutoMapper mapping (map request into existing entity)
                _mapper.Map(request, assetLocation);

                // ✅ Pass the updated assetLocation to repository
                var updateResult = await _assetLocationRepository.UpdateAsync(request.AssetId, assetLocation);

                // ✅ Ensure meaningful action name
                var actionName = updateResult > 0 ? "Update Successful" : "Update Failed";

                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Update",
                    actionCode: request.AssetId.ToString(),
                    actionName: actionName,
                    details: $"AssetLocation '{request.AssetId}' was updated.",
                    module: "AssetLocation"
                );

                await _mediator.Publish(domainEvent, cancellationToken);

                if (updateResult >0)  // ✅ Check if update was successful
                {
                    return new ApiResponseDTO<int>
                    {
                        IsSuccess = true,
                        Message = "AssetLocation updated successfully.",
                        Data = request.AssetId  // Return updated ID
                    };
                }

                return new ApiResponseDTO<int>
                {
                    IsSuccess = false,
                    Message = "AssetLocation not updated."
                };
                

    }

    }
}