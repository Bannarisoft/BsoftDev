using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Exceptions;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.Power.IFeeder;
using Core.Application.Power.Feeder.Queries.GetFeeder;
using Core.Domain.Events;
using MassTransit.Futures.Contracts;
using MediatR;

namespace Core.Application.Power.Feeder.Command.DeleteFeeder
{
    public class DeleteFeederCommandHandler : IRequestHandler<DeleteFeederCommand, bool>
    {

        private readonly IFeederCommandRepository _feederCommandRepository;
        private readonly IMapper _imapper;
        private readonly IMediator _mediator;


        public DeleteFeederCommandHandler(IFeederCommandRepository feederCommandRepository, IMapper imapper, IMediator imediator)
        {
            _feederCommandRepository = feederCommandRepository;
            _imapper = imapper;
            _mediator = imediator;
        }
         public async Task<bool> Handle(DeleteFeederCommand request, CancellationToken cancellationToken)
        {
            var feeder = _imapper.Map<Core.Domain.Entities.Power.Feeder>(request);

            // Perform the delete operation
            var isDeleted = await _feederCommandRepository.DeleteAsync(request.Id,feeder);

            // Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Delete",
                actionCode: feeder.Id.ToString(),
                actionName: feeder.IsDeleted.ToString(),
                details: $"Feeder with ID {feeder.Id} was deleted.",
                module: "Feeder"
            );
            await _mediator.Publish(domainEvent, cancellationToken);

         
            return isDeleted == true ? isDeleted : throw new ExceptionRules("Feeder deletion failed.");
           
        }
        
    }
}