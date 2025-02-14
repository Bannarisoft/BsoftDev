using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IManufacture;
using Core.Application.Manufacture.Queries.GetManufacture;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Manufacture.Queries.GetManufactureById
{
    public class GetManufactureByIdQueryHandler : IRequestHandler<GetManufactureByIdQuery, ApiResponseDTO<ManufactureDTO>>
    {
        private readonly IManufactureQueryRepository _manufactureRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

        public GetManufactureByIdQueryHandler(IManufactureQueryRepository manufactureRepository,  IMapper mapper, IMediator mediator)
        {
            _manufactureRepository =manufactureRepository;
            _mapper =mapper;
            _mediator = mediator;
        }
        public async Task<ApiResponseDTO<ManufactureDTO>> Handle(GetManufactureByIdQuery request, CancellationToken cancellationToken)
        {
            var manufacture = await _manufactureRepository.GetByIdAsync(request.Id);                
            var manufactureDto = _mapper.Map<ManufactureDTO>(manufacture);
            if (manufacture is null)
            {                
                return new ApiResponseDTO<ManufactureDTO>
                {
                    IsSuccess = false,
                    Message = "Manufacture with ID {request.Id} not found."
                };   
            }       
                //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetById",
                actionCode: manufactureDto.Code ?? string.Empty,        
                actionName: manufactureDto.ManufactureName ?? string.Empty,                
                details: $"Manufacture '{manufactureDto.ManufactureName}' was created. Code: {manufactureDto.Code}",
                module:"Manufacture"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<ManufactureDTO>
            {
                IsSuccess = true,
                Message = "Success",
                Data = manufactureDto
            };       
        }
    }
}