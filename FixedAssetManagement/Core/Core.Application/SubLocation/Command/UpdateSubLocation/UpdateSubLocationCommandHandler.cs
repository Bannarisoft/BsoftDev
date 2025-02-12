using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.ISubLocation;
using Core.Application.Location.Command.UpdateSubLocation;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.SubLocation.Command.UpdateSubLocation
{
    public class UpdateSubLocationCommandHandler : IRequestHandler<UpdateSubLocationCommand, ApiResponseDTO<bool>>
    {
        private readonly ISubLocationCommandRepository _sublocationCommandRepository;
        private readonly ISubLocationQueryRepository _sublocationQueryRepository;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public UpdateSubLocationCommandHandler(ISubLocationCommandRepository sublocationCommandRepository,ISubLocationQueryRepository sublocationQueryRepository,IMapper mapper,IMediator mediator)
        {
            _sublocationCommandRepository = sublocationCommandRepository;
            _sublocationQueryRepository = sublocationQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
        }
        public async Task<ApiResponseDTO<bool>> Handle(UpdateSubLocationCommand request, CancellationToken cancellationToken)
        {
            var existingSubLocation = await _sublocationQueryRepository.GetBySubLocationNameAsync(request.SubLocationName, request.Id);

                if (existingSubLocation != null)
                {
                    return new ApiResponseDTO<bool>{IsSuccess = false, Message = "SubLocation already exists"};
                }
                 var sublocation  = _mapper.Map<Core.Domain.Entities.SubLocation>(request);
         
                var sublocationresult = await _sublocationCommandRepository.UpdateAsync(sublocation);

                
                    var domainEvent = new AuditLogsDomainEvent(
                        actionDetail: "Update",
                        actionCode: sublocation.Code,
                        actionName: sublocation.SubLocationName,
                        details: $"SubLocation '{sublocation.Id}' was updated.",
                        module:"SubLocation"
                    );               
                    await _mediator.Publish(domainEvent, cancellationToken); 
              
                if(sublocationresult)
                {
                    return new ApiResponseDTO<bool>{IsSuccess = true, Message = "SubLocation updated successfully."};
                }

                return new ApiResponseDTO<bool>{IsSuccess = false, Message = "SubLocation not updated."};
        }
    }
}