using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Exceptions;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.ICostCenter;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.CostCenter.Command.UpdateCostCenter
{
    public class UpdateCostCenterCommandHandler  : IRequestHandler<UpdateCostCenterCommand, int>
    {
        private readonly ICostCenterCommandRepository _iCostCenterCommandRepository;
        private readonly ICostCenterQueryRepository _iCostCenterQueryRepository;
        private readonly IMapper _Imapper;
        private readonly IMediator _mediator; 

        public UpdateCostCenterCommandHandler(ICostCenterCommandRepository iCostCenterCommandRepository, ICostCenterQueryRepository iCostCenterQueryRepository, IMapper imapper, IMediator mediator)
        {
            _iCostCenterCommandRepository = iCostCenterCommandRepository;
            _iCostCenterQueryRepository = iCostCenterQueryRepository;
            _Imapper = imapper;
            _mediator = mediator;
        }

        public async Task<int> Handle(UpdateCostCenterCommand request, CancellationToken cancellationToken)
        {
           
       
            var costCenter = _Imapper.Map<Core.Domain.Entities.CostCenter>(request);
            var result = await _iCostCenterCommandRepository.UpdateAsync(request.Id, costCenter);
            
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Update",
                actionCode: costCenter.CostCenterCode,
                actionName: costCenter.CostCenterName,
                details: $"CostCenter details was updated",
                module: "CostCenter");
            await _mediator.Publish(domainEvent, cancellationToken);
           
            return result > 0 ? result : throw new ExceptionRules("CostCenter update failed.");   
        }
    }
}