
using AutoMapper;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral;
using Core.Domain.Common;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Commands.DeleteAssetMasterGeneral
{
    public class DeleteAssetMasterGeneralCommandHandler : IRequestHandler<DeleteAssetMasterGeneralCommand, ApiResponseDTO<AssetMasterGeneralDTO>>
    {
        private readonly IAssetMasterGeneralCommandRepository _assetMasterGeneralRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 
        private readonly IAssetMasterGeneralQueryRepository _assetMasterGeneralQueryRepository;
        
        public DeleteAssetMasterGeneralCommandHandler(IAssetMasterGeneralCommandRepository assetMasterGeneralRepository, IMapper mapper,  IMediator mediator,IAssetMasterGeneralQueryRepository assetMasterGeneralQueryRepository)
        {
            _assetMasterGeneralRepository = assetMasterGeneralRepository;
             _mapper = mapper;        
            _mediator = mediator;
            _assetMasterGeneralQueryRepository=assetMasterGeneralQueryRepository;
        }

        public async Task<ApiResponseDTO<AssetMasterGeneralDTO>> Handle(DeleteAssetMasterGeneralCommand request, CancellationToken cancellationToken)
        {
            var assetMasterGeneral = await _assetMasterGeneralQueryRepository.GetByIdAsync(request.Id);
            if (assetMasterGeneral is null || assetMasterGeneral.IsDeleted is BaseEntity.IsDelete.Deleted )
            {
                return new ApiResponseDTO<AssetMasterGeneralDTO>
                {
                    IsSuccess = false,
                    Message = "Invalid AssetId. The specified AssetName does not exist or is inactive."
                };
            }
            var assetMasterDelete = _mapper.Map<AssetMasterGenerals>(request);      
            var updateResult = await _assetMasterGeneralRepository.DeleteAsync(request.Id, assetMasterDelete);
            if (updateResult > 0)
            {
                var assetMasterDto = _mapper.Map<AssetMasterGeneralDTO>(assetMasterDelete);  
                //Domain Event  
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Delete",
                    actionCode: assetMasterDelete.AssetCode ?? string.Empty,
                    actionName: assetMasterDelete.AssetName ?? string.Empty,
                    details: $"AssetMaster '{assetMasterDto.AssetName}' was created. Code: {assetMasterDto.AssetCode}",
                    module:"AssetMaster"
                );               
                await _mediator.Publish(domainEvent, cancellationToken);                 
                return new ApiResponseDTO<AssetMasterGeneralDTO>
                {
                    IsSuccess = true,
                    Message = "AssetMaster deleted successfully.",
                    Data = assetMasterDto
                };
            }

            return new ApiResponseDTO<AssetMasterGeneralDTO>
            {
                IsSuccess = false,
                Message = "AssetMaster deletion failed."                             
            };
        }
    }
}