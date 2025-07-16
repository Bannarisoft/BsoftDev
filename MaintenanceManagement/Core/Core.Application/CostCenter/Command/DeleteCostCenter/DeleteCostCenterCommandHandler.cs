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

namespace Core.Application.CostCenter.Command.DeleteCostCenter
{
    public class DeleteCostCenterCommandHandler : IRequestHandler<DeleteCostCenterCommand, int>
    {
        private readonly ICostCenterCommandRepository _iCostCenterCommandRepository;
        private readonly ICostCenterQueryRepository _iCostCenterQueryRepository;
        private readonly IMapper _Imapper;
        private readonly IMediator _mediator; 

        public DeleteCostCenterCommandHandler(ICostCenterCommandRepository iCostCenterCommandRepository, ICostCenterQueryRepository iCostCenterQueryRepository, IMapper imapper, IMediator mediator)
        {
            _iCostCenterCommandRepository = iCostCenterCommandRepository;
            _iCostCenterQueryRepository = iCostCenterQueryRepository;
            _Imapper = imapper;
            _mediator = mediator;
        }

        public async Task<int> Handle(DeleteCostCenterCommand request, CancellationToken cancellationToken)
        {
            
            var costCenterGroup = _Imapper.Map<Core.Domain.Entities.CostCenter>(request);
            var result = await _iCostCenterCommandRepository.DeleteAsync(request.Id,costCenterGroup);
          

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Delete",
                actionCode: costCenterGroup.CostCenterCode,
                actionName: costCenterGroup.CostCenterName,
                details: $"CostCenter details was deleted",
                module: "CostCenter");
            await _mediator.Publish(domainEvent);
          

            return result > 0 ? result : throw new ExceptionRules("CostCenter not found.");
        }


    }
}