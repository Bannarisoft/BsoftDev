using AutoMapper;
using Core.Application.AssetMaster.AssetSpecification.Queries.GetAssetSpecification;
using Core.Application.AssetMaster.AssetSpecification.Queries.GetAssetSpecificationById;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetSpecification;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetSpecification.Queries.GetAssetSpecificationById
{
    public class GetAssetSpecificationByIdQueryHandler  : IRequestHandler<GetAssetSpecificationByIdQuery, ApiResponseDTO<AssetSpecificationDTO>>
    {
        private readonly IAssetSpecificationQueryRepository _assetSpecificationRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

        public GetAssetSpecificationByIdQueryHandler(IAssetSpecificationQueryRepository assetSpecificationRepository,  IMapper mapper, IMediator mediator)
        {
            _assetSpecificationRepository =assetSpecificationRepository;
            _mapper =mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<AssetSpecificationDTO>> Handle(GetAssetSpecificationByIdQuery request, CancellationToken cancellationToken)
        {
            var assetSpecification = await _assetSpecificationRepository.GetByIdAsync(request.Id);                
            var assetSpecificationDto = _mapper.Map<AssetSpecificationDTO>(assetSpecification);
            if (assetSpecification is null)
            {                
                return new ApiResponseDTO<AssetSpecificationDTO>
                {
                    IsSuccess = false,
                    Message = "AssetSpecification with ID {request.Id} not found."
                };   
            }       
                //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetById",
                actionCode: assetSpecificationDto.AssetId.ToString(),
                actionName: assetSpecificationDto.SpecificationId.ToString(),
                details: $"SpecificationMaster '{assetSpecificationDto.SpecificationValue}' was created",
                module:"SpecificationMaster"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<AssetSpecificationDTO>
            {
                IsSuccess = true,
                Message = "Success",
                Data = assetSpecificationDto
            };       
        }
    }
}