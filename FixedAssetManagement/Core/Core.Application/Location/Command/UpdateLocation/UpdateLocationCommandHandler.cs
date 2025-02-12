using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.ILocation;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Location.Command.UpdateLocation
{
    public class UpdateLocationCommandHandler : IRequestHandler<UpdateLocationCommand, ApiResponseDTO<bool>>
    {
        private readonly ILocationCommandRepository _locationCommandRepository;
        private readonly ILocationQueryRepository _locationQueryRepository;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public UpdateLocationCommandHandler(ILocationCommandRepository locationCommandRepository,ILocationQueryRepository locationQueryRepository,IMediator mediator,IMapper mapper)
        {
            _locationCommandRepository = locationCommandRepository;
            _locationQueryRepository =  locationQueryRepository;
            _mediator = mediator;
            _mapper = mapper;    
        }
        public async Task<ApiResponseDTO<bool>> Handle(UpdateLocationCommand request, CancellationToken cancellationToken)
        {
            var existingLocation = await _locationQueryRepository.GetByLocationNameAsync(request.LocationName, request.Id);

                if (existingLocation != null)
                {
                    return new ApiResponseDTO<bool>{IsSuccess = false, Message = "Location already exists"};
                }
                 var location  = _mapper.Map<Core.Domain.Entities.Location>(request);
         
                var locationresult = await _locationCommandRepository.UpdateAsync(location);

                
                    var domainEvent = new AuditLogsDomainEvent(
                        actionDetail: "Update",
                        actionCode: location.Code,
                        actionName: location.LocationName,
                        details: $"Location '{location.Id}' was updated.",
                        module:"Location"
                    );               
                    await _mediator.Publish(domainEvent, cancellationToken); 
              
                if(locationresult)
                {
                    return new ApiResponseDTO<bool>{IsSuccess = true, Message = "Location updated successfully."};
                }

                return new ApiResponseDTO<bool>{IsSuccess = false, Message = "Location not updated."};
        }
    }
}