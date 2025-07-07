using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IMachineSpecification;
using Core.Application.MachineSpecification.Command;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MachineSpecification.Queries.GetMachineSpecificationById
{
    public class GetMachineSpecificationByIdQueryHandler : IRequestHandler<GetMachineSpecificationByIdQuery, ApiResponseDTO<MachineSpecificationDto>>
    {
        private readonly IMachineSpecificationQueryRepository _imachineSpecificationQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;     

        public GetMachineSpecificationByIdQueryHandler(IMachineSpecificationQueryRepository imachineSpecificationQueryRepository, IMapper mapper, IMediator mediator)
        {
            _imachineSpecificationQueryRepository = imachineSpecificationQueryRepository;            
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<MachineSpecificationDto>> Handle(GetMachineSpecificationByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _imachineSpecificationQueryRepository.GetByIdAsync(request.Id);
            // Check if the entity exists
            if (result is null)
            {
                return new ApiResponseDTO<MachineSpecificationDto> { IsSuccess = false, Message = $"Machine ID {request.Id} not found." };
            }
            // Map a single entity
            var machineMaster = _mapper.Map<MachineSpecificationDto>(result);
             var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: "GetMachineSpecificationByIdQuery",        
                    actionName: machineMaster.Id.ToString(),
                    details: $"MachineSpecification details {machineMaster.Id} was fetched.",
                    module:"MachineSpecification"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
          return new ApiResponseDTO<MachineSpecificationDto> { IsSuccess = true, Message = "Success", Data = machineMaster };
            
        }
    }
}