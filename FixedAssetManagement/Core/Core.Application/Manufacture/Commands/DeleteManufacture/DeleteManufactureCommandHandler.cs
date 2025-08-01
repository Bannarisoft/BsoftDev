

using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IManufacture;
using Core.Application.Manufacture.Queries.GetManufacture;
using Core.Domain.Common;
using Core.Domain.Entities;
using Core.Domain.Events;
using FluentValidation;
using MediatR;

namespace Core.Application.Manufacture.Commands.DeleteManufacture
{
    public class DeleteManufactureCommandHandler   : IRequestHandler<DeleteManufactureCommand, ManufactureDTO>
    {
         private readonly IManufactureCommandRepository _manufactureRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 
        private readonly IManufactureQueryRepository _manufactureQueryRepository;

        public DeleteManufactureCommandHandler(IManufactureCommandRepository manufactureRepository, IMapper mapper,  IMediator mediator,IManufactureQueryRepository manufactureQueryRepository)
        {
            _manufactureRepository = manufactureRepository;
             _mapper = mapper;        
            _mediator = mediator;
            _manufactureQueryRepository=manufactureQueryRepository;
        }

        public async Task<ManufactureDTO> Handle(DeleteManufactureCommand request, CancellationToken cancellationToken)
        {
              var manufactures = await _manufactureQueryRepository.GetByIdAsync(request.Id);
            if (manufactures is null )
            {
                throw new ValidationException("Invalid ManufactureID.");
               
            }
            var manufacturesDelete = _mapper.Map<Manufactures>(request);      
            var updateResult = await _manufactureRepository.DeleteAsync(request.Id, manufacturesDelete);
            if (updateResult > 0)
            {
                var manufactureDto = _mapper.Map<ManufactureDTO>(manufacturesDelete);  
                //Domain Event  
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Delete",
                    actionCode: manufacturesDelete.Code ?? string.Empty,
                    actionName: manufacturesDelete.ManufactureName ?? string.Empty,
                    details: $"Manufacture '{manufactureDto.ManufactureName}' was created. Code: {manufactureDto.Code}",
                    module:"Manufacture"
                );               
                await _mediator.Publish(domainEvent, cancellationToken);                 
                return  manufactureDto;
            }
            throw new Exception("Manufacture deletion failed.");
        
        }
    }
}