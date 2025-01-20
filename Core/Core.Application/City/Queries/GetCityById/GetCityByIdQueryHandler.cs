using AutoMapper;
using Core.Application.City.Queries.GetCities;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.ICity;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.City.Queries.GetCityById
{
    public class GetCityByIdQueryHandler : IRequestHandler<GetCityByIdQuery,ApiResponseDTO<CityDto>>
    {
    private readonly ICityQueryRepository _cityRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetCityByIdQueryHandler(ICityQueryRepository cityRepository, IMapper mapper, IMediator mediator)
        {
           _cityRepository = cityRepository;
            _mapper = mapper;
            _mediator = mediator;
        }
        public async Task<ApiResponseDTO<CityDto>> Handle(GetCityByIdQuery request, CancellationToken cancellationToken)
        {                    
            var city = await _cityRepository.GetByIdAsync(request.Id);                
            var cityDto = _mapper.Map<CityDto>(city);
            if (city == null)
            {                
                return new ApiResponseDTO<CityDto>
                {
                    IsSuccess = false,
                    Message = "City with ID {request.Id} not found."
                };   
            }       
                //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetById",
                actionCode: cityDto.CityCode,        
                actionName: cityDto.CityName,                
                details: $"City '{cityDto.CityName}' was created. CityCode: {cityDto.CityCode}",
                module:"City"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<CityDto>
            {
                IsSuccess = true,
                Message = "Success",
                Data = cityDto
            };              
        }
    }
}