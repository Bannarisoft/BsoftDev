using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.ISubLocation;
using Core.Application.SubLocation.Queries.GetSubLocations;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.SubLocation.Queries.GetSubLocationById
{
    public class GetSubLocationByIdQueryHandler : IRequestHandler<GetSubLocationByIdQuery, ApiResponseDTO<SubLocationDto>>
    {
         private readonly ISubLocationQueryRepository _sublocationQueryRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        public GetSubLocationByIdQueryHandler(ISubLocationQueryRepository sublocationQueryRepository,IMapper mapper,IMediator mediator)
        {
            _sublocationQueryRepository = sublocationQueryRepository;
            _mapper = mapper;
            _mediator = mediator;   
        }
        public async Task<ApiResponseDTO<SubLocationDto>> Handle(GetSubLocationByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _sublocationQueryRepository.GetByIdAsync(request.Id);
             if (result is null)
            {
                return new ApiResponseDTO<SubLocationDto>
                {
                    IsSuccess = false,
                    Message = "SubLocationId not found"
                };
            }  
           var sublocation = _mapper.Map<SubLocationDto>(result);

          //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: "",        
                    actionName: "",
                    details: $"SubLocation details {sublocation.Id} was fetched.",
                    module:"SubLocation"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
          return new ApiResponseDTO<SubLocationDto> { IsSuccess = true, Message = "Success", Data = sublocation };
        }
    }
}