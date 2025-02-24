using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.AssetMaster.AssetWarranty.Queries.GetAssetWarranty;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetWarranty;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetWarranty.Queries.GetAssetWarrantyById
{
    public class GetAssetWarrantyByIdQueryHandler : IRequestHandler<GetAssetWarrantyByIdQuery, ApiResponseDTO<AssetWarrantyDTO>>
    {
        private readonly IAssetWarrantyQueryRepository _assetWarrantyRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

        public GetAssetWarrantyByIdQueryHandler(IAssetWarrantyQueryRepository assetWarrantyRepository,  IMapper mapper, IMediator mediator)
        {
            _assetWarrantyRepository =assetWarrantyRepository;
            _mapper =mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<AssetWarrantyDTO>> Handle(GetAssetWarrantyByIdQuery request, CancellationToken cancellationToken)
        {
            var assetWarranty = await _assetWarrantyRepository.GetByIdAsync(request.Id);                
            var assetWarrantyDto = _mapper.Map<AssetWarrantyDTO>(assetWarranty);
            if (assetWarranty is null)
            {                
                return new ApiResponseDTO<AssetWarrantyDTO>
                {
                    IsSuccess = false,
                    Message = "AssetWarranty with ID {request.Id} not found."
                };   
            }       
                //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetById",
                actionCode: assetWarrantyDto.AssetId.ToString(),
                actionName: assetWarrantyDto.WarrantyType.ToString() ?? string.Empty,
                details: $"WarrantyMaster '{assetWarrantyDto.Description}' was created",
                module:"WarrantyMaster"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<AssetWarrantyDTO>
            {
                IsSuccess = true,
                Message = "Success",
                Data = assetWarrantyDto
            };       
        }
    }
}