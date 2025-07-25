using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Exceptions;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.Power.IFeederGroup;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Power.FeederGroup.Command.UpdateFeederGroup
{
    public class UpdateFeederGroupCommandHandler : IRequestHandler<UpdateFeederGroupCommand, bool>
    {

        private readonly IFeederGroupCommandRepository _feederGroupCommandRepository;
        private readonly IMediator _imediator;
        private readonly IMapper _imapper;
        
        public UpdateFeederGroupCommandHandler( IFeederGroupCommandRepository feederGroupCommandRepository, IMediator imediator, IMapper imapper)
        {
            _feederGroupCommandRepository = feederGroupCommandRepository;
             _imediator = imediator;
            _imapper = imapper;
        }

      public async Task<bool> Handle(UpdateFeederGroupCommand request, CancellationToken cancellationToken)
        {
            var feederGroup = _imapper.Map<Core.Domain.Entities.Power.FeederGroup>(request);

            var result = await _feederGroupCommandRepository.UpdateAsync(request.Id, feederGroup);

            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Update",
                actionCode: feederGroup.Id.ToString(),
                actionName: feederGroup.FeederGroupName ?? "NULL",
                details: "FeederGroup details were updated",
                module: "FeederGroup");

            await _imediator.Publish(domainEvent, cancellationToken);

            return result == true ? result : throw new ExceptionRules("FeederGroup Updation Failed.");
        }

    }
}