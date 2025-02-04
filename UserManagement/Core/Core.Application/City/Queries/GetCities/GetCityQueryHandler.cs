using MediatR;
using AutoMapper;
using Core.Application.Common.Interfaces.ICity;
using Core.Domain.Events;
using Core.Application.Common.HttpResponse;

namespace Core.Application.City.Queries.GetCities
{
    public class GetCityQueryHandler : IRequestHandler<GetCityQuery, ApiResponseDTO<List<CityDto>>>
    {
        private readonly ICityQueryRepository _cityRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

        public GetCityQueryHandler(ICityQueryRepository cityRepository , IMapper mapper, IMediator mediator)
        {
            _cityRepository = cityRepository;
            _mapper = mapper;
            _mediator = mediator;
        }
        public async Task<ApiResponseDTO<List<CityDto>>> Handle(GetCityQuery request, CancellationToken cancellationToken)
        {          
            var cities = await _cityRepository.GetAllCityAsync();
            var citiesList = _mapper.Map<List<CityDto>>(cities);
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetAll",
                actionCode: "",        
                actionName: "",
                details: $"City details was fetched.",
                module:"City"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<CityDto>>
            {
                IsSuccess = true,
                Message = "Success",
                Data = citiesList
            };            
        }
    }
}