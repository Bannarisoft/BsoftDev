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

namespace Core.Application.Location.Command.CreateLocation
{
    public class CreateLocationCommandHandler : IRequestHandler<CreateLocationCommand, ApiResponseDTO<LocationDto>>
    {
        private readonly ILocationCommandRepository _locationCommandRepository;
        private readonly ILocationQueryRepository _locationQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        public CreateLocationCommandHandler(ILocationCommandRepository locationCommandRepository,ILocationQueryRepository locationQueryRepository,IMapper mapper,IMediator mediator)
        {
            _locationCommandRepository = locationCommandRepository;
            _locationQueryRepository = locationQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
        }
        public async Task<ApiResponseDTO<LocationDto>> Handle(CreateLocationCommand request, CancellationToken cancellationToken)
        {
            var existingLocation = await _locationQueryRepository.GetByLocationNameAsync(request.LocationName);

               if (existingLocation != null)
               {
                   return new ApiResponseDTO<LocationDto>{IsSuccess = false, Message = "Location already exists"};
               }
           
                 var location  = _mapper.Map<Core.Domain.Entities.Location>(request);

                var locationresult = await _locationCommandRepository.CreateAsync(location);
                
                var locationMap = _mapper.Map<LocationDto>(locationresult);
                if (locationresult.Id > 0)
                {
                    var domainEvent = new AuditLogsDomainEvent(
                     actionDetail: "Create",
                     actionCode: locationresult.Code,
                     actionName: locationresult.LocationName,
                     details: $"Location '{locationresult.Code}' was created. LocationName: {locationresult.LocationName}",
                     module:"Location"
                 );
                 await _mediator.Publish(domainEvent, cancellationToken);
                 
                    return new ApiResponseDTO<LocationDto>{IsSuccess = true, Message = "Location created successfully", Data = locationMap};
                }
               
                    return new ApiResponseDTO<LocationDto>{IsSuccess = false, Message = "Location not created"};
        }
    }
}