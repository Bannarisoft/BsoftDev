using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IManufacture;
using Core.Application.Manufacture.Queries.GetManufacture;
using Core.Domain.Entities;
using Core.Domain.Events;
using FluentValidation;
using MediatR;

namespace Core.Application.Manufacture.Commands.CreateManufacture
{
    public class CreateManufactureCommandHandler : IRequestHandler<CreateManufactureCommand, ManufactureDTO>
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

        public async Task<ManufactureDTO> Handle(CreateManufactureCommand request, CancellationToken cancellationToken)
        {
            var manufactureExists = await _manufactureRepository.ExistsByCodeAsync(request.Code??string.Empty);
            if (manufactureExists)
            {
                throw new ValidationException("Manufacture Code already exists.");
                              
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
                throw new Exception("Manufacture created successfully.");
             
            }
            throw new Exception("Manufacture not created.");
                
        }
    }
}