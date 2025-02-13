using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IManufacture;
using Core.Application.Manufacture.Queries.GetManufacture;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Manufacture.Queries.GetManufactureAutoComplete
{
    public class GetManufactureAutoCompleteQueryHandler : IRequestHandler<GetManufactureAutoCompleteQuery, ApiResponseDTO<List<ManufactureAutoCompleteDTO>>>
    {
        private readonly IManufactureQueryRepository _manufactureRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

        public GetManufactureAutoCompleteQueryHandler(IManufactureQueryRepository manufactureRepository,  IMapper mapper, IMediator mediator)
        {
            _manufactureRepository =manufactureRepository;
            _mapper =mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<List<ManufactureAutoCompleteDTO>>> Handle(GetManufactureAutoCompleteQuery request, CancellationToken cancellationToken)
        {
            var result = await _manufactureRepository.GetByManufactureNameAsync(request.SearchPattern ?? string.Empty);
            if (result is null || result.Count is 0)
            {
                return new ApiResponseDTO<List<ManufactureAutoCompleteDTO>>
                {
                    IsSuccess = false,
                    Message = "No Manufacture found matching the search pattern."
                };
            }
            var manufacturesDto = _mapper.Map<List<ManufactureAutoCompleteDTO>>(result);
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetAutoComplete",
                actionCode:"",        
                actionName: request.SearchPattern ?? string.Empty,                
                details: $"Manufacture '{request.SearchPattern}' was searched",
                module:"Manufacture"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<ManufactureAutoCompleteDTO>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = manufacturesDto
            };
        }
    }
}