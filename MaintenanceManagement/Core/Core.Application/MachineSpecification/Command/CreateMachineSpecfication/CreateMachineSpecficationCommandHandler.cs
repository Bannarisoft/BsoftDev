using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.IMachineSpecification;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.MachineSpecification.Command.CreateMachineSpecfication
{
    public class CreateMachineSpecficationCommandHandler : IRequestHandler<CreateMachineSpecficationCommand, ApiResponseDTO<int>>
    {
        private readonly IMachineSpecificationCommandRepository _imachineSpecificationCommandRepository;
        private readonly IMediator _imediator;
        private readonly IMapper _imapper;

        public CreateMachineSpecficationCommandHandler(IMachineSpecificationCommandRepository imachineSpecificationCommandRepository, IMediator imediator, IMapper imapper)
        {
            _imachineSpecificationCommandRepository = imachineSpecificationCommandRepository;
            _imediator = imediator;
            _imapper = imapper;
        }

        public async Task<ApiResponseDTO<int>> Handle(CreateMachineSpecficationCommand request, CancellationToken cancellationToken)
        {
            var machineMaster = _imapper.Map<Core.Domain.Entities.MachineSpecification>(request);
            
            var result = await _imachineSpecificationCommandRepository.CreateAsync(machineMaster);

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: machineMaster.MachineId.ToString(),
                actionName: machineMaster.SpecificationId.ToString(),
                details: $"MachineSpecification details was created",
                module: "MachineSpecification");
            await _imediator.Publish(domainEvent, cancellationToken);
          
            var costcenterGroupDtoDto = _imapper.Map<MachineSpecificationDto>(machineMaster);
            if (result > 0)
                  {
                    
                        return new ApiResponseDTO<int>
                       {
                           IsSuccess = true,
                           Message = "MachineSpecification created successfully",
                           Data = result
                      };
                 }
            return new ApiResponseDTO<int>
            {
                IsSuccess = true,
                Message = "MachineSpecification Creation Failed",
                Data = result
            };
        }
    }
}