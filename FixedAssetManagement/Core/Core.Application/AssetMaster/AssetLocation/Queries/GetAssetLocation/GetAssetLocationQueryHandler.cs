using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetLocation;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetLocation.Queries.GetAssetLocation
{
    public class GetAssetLocationQueryHandler  : IRequestHandler<GetAssetLocationQuery, ApiResponseDTO<List<AssetLocationDto>>>
    {
        private readonly IAssetLocationQueryRepository _assetLocationRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetAssetLocationQueryHandler(IAssetLocationQueryRepository assetLocationRepository, IMapper mapper, IMediator mediator)
        {
            _assetLocationRepository = assetLocationRepository;
            _mapper = mapper;
            _mediator = mediator;
        } 
      
          public async Task<ApiResponseDTO<List<AssetLocationDto>>> Handle(GetAssetLocationQuery request, CancellationToken cancellationToken)
        {
            var (assetlocation, totalCount) = await _assetLocationRepository.GetAllAssetLocationAsync(request.PageNumber, request.PageSize, request.SearchTerm);
            var assetMasterList = _mapper.Map<List<AssetLocationDto>>(assetlocation);

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetAll",
                actionCode: "",        
                actionName: "",
                details: $"DepreciationGroup details was fetched.",
                module:"DepreciationGroup"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<AssetLocationDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = assetMasterList,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };          
        }
        
    }

        
    }