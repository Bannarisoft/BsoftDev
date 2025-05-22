using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.ISpecificationMaster;
using Core.Application.SpecificationMaster.Queries.GetSpecificationMaster;
using Core.Domain.Common;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.SpecificationMaster.Commands.UpdateSpecificationMaster
{
    public class UpdateSpecificationMasterCommandHandler : IRequestHandler<UpdateSpecificationMasterCommand, ApiResponseDTO<SpecificationMasterDTO>>
    {
        private readonly ISpecificationMasterCommandRepository _specificationMasterRepository;
        private readonly ISpecificationMasterQueryRepository _specificationMasterQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

        public UpdateSpecificationMasterCommandHandler(ISpecificationMasterCommandRepository specificationMasterRepository, IMapper mapper,ISpecificationMasterQueryRepository specificationMasterQueryRepository, IMediator mediator)
        {
            _specificationMasterRepository = specificationMasterRepository;
            _mapper = mapper;
            _specificationMasterQueryRepository = specificationMasterQueryRepository;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<SpecificationMasterDTO>> Handle(UpdateSpecificationMasterCommand request, CancellationToken cancellationToken)
        {
            var specificationMaster = await _specificationMasterQueryRepository.GetByIdAsync(request.Id);
            if (specificationMaster is null)
            return new ApiResponseDTO<SpecificationMasterDTO>
            {
                IsSuccess = false,
                Message = "Invalid SpecificationMaster. The specified Name does not exist or is inactive."
            };
            
            var oldSpecificationMaster = specificationMaster.SpecificationName;
            specificationMaster.SpecificationName = request.SpecificationName;

            /*if (specificationMaster is null || specificationMaster.IsDeleted is BaseEntity.IsDelete.Deleted )
            {
                return new ApiResponseDTO<SpecificationMasterDTO>
                {
                    IsSuccess = false,
                    Message = "Invalid Specification Id. The specified Name does not exist or is deleted."
                };
            }
            if (specificationMaster.IsActive != request.IsActive)
            {    
                 specificationMaster.IsActive =  (BaseEntity.Status)request.IsActive;             
                await _specificationMasterRepository.UpdateAsync(specificationMaster.Id, specificationMaster);
                if (request.IsActive is 0)
                {
                    return new ApiResponseDTO<SpecificationMasterDTO>
                    {
                        IsSuccess = true,
                        Message = "Code DeActivated."
                    };
                }
                else{
                    return new ApiResponseDTO<SpecificationMasterDTO>
                    {
                        IsSuccess = true,
                        Message = "Code Activated."
                    }; 
                }                                     
            } */

         /*    var specificationMasterExistsByName = await _specificationMasterRepository.ExistsByAssetGroupIdAsync(request.AssetGroupId, request.SpecificationName);
            if (specificationMasterExistsByName)
            {                                   
                return new ApiResponseDTO<SpecificationMasterDTO>
                {
                    IsSuccess = false,
                    Message = $"SpecificationName already exists and is {(BaseEntity.Status) request.IsActive}."
                };                     
            } */ 
            var updatedSpecificationEntity= _mapper.Map<SpecificationMasters>(request);                   
            var updateResult = await _specificationMasterRepository.UpdateAsync(updatedSpecificationEntity);            

            var updatedSpecificationMaster =  await _specificationMasterQueryRepository.GetByIdAsync(request.Id);    
            if (updatedSpecificationMaster != null)
            {
                var specificationMasterDto = _mapper.Map<SpecificationMasterDTO>(updatedSpecificationMaster);
                //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Update",
                    actionCode: specificationMasterDto.AssetGroupId.ToString()??string.Empty,
                    actionName: specificationMasterDto.SpecificationName ?? string.Empty,                            
                    details: $"SpecificationMaster '{oldSpecificationMaster}' was updated to '{specificationMasterDto.SpecificationName}'",
                    module:"SpecificationMaster"
                );            
                await _mediator.Publish(domainEvent, cancellationToken);
                if(updateResult>0)
                {
                    return new ApiResponseDTO<SpecificationMasterDTO>
                    {
                        IsSuccess = true,
                        Message = "SpecificationMaster updated successfully.",
                        Data = specificationMasterDto
                    };
                }
                return new ApiResponseDTO<SpecificationMasterDTO>
                {
                    IsSuccess = false,
                    Message = "SpecificationMaster not updated."
                };                
            }
            else
            {
                return new ApiResponseDTO<SpecificationMasterDTO>{
                    IsSuccess = false,
                    Message = "SpecificationMaster not found."
                };
            }
        }
    }
}