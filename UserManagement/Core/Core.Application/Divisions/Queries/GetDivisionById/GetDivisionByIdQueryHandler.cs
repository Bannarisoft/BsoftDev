using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.Interfaces;
using MediatR;
using System.Text;
using Core.Application.Divisions.Queries.GetDivisions;
using Core.Application.Common.Interfaces.IDivision;
using Core.Application.Common.HttpResponse;
using Core.Domain.Events;

namespace Core.Application.Divisions.Queries.GetDivisionById
{
    public class GetDivisionByIdQueryHandler : IRequestHandler<GetDivisionByIdQuery,ApiResponseDTO<DivisionDTO>>
    {
         private readonly IDivisionQueryRepository _divisionRepository;        
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

         public GetDivisionByIdQueryHandler(IDivisionQueryRepository divisionRepository, IMapper mapper, IMediator mediator)
        {
            _divisionRepository = divisionRepository;
            _mapper =mapper;
            _mediator = mediator;
        } 
        public async Task<ApiResponseDTO<DivisionDTO>> Handle(GetDivisionByIdQuery request, CancellationToken cancellationToken)
        {
            
        var result = await _divisionRepository.GetByIdAsync(request.Id);
        var division = _mapper.Map<DivisionDTO>(result);

          //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: "",        
                    actionName: "",
                    details: $"Division details {division.Id} was fetched.",
                    module:"Division"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
          return new ApiResponseDTO<DivisionDTO> { IsSuccess = true, Message = "Success", Data = division };

        }
    }
}