
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
    public class UpdateManufactureCommandHandler : IRequestHandler<UpdateManufactureCommand, ApiResponseDTO<bool>>
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
        public async Task<ApiResponseDTO<bool>> Handle(UpdateManufactureCommand request, CancellationToken cancellationToken)
        {
            var manufactures = await _manufactureQueryRepository.GetByIdAsync(request.Id);
            if (manufactures is null)
            return new ApiResponseDTO<bool>
            {
                IsSuccess = false,
                Message = "Invalid ManufactureId. The specified Name does not exist."
            };
            var oldManufactureName = manufactures.ManufactureName;
            manufactures.ManufactureName = request.ManufactureName;

            if (manufactures is null || manufactures.IsDeleted is BaseEntity.IsDelete.Deleted )
            {
                return new ApiResponseDTO<bool>
                {
                    IsSuccess = false,
                    Message = "Invalid ManufactureID. The specified ManufactureName does not exist or is deleted."
                };
            }        
            var manufactureExists = await _manufactureRepository.ExistsByCodeAsync(request.Code??string.Empty,request.Id);

            if (manufactureExists)
            {
                return new ApiResponseDTO<bool> {
                    IsSuccess = false, 
                    Message = "Manufacture Code already exists."
                };                             
            }

            var updatedManufactures = _mapper.Map<Manufactures>(request);                   
            var updateResult = await _manufactureRepository.UpdateAsync(updatedManufactures);            
            
                         
                //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Update",
                    actionCode: request.Code ?? string.Empty,
                    actionName: request.ManufactureName ?? string.Empty,                            
                    details: $"Manufacture '{oldManufactureName}' was updated to '{request.ManufactureName}'.  Code: {request.Code}",
                    module:"Manufacture"
                );            
                await _mediator.Publish(domainEvent, cancellationToken);
                if (updateResult)
                { 
                    return new ApiResponseDTO<bool>{IsSuccess = true, Message = "Manufacture updated successfully."};
                }
            
                return new ApiResponseDTO<bool>
                {
                    IsSuccess = false,
                    Message = "Manufacture not updated."
                };                
            }
    }
}