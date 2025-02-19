
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IManufacture;
using Core.Application.Manufacture.Queries.GetManufacture;
using Core.Domain.Common;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Manufacture.Commands.UpdateManufacture
{
    public class UpdateManufactureCommandHandler : IRequestHandler<UpdateManufactureCommand, ApiResponseDTO<ManufactureDTO>>
    {
        private readonly IManufactureCommandRepository _manufactureRepository;
        private readonly IManufactureQueryRepository _manufactureQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

        public UpdateManufactureCommandHandler(IManufactureCommandRepository manufactureRepository, IMapper mapper,IManufactureQueryRepository manufactureQueryRepository, IMediator mediator)
        {
            _manufactureRepository = manufactureRepository;
            _mapper = mapper;
            _manufactureQueryRepository = 
            manufactureQueryRepository;
            _mediator = mediator;
        }
        public async Task<ApiResponseDTO<ManufactureDTO>> Handle(UpdateManufactureCommand request, CancellationToken cancellationToken)
        {
            var manufactures = await _manufactureQueryRepository.GetByIdAsync(request.Id);
            if (manufactures is null)
            return new ApiResponseDTO<ManufactureDTO>
            {
                IsSuccess = false,
                Message = "Invalid ManufactureId. The specified Name does not exist or is inactive."
            };
            var oldManufactureName = manufactures.ManufactureName;
            manufactures.ManufactureName = request.ManufactureName;

            if (manufactures is null || manufactures.IsDeleted is BaseEntity.IsDelete.Deleted )
            {
                return new ApiResponseDTO<ManufactureDTO>
                {
                    IsSuccess = false,
                    Message = "Invalid ManufactureID. The specified ManufactureName does not exist or is deleted."
                };
            }
            if (manufactures.IsActive != request.IsActive)
            {    
                 manufactures.IsActive =  (BaseEntity.Status)request.IsActive;             
                await _manufactureRepository.UpdateAsync(manufactures.Id, manufactures);
                if (request.IsActive is 0)
                {
                    return new ApiResponseDTO<ManufactureDTO>
                    {
                        IsSuccess = true,
                        Message = "Code DeActivated."
                    };
                }
                else{
                    return new ApiResponseDTO<ManufactureDTO>
                    {
                        IsSuccess = true,
                        Message = "Code Activated."
                    }; 
                }                                     
            }

            var manufacturesExistsByName = await _manufactureRepository.ExistsByCodeAsync(request.Code??string.Empty);
            if (manufacturesExistsByName)
            {                                   
                return new ApiResponseDTO<ManufactureDTO>
                {
                    IsSuccess = false,
                    Message = $"Code already exists and is {(BaseEntity.Status) request.IsActive}."
                };                     
            }
            var updatedManufactures = _mapper.Map<Manufactures>(request);                   
            var updateResult = await _manufactureRepository.UpdateAsync(request.Id, updatedManufactures);            

            var updatedManufacture =  await _manufactureQueryRepository.GetByIdAsync(request.Id);    
            if (updatedManufacture != null)
            {
                var manufactureDto = _mapper.Map<ManufactureDTO>(updatedManufacture);
                //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Update",
                    actionCode: manufactureDto.Code ?? string.Empty,
                    actionName: manufactureDto.ManufactureName ?? string.Empty,                            
                    details: $"Manufacture '{oldManufactureName}' was updated to '{manufactureDto.ManufactureName}'.  Code: {manufactureDto.Code}",
                    module:"Manufacture"
                );            
                await _mediator.Publish(domainEvent, cancellationToken);
                if(updateResult>0)
                {
                    return new ApiResponseDTO<ManufactureDTO>
                    {
                        IsSuccess = true,
                        Message = "Manufacture updated successfully.",
                        Data = manufactureDto
                    };
                }
                return new ApiResponseDTO<ManufactureDTO>
                {
                    IsSuccess = false,
                    Message = "Manufacture not updated."
                };                
            }
            else
            {
                return new ApiResponseDTO<ManufactureDTO>{
                    IsSuccess = false,
                    Message = "Manufacture not found."
                };
            }
        }
    }
}