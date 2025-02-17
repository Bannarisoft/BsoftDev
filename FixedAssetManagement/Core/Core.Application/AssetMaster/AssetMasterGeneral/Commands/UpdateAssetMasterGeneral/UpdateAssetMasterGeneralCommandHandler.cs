
using AutoMapper;
using Core.Application.AssetMaster.AssetMasterGeneral.Queries.GetAssetMasterGeneral;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetMasterGeneral;
using Core.Domain.Common;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.AssetMaster.AssetMasterGeneral.Commands.UpdateAssetMasterGeneral
{
    public class UpdateAssetMasterGeneralCommandHandler : IRequestHandler<UpdateAssetMasterGeneralCommand, ApiResponseDTO<AssetMasterGeneralDTO>>
    {
        private readonly IAssetMasterGeneralCommandRepository _assetMasterGeneralRepository;
        private readonly IAssetMasterGeneralQueryRepository _assetMasterGeneralQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

        public UpdateAssetMasterGeneralCommandHandler(IAssetMasterGeneralCommandRepository assetMasterGeneralRepository, IMapper mapper,IAssetMasterGeneralQueryRepository assetMasterGeneralQueryRepository, IMediator mediator)
        {
            _assetMasterGeneralRepository = assetMasterGeneralRepository;
            _mapper = mapper;
            _assetMasterGeneralQueryRepository = assetMasterGeneralQueryRepository;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<AssetMasterGeneralDTO>> Handle(UpdateAssetMasterGeneralCommand request, CancellationToken cancellationToken)
        {
            var assetMaster = await _assetMasterGeneralQueryRepository.GetByIdAsync(request.Id);
            if (assetMaster is null)
            return new ApiResponseDTO<AssetMasterGeneralDTO>
            {
                IsSuccess = false,
                Message = "Invalid AssetId. The specified AssetName does not exist or is inactive."
            };
            var oldAssetName = assetMaster.AssetName;
            assetMaster.AssetName = request.AssetName;

            if (assetMaster is null || assetMaster.IsDeleted is BaseEntity.IsDelete.Deleted )
            {
                return new ApiResponseDTO<AssetMasterGeneralDTO>
                {
                    IsSuccess = false,
                    Message = "Invalid AssetId. The specified AssetName does not exist or is deleted."
                };
            }
            if (assetMaster.IsActive != request.IsActive)
            {    
                 assetMaster.IsActive =  (BaseEntity.Status)request.IsActive;             
                await _assetMasterGeneralRepository.UpdateAsync(assetMaster.Id, assetMaster);
                if (request.IsActive is 0)
                {
                    return new ApiResponseDTO<AssetMasterGeneralDTO>
                    {
                        IsSuccess = false,
                        Message = "Code DeActivated."
                    };
                }
                else{
                    return new ApiResponseDTO<AssetMasterGeneralDTO>
                    {
                        IsSuccess = false,
                        Message = "Code Activated."
                    }; 
                }                                     
            }
            var updatedAssetMasterEntity = _mapper.Map<AssetMasterGenerals>(request);                   
            var updateResult = await _assetMasterGeneralRepository.UpdateAsync(request.Id, updatedAssetMasterEntity);            

            var updatedAssetMaster =  await _assetMasterGeneralQueryRepository.GetByIdAsync(request.Id);    
            if (updatedAssetMaster != null)
            {
                var assetMasterDto = _mapper.Map<AssetMasterGeneralDTO>(updatedAssetMaster);
                //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Update",
                    actionCode: assetMasterDto.AssetCode ?? string.Empty,
                    actionName: assetMasterDto.AssetName ?? string.Empty,                            
                    details: $"AssetMaster '{oldAssetName}' was updated to '{assetMasterDto.AssetName}'.  Code: {assetMasterDto.AssetCode}",
                    module:"AssetMasterGeneral"
                );            
                await _mediator.Publish(domainEvent, cancellationToken);
                if(updateResult>0)
                {
                    return new ApiResponseDTO<AssetMasterGeneralDTO>
                    {
                        IsSuccess = true,
                        Message = "AssetMaster updated successfully.",
                        Data = assetMasterDto
                    };
                }
                return new ApiResponseDTO<AssetMasterGeneralDTO>
                {
                    IsSuccess = false,
                    Message = "AssetMaster not updated."
                };                
            }
            else
            {
                return new ApiResponseDTO<AssetMasterGeneralDTO>{
                    IsSuccess = false,
                    Message = "AssetMaster not found."
                };
            }        
        }
    }
}