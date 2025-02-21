using AutoMapper;
using Core.Application.AssetMaster.AssetSpecification.Queries.GetAssetSpecification;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetSpecification;
using Core.Domain.Entities.AssetMaster;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetSpecification.Commands.DeleteAssetSpecification
{
    public class DeleteAssetSpecificationCommandHandler : IRequestHandler<DeleteAssetSpecificationCommand, ApiResponseDTO<AssetSpecificationDTO>>
    {
        private readonly IAssetSpecificationCommandRepository _assetSpecificationRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 
        private readonly IAssetSpecificationQueryRepository _assetSpecificationQueryRepository;
        
        public DeleteAssetSpecificationCommandHandler(IAssetSpecificationCommandRepository assetSpecificationRepository, IMapper mapper,  IMediator mediator,IAssetSpecificationQueryRepository assetSpecificationQueryRepository)
        {
            _assetSpecificationRepository = assetSpecificationRepository;
             _mapper = mapper;        
            _mediator = mediator;
            _assetSpecificationQueryRepository=assetSpecificationQueryRepository;
        }

        public async Task<ApiResponseDTO<AssetSpecificationDTO>> Handle(DeleteAssetSpecificationCommand request, CancellationToken cancellationToken)
        {             
            var assetSpecifications = await _assetSpecificationQueryRepository.GetByIdAsync(request.Id);
            if (assetSpecifications is null )
            {
                return new ApiResponseDTO<AssetSpecificationDTO>
                {
                    IsSuccess = false,
                    Message = "Invalid DepreciationGroupID."
                };
            }
            var assetSpecificationDelete = _mapper.Map<AssetSpecifications>(request);      
            var updateResult = await _assetSpecificationRepository.DeleteAsync(request.Id, assetSpecificationDelete);
            if (updateResult > 0)
            {
                var assetSpecificationDto = _mapper.Map<AssetSpecificationDTO>(assetSpecificationDelete);  
                //Domain Event  
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Delete",
                    actionCode: assetSpecificationDelete.AssetId.ToString() ?? string.Empty,
                    actionName: assetSpecificationDelete.SpecificationId.ToString() ?? string.Empty,
                    details: $"AssetSpecification '{assetSpecificationDto.SpecificationValue}' was created.",
                    module:"AssetSpecification"
                );               
                await _mediator.Publish(domainEvent, cancellationToken);                 
                return new ApiResponseDTO<AssetSpecificationDTO>
                {
                    IsSuccess = true,
                    Message = "Asset Specification deleted successfully.",
                    Data = assetSpecificationDto
                };
            }
            return new ApiResponseDTO<AssetSpecificationDTO>
            {
                IsSuccess = false,
                Message = "Asset Specification deletion failed."                             
            };           
        }
    }
}