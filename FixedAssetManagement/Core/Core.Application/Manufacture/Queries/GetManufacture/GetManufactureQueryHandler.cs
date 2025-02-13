using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.IManufacture;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Manufacture.Queries.GetManufacture
{
    public class GetManufactureQueryHandler : IRequestHandler<GetManufactureQuery, ApiResponseDTO<List<ManufactureDTO>>>    
    {
        private readonly IManufactureQueryRepository _manufactureRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

        public GetManufactureQueryHandler(IManufactureQueryRepository manufactureRepository , IMapper mapper, IMediator mediator)
        {
            _manufactureRepository = manufactureRepository;
            _mapper = mapper;
            _mediator = mediator;
        }
        public async Task<ApiResponseDTO<List<ManufactureDTO>>> Handle(GetManufactureQuery request, CancellationToken cancellationToken)
        {
            var (manufacture, totalCount) = await _manufactureRepository.GetAllManufactureAsync(request.PageNumber, request.PageSize, request.SearchTerm);
            var manufactureList = _mapper.Map<List<ManufactureDTO>>(manufacture);

            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetAll",
                actionCode: "",        
                actionName: "",
                details: $"Manufacture details was fetched.",
                module:"Manufacture"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<ManufactureDTO>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = manufactureList,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };            
        }
    }
}