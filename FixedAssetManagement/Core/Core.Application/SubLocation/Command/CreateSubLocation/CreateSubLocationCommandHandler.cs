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

namespace Core.Application.SubLocation.Command.CreateSubLocation
{
    public class CreateSubLocationCommandHandler : IRequestHandler<CreateSubLocationCommand, ApiResponseDTO<SubLocationDto>>
    {
         private readonly ISubLocationCommandRepository _sublocationCommandRepository;
        private readonly ISubLocationQueryRepository _sublocationQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        public CreateSubLocationCommandHandler(ISubLocationCommandRepository sublocationCommandRepository,ISubLocationQueryRepository sublocationQueryRepository,IMapper mapper,IMediator mediator)
        {
            _sublocationCommandRepository = sublocationCommandRepository;
            _sublocationQueryRepository = sublocationQueryRepository;
            _mapper = mapper;
            _mediator = mediator;   
        }
        public async Task<ApiResponseDTO<SubLocationDto>> Handle(CreateSubLocationCommand request, CancellationToken cancellationToken)
        {
            var existingsubLocation = await _sublocationQueryRepository.GetBySubLocationNameAsync(request.SubLocationName);

               if (existingsubLocation != null)
               {
                   return new ApiResponseDTO<SubLocationDto>{IsSuccess = false, Message = "SubLocation already exists"};
               }
           
                 var sublocation  = _mapper.Map<Core.Domain.Entities.SubLocation>(request);

                var sublocationresult = await _sublocationCommandRepository.CreateAsync(sublocation);
                
                var sublocationMap = _mapper.Map<SubLocationDto>(sublocationresult);
                if (sublocationresult.Id > 0)
                {
                    var domainEvent = new AuditLogsDomainEvent(
                     actionDetail: "Create",
                     actionCode: sublocationresult.Code,
                     actionName: sublocationresult.SubLocationName,
                     details: $"SubLocation '{sublocationresult.Code}' was created. SubLocationName: {sublocationresult.SubLocationName}",
                     module:"SubLocation"
                 );
                 await _mediator.Publish(domainEvent, cancellationToken);
                 
                    return new ApiResponseDTO<SubLocationDto>{IsSuccess = true, Message = "SubLocation created successfully", Data = sublocationMap};
                }
               
                    return new ApiResponseDTO<SubLocationDto>{IsSuccess = false, Message = "SubLocation not created"};
        }
    }
}