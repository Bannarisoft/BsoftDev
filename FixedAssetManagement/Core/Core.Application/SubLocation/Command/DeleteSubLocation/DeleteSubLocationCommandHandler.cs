using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IAssetMaster.IAssetLocation;
using Core.Application.Common.Interfaces.ILocation;
using Core.Application.Common.Interfaces.ISubLocation;
using Core.Application.Location.Command.DeleteAubLocation;
using Core.Application.SubLocation.Queries.GetSubLocations;
using Core.Domain.Common;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.SubLocation.Command.DeleteSubLocation
{
    public class DeleteSubLocationCommandHandler : IRequestHandler<DeleteSubLocationCommand, ApiResponseDTO<SubLocationDto>>
    {
        private readonly ISubLocationCommandRepository _sublocationCommandRepository;
        private readonly ISubLocationQueryRepository _subLocationQueryRepository;
        private readonly ILocationQueryRepository _locationQueryRepository;

        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public DeleteSubLocationCommandHandler(ISubLocationCommandRepository sublocationCommandRepository, IMediator mediator, IMapper mapper, ISubLocationQueryRepository subLocationQueryRepository, ILocationQueryRepository locationQueryRepository)
        {
            _sublocationCommandRepository = sublocationCommandRepository;
            _subLocationQueryRepository = subLocationQueryRepository;
            _mediator = mediator;
            _mapper = mapper;
            _locationQueryRepository = locationQueryRepository;
        }
        public async Task<ApiResponseDTO<SubLocationDto>> Handle(DeleteSubLocationCommand request, CancellationToken cancellationToken)
        {
            var sublocations = await _subLocationQueryRepository.GetByIdAsync(request.Id);
            if (sublocations is null || sublocations.IsDeleted is BaseEntity.IsDelete.Deleted)
            {
                return new ApiResponseDTO<SubLocationDto>
                {
                    IsSuccess = false,
                    Message = "Invalid SubLocationID.The specified SubLocation does not exist or is inactive.  "
                };
            }
            var location = await _locationQueryRepository.GetByIdAsync(request.Id);
            if (location != null)
            {
                return new ApiResponseDTO<SubLocationDto>
                {
                    IsSuccess = false,
                    Message = "SubLocation already exists for this Location.Cannot delete the SubLocation."
                };
            }
            var sublocation = _mapper.Map<Core.Domain.Entities.SubLocation>(request);
            var sublocationresult = await _sublocationCommandRepository.DeleteAsync(request.Id, sublocation);


            //Domain Event  
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Delete",
                actionCode: sublocation.Id.ToString(),
                actionName: sublocation.Id.ToString(),
                details: $"SubLocation '{sublocation.Id}' was deleted.",
                module: "SubLocation"
            );
            await _mediator.Publish(domainEvent, cancellationToken);

            if (sublocationresult)
            {
                return new ApiResponseDTO<SubLocationDto> { IsSuccess = true, Message = "SubLocation deleted successfully." };
            }

            return new ApiResponseDTO<SubLocationDto> { IsSuccess = false, Message = "SubLocation not deleted." };
        }
    }
}