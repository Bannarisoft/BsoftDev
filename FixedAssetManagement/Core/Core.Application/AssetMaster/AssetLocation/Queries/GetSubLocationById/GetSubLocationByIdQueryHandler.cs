using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetLocation;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetLocation.Queries.GetSubLocationById
{
    public class GetSubLocationByIdQueryHandler  : IRequestHandler<GetSubLocationByIdQuery, ApiResponseDTO<List<GetAssetSubLocationDto>>>
    {
         private readonly IAssetLocationQueryRepository _assetLocationRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

         public GetSubLocationByIdQueryHandler(IAssetLocationQueryRepository assetLocationRepository,  IMapper mapper, IMediator mediator)
        {
            _assetLocationRepository = assetLocationRepository;
            _mapper = mapper;
            _mediator = mediator;

        }
        public async Task<ApiResponseDTO<List<GetAssetSubLocationDto>>> Handle(GetSubLocationByIdQuery request, CancellationToken cancellationToken)
                {
                    var assetLocations = await _assetLocationRepository.GetSublocationByIdAsync(request.Id); // Fetch list

                    if (assetLocations == null ) // Check if empty
                    {
                        return new ApiResponseDTO<List<GetAssetSubLocationDto>>
                        {
                            IsSuccess = false,
                            Message = $"No AssetSub Locations found for ID {request.Id}.",
                            Data = new List<GetAssetSubLocationDto>()
                        };
                    }

                    var assetLocationsDto = _mapper.Map<List<GetAssetSubLocationDto>>(assetLocations); // Map list

                    // Domain Event
                    var domainEvent = new AuditLogsDomainEvent(
                        actionDetail: "GetAll",
                        actionCode: "",        
                        actionName: "", 
                        details: $"Asset Sub Location details were fetched for ID {request.Id}.",
                        module: "AssetSub Location"
                    );
                    await _mediator.Publish(domainEvent, cancellationToken);

                    return new ApiResponseDTO<List<GetAssetSubLocationDto>> 
                    { 
                        IsSuccess = true, 
                        Message = "Success", 
                        Data = assetLocationsDto 
                    };
                }
                    // public async Task<ApiResponseDTO<GetAssetSubLocationDto>> Handle(GetSubLocationByIdQuery request, CancellationToken cancellationToken)
        // public async Task<ApiResponseDTO<List<GetAssetSubLocationDto>>> Handle(GetSubLocationByIdQuery request, CancellationToken cancellationToken)
        // {
        //      var assetLocation = await _assetLocationRepository.GetSublocationByIdAsync(request.Id);
           

        //     var assetlocationDto = _mapper.Map<GetAssetSubLocationDto>(assetLocation);
         

        //     if (assetLocation is null)
        //     {                
        //         return new ApiResponseDTO<List<GetAssetSubLocationDto>>
        //         {
        //             IsSuccess = false,
        //             Message = "AssetLocation with ID {request.Id} not found."
        //         };   
        //     }       
        //     //Domain Event
        //     var domainEvent = new AuditLogsDomainEvent(
        //         actionDetail: "GetById",
        //         actionCode: assetlocationDto.Id == null ? "" : assetlocationDto.Id.ToString(),        
        //         actionName: assetlocationDto.SubLocationName == null ? "" : assetlocationDto.SubLocationName.ToString(),                
        //         details: $"Asset Location '{assetlocationDto.Code}' Retrieved. Code: {assetlocationDto.SubLocationName}",
        //         module:"AssetLocation"
        //     );
        //     await _mediator.Publish(domainEvent, cancellationToken);
        //      return new ApiResponseDTO<List<GetAssetSubLocationDto>>
        //     {
        //         IsSuccess = true,
        //         Message = "Success",
        //         Data = assetlocationDto
        //     };       
        //}       
        
    }
}