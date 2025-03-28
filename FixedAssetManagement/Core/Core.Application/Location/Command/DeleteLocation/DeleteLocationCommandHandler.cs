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
        private readonly ILocationQueryRepository _locationQueryRepository;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public DeleteLocationCommandHandler(ILocationCommandRepository locationCommandRepository, IMediator mediator, IMapper mapper, ILocationQueryRepository locationQueryRepository)
        {
            _locationCommandRepository = locationCommandRepository;
            _locationQueryRepository = locationQueryRepository;
            _mediator = mediator;
            _mapper = mapper;
        }
        public async Task<ApiResponseDTO<LocationDto>> Handle(DeleteLocationCommand request, CancellationToken cancellationToken)
        {
            var locations = await _locationQueryRepository.GetByIdAsync(request.Id);
            if (locations is null)
            {
                return new ApiResponseDTO<LocationDto>
                {
                    IsSuccess = false,
                    Message = "Invalid LocationID. "
                };
            }
            var locationdelete = _mapper.Map<Core.Domain.Entities.Location>(request);
            var locationresult = await _locationCommandRepository.DeleteAsync(request.Id, locationdelete);
            if (locationresult > 0)
            {
                var locationDto = _mapper.Map<LocationDto>(locationdelete);
                //Domain Event  
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Delete",
                    actionCode: locationdelete.Id.ToString(),
                    actionName: locationdelete.Id.ToString(),
                    details: $"Location '{locationdelete.Id}' was deleted.",
                    module: "Location"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
                return new ApiResponseDTO<LocationDto>
                {
                    IsSuccess = true,
                    Message = "Location deleted successfully.",
                    Data = locationDto
                };
            }

            return new ApiResponseDTO<LocationDto>
            {
                IsSuccess = false,
                Message = "Location deletion failed."
            };
        }
    }
}