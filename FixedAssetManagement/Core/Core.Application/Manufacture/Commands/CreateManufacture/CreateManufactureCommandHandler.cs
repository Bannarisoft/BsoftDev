using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IManufacture;
using Core.Application.Manufacture.Queries.GetManufacture;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Manufacture.Commands.CreateManufacture
{
    public class CreateManufactureCommandHandler : IRequestHandler<CreateManufactureCommand, ApiResponseDTO<ManufactureDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IManufactureCommandRepository _manufactureRepository;
        private readonly IMediator _mediator;

        public CreateManufactureCommandHandler(IMapper mapper, IManufactureCommandRepository manufactureRepository, IMediator mediator)
        {
            _mapper = mapper;
            _manufactureRepository = manufactureRepository;
            _mediator = mediator;    
        } 

        public async Task<ApiResponseDTO<ManufactureDTO>> Handle(CreateManufactureCommand request, CancellationToken cancellationToken)
        {
            var manufactureExists = await _manufactureRepository.ExistsByCodeAsync(request.Code??string.Empty);
            if (manufactureExists)
            {
                return new ApiResponseDTO<ManufactureDTO> {
                    IsSuccess = false, 
                    Message = "Manufacture Code already exists."
                };                 
            }
            var manufactureEntity = _mapper.Map<Manufactures>(request);            
            var result = await _manufactureRepository.CreateAsync(manufactureEntity);
            
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: manufactureEntity.Code ?? string.Empty,
                actionName: manufactureEntity.ManufactureName ?? string.Empty,
                details: $"Manufacture '{manufactureEntity.ManufactureName}' was created. Code: {manufactureEntity.Code}",
                module:"Manufacture"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            
            var manufactureDto = _mapper.Map<ManufactureDTO>(result);
            if (manufactureDto.Id > 0)
            {
                return new ApiResponseDTO<ManufactureDTO>{
                    IsSuccess = true, 
                    Message = "Manufacture created successfully.",
                    Data = manufactureDto
                };
            }
            return  new ApiResponseDTO<ManufactureDTO>{
                IsSuccess = false, 
                Message = "Manufacture not created."
            };      
        }
    }
}