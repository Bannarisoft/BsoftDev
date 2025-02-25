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
    public class UpdateAssetLocationCommandHandler : IRequestHandler<UpdateAssetLocationCommand, ApiResponseDTO<AssetLocationDto>>
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

    public async Task<ApiResponseDTO<AssetLocationDto>> Handle(UpdateAssetLocationCommand request, CancellationToken cancellationToken)
    {
        var assetLocation = await _assetLocationQueryRepository.GetByIdAsync(request.AssetId);
        
        if (assetLocation == null)
        {
            return new ApiResponseDTO<AssetLocationDto>
            {
                IsSuccess = false,
                Message = "AssetLocation not found."
            };
        }

        // Update properties
        assetLocation.UnitId = request.UnitId;
        assetLocation.DepartmentId = request.DepartmentId;
        assetLocation.LocationId = request.LocationId;
        assetLocation.SubLocationId = request.SubLocationId;
        assetLocation.CustodianId = request.CustodianId;
        assetLocation.UserID = request.UserID;
    

        var updateResult = await _assetLocationRepository.UpdateAsync(request.AssetId, assetLocation);

        if (updateResult  is true)
        {
            var updatedAssetLocation = await _assetLocationQueryRepository.GetByIdAsync(request.AssetId);
            var assetLocationDto = _mapper.Map<AssetLocationDto>(updatedAssetLocation);

            return new ApiResponseDTO<AssetLocationDto>
            {
                IsSuccess = true,
                Message = "AssetLocation updated successfully.",
                Data = assetLocationDto
            };
        }
        else
        {
            return new ApiResponseDTO<AssetLocationDto>
            {
                IsSuccess = false,
                Message = "AssetLocation update failed."
            };
        }
    }

    }
}