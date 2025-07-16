using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Exceptions;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.Power.IFeederGroup;
using Core.Application.Power.FeederGroup.Queries.GetFeederGroup;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Power.FeederGroup.Command.DeleteFeederGroup
{
    public class DeleteFeederGroupCommandHandler : IRequestHandler<DeleteFeederGroupCommand, bool>
    {

        private readonly IFeederGroupCommandRepository _feederGroupCommandRepository;
        private readonly IMapper _imapper;
        private readonly IMediator _mediator;
        
        public DeleteFeederGroupCommandHandler( IFeederGroupCommandRepository feederGroupCommandRepository, IMapper imapper, IMediator imediator)
        {
            _feederGroupCommandRepository = feederGroupCommandRepository;
                _imapper = imapper;
                _mediator = imediator;
        }

        public async Task<bool> Handle(DeleteFeederGroupCommand request, CancellationToken cancellationToken)
        {
            var feederGroup = _imapper.Map<Core.Domain.Entities.Power.FeederGroup>(request);

            // Perform the delete operation
            var isDeleted = await _feederGroupCommandRepository.DeleteAsync(request.Id,feederGroup);

            // Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Delete",
                actionCode: feederGroup.Id.ToString(),
                actionName: feederGroup.IsDeleted.ToString(),
                details: $"FeederGroup with ID {feederGroup.Id} was deleted.",
                module: "FeederGroup"
            );
            await _mediator.Publish(domainEvent, cancellationToken);

          
                return isDeleted == true ? isDeleted : throw new ExceptionRules("FeederGroup deletion failed.");
            
        }
    }
}