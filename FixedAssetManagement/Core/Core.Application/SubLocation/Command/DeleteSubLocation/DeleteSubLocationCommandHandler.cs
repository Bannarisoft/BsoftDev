using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.ISubLocation;
using Core.Application.Location.Command.DeleteAubLocation;
using Core.Application.SubLocation.Queries.GetSubLocations;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.SubLocation.Command.DeleteSubLocation
{
    public class DeleteSubLocationCommandHandler : IRequestHandler<DeleteSubLocationCommand, ApiResponseDTO<SubLocationDto>>
    {
        private readonly ISubLocationCommandRepository _sublocationCommandRepository;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public DeleteSubLocationCommandHandler(ISubLocationCommandRepository sublocationCommandRepository,IMediator mediator,IMapper mapper)
        {
            _sublocationCommandRepository = sublocationCommandRepository;
            _mediator = mediator;
            _mapper = mapper;
        }
        public async Task<ApiResponseDTO<SubLocationDto>> Handle(DeleteSubLocationCommand request, CancellationToken cancellationToken)
        {
            var sublocation  = _mapper.Map<Core.Domain.Entities.SubLocation>(request);
            var sublocationresult = await _sublocationCommandRepository.DeleteAsync(request.Id, sublocation);


                  //Domain Event  
                    var domainEvent = new AuditLogsDomainEvent(
                        actionDetail: "Delete",
                        actionCode: sublocation.Id.ToString(),
                        actionName: sublocation.Id.ToString(),
                        details: $"SubLocation '{sublocation.Id}' was deleted.",
                        module:"SubLocation"
                    );               
                    await _mediator.Publish(domainEvent, cancellationToken);  

                 if(sublocationresult)
                {
                    return new ApiResponseDTO<SubLocationDto>{IsSuccess = true, Message = "SubLocation deleted successfully."};
                }

                return new ApiResponseDTO<SubLocationDto>{IsSuccess = false, Message = "SubLocation not deleted."};
        }
    }
}