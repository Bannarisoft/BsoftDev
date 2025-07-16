using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Exceptions;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.ICostCenter;
using Core.Application.CostCenter.Queries.GetCostCenter;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.CostCenter.Command.CreateCostCenter
{
    public class CreateCostCenterCommandHandler : IRequestHandler<CreateCostCenterCommand, int>
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

        public async Task<int> Handle(CreateCostCenterCommand request, CancellationToken cancellationToken)
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
            
            return result > 0 ? result : throw new ExceptionRules("CostCenter Creation Failed.");
        }
    }
}