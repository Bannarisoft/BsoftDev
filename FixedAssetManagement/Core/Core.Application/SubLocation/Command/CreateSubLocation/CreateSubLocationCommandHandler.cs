using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.ISubLocation;
using Core.Application.SubLocation.Queries.GetSubLocations;
using Core.Domain.Events;
using FluentValidation;
using MediatR;

namespace Core.Application.SubLocation.Command.CreateSubLocation
{
    public class CreateSubLocationCommandHandler : IRequestHandler<CreateSubLocationCommand, SubLocationDto>
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
        public async Task<SubLocationDto> Handle(CreateSubLocationCommand request, CancellationToken cancellationToken)
        {
            var existingsubLocation = await _sublocationQueryRepository.GetBySubLocationNameAsync(request.SubLocationName,request.DepartmentId,request.LocationId,request.UnitId);

               if (existingsubLocation != null)
               {
                throw new ValidationException("SubLocation already exists");
                   
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
                 
                    return sublocationMap;
                }

               throw new Exception("SubLocation not created");
                    
        }
    }
}