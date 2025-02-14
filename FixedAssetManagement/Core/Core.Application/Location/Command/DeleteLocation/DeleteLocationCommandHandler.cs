using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.ILocation;
using Core.Application.Location.Queries.GetLocations;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Location.Command.DeleteLocation
{
    public class DeleteLocationCommandHandler : IRequestHandler<DeleteLocationCommand, ApiResponseDTO<LocationDto>>
    {
        private readonly ILocationCommandRepository _locationCommandRepository;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public DeleteLocationCommandHandler(ILocationCommandRepository locationCommandRepository,IMediator mediator,IMapper mapper)
        {
            _locationCommandRepository = locationCommandRepository;
            _mediator = mediator;
            _mapper = mapper;
        }
        public async Task<ApiResponseDTO<LocationDto>> Handle(DeleteLocationCommand request, CancellationToken cancellationToken)
        {
            var location  = _mapper.Map<Core.Domain.Entities.Location>(request);
            var locationresult = await _locationCommandRepository.DeleteAsync(request.Id, location);


                  //Domain Event  
                    var domainEvent = new AuditLogsDomainEvent(
                        actionDetail: "Delete",
                        actionCode: location.Id.ToString(),
                        actionName: location.Id.ToString(),
                        details: $"Location '{location.Id}' was deleted.",
                        module:"Location"
                    );               
                    await _mediator.Publish(domainEvent, cancellationToken);  

                 if(locationresult)
                {
                    return new ApiResponseDTO<LocationDto>{IsSuccess = true, Message = "Location deleted successfully."};
                }

                return new ApiResponseDTO<LocationDto>{IsSuccess = false, Message = "Location not deleted."};
        }
    }
}