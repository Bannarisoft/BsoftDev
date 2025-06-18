using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.Power.IFeeder;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Power.Feeder.Command.UpdateFeeder
{
    public class UpdateFeederCommandHandler : IRequestHandler<UpdateFeederCommand, ApiResponseDTO<bool>>
    {
        private readonly IFeederCommandRepository _feederCommandRepository;
        private readonly IMediator _imediator;
        private readonly IMapper _imapper;

        public UpdateFeederCommandHandler(IFeederCommandRepository feederCommandRepository, IMediator imediator, IMapper imapper)
        {
            _feederCommandRepository = feederCommandRepository;
            _imediator = imediator;
            _imapper = imapper;
        }

        public async Task<ApiResponseDTO<bool>> Handle(UpdateFeederCommand request, CancellationToken cancellationToken)
        {
              var feeder = _imapper.Map<Core.Domain.Entities.Power.Feeder>(request);

            var result = await _feederCommandRepository.UpdateAsync(request.Id, feeder);

            if (!result)
            {
                return new ApiResponseDTO<bool>
                {
                    IsSuccess = false,
                    Message = "FeederGroup not found."
                };
            }

            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Update",
                actionCode: feeder.Id.ToString(),
                actionName: "Update Feeder",
                details: "Feeder details were updated",
                module: "Feeder");

            await _imediator.Publish(domainEvent, cancellationToken);

            return new ApiResponseDTO<bool>
            {
                IsSuccess = true,
                Message = "Feeder updated successfully.",
                Data = true
            };
        }
    }
}