
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
    public class UpdateAssetMasterGeneralCommandHandler : IRequestHandler<UpdateAssetMasterGeneralCommand, ApiResponseDTO<AssetMasterUpdateDto>>
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

        public async Task<ApiResponseDTO<AssetMasterUpdateDto>> Handle(UpdateAssetMasterGeneralCommand request, CancellationToken cancellationToken)
        {
            var assetMaster = await _assetMasterGeneralQueryRepository.GetByIdAsync(request.AssetMaster.Id);
            if (assetMaster is null)
            return new ApiResponseDTO<AssetMasterUpdateDto>
            {
                IsSuccess = false,
                Message = "Invalid AssetId. The specified AssetName does not exist or is inactive."
            };
            var oldAssetName = assetMaster.AssetName;
            assetMaster.AssetName = request.AssetMaster.AssetName;

            if (assetMaster is null || assetMaster.IsDeleted is BaseEntity.IsDelete.Deleted )
            {
                return new ApiResponseDTO<AssetMasterUpdateDto>
                {
                    IsSuccess = false,
                    Message = "Invalid AssetId. The specified AssetName does not exist or is deleted."
                };
            }
            if (assetMaster.IsActive != request.AssetMaster.IsActive)
            {    
                 assetMaster.IsActive =  (BaseEntity.Status)request.AssetMaster.IsActive;   
                 var updatedAssetMasterGeneral = _mapper.Map<AssetMasterGenerals>(request);           
                await _assetMasterGeneralRepository.UpdateAsync(assetMaster.Id, updatedAssetMasterGeneral);
                if (request.AssetMaster.IsActive is 0)
                {
                    return new ApiResponseDTO<AssetMasterUpdateDto>
                    {
                        IsSuccess = false,
                        Message = "Code DeActivated."
                    };
                }
                else{
                    return new ApiResponseDTO<AssetMasterUpdateDto>
                    {
                        IsSuccess = false,
                        Message = "Code Activated."
                    }; 
                }                                     
            }
            var updatedAssetMasterEntity = _mapper.Map<AssetMasterGenerals>(request);                   
            var updateResult = await _assetMasterGeneralRepository.UpdateAsync(request.AssetMaster.Id, updatedAssetMasterEntity);            

            var updatedAssetMaster =  await _assetMasterGeneralQueryRepository.GetByIdAsync(request.AssetMaster.Id);    
            if (updatedAssetMaster != null)
            {
                var assetMasterDto = _mapper.Map<AssetMasterUpdateDto>(updatedAssetMaster);
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
                    return new ApiResponseDTO<AssetMasterUpdateDto>
                    {
                        IsSuccess = true,
                        Message = "AssetMaster updated successfully.",
                        Data = assetMasterDto
                    };
                }
                return new ApiResponseDTO<AssetMasterUpdateDto>
                {
                    IsSuccess = false,
                    Message = "AssetMaster not updated."
                };                
            }
            else
            {
                return new ApiResponseDTO<AssetMasterUpdateDto>{
                    IsSuccess = false,
                    Message = "AssetMaster not found."
                };
            }        
        }
    }
}