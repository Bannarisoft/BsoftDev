using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.ICostCenter;
using Core.Application.CostCenter.Queries.GetCostCenter;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.CostCenter.Command.CreateCostCenter
{
    public class CreateCostCenterCommandHandler : IRequestHandler<CreateCostCenterCommand, ApiResponseDTO<int>>
    {
        private readonly ICostCenterCommandRepository _iCostCenterCommandRepository;
        private readonly IMediator _imediator;
        private readonly IMapper _imapper;

        public CreateCostCenterCommandHandler(ICostCenterCommandRepository iCostCenterCommandRepository, IMediator imediator, IMapper imapper)
        {
            _iCostCenterCommandRepository = iCostCenterCommandRepository;
            _imediator = imediator;
            _imapper = imapper;
        }

        public async Task<ApiResponseDTO<int>> Handle(CreateCostCenterCommand request, CancellationToken cancellationToken)
        {
            var costCenter = _imapper.Map<Core.Domain.Entities.CostCenter>(request);
            
            var result = await _iCostCenterCommandRepository.CreateAsync(costCenter);

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: costCenter.CostCenterCode,
                actionName: costCenter.CostCenterName,
                details: $"CostCenter details was created",
                module: "CostCenter");
            await _imediator.Publish(domainEvent, cancellationToken);
          
            var costcenterGroupDtoDto = _imapper.Map<CostCenterDto>(costCenter);
            if (result > 0)
                  {
                    
                        return new ApiResponseDTO<int>
                       {
                           IsSuccess = true,
                           Message = "CostCenter created successfully",
                           Data = result
                      };
                 }
            return new ApiResponseDTO<int>
            {
                IsSuccess = true,
                Message = "CostCenter Creation Failed",
                Data = result
            };
        }
    }
}