using System.Data;
using AutoMapper;
using Core.Application.City.Queries.GetCities;
using Core.Application.City.Queries.GetCityById;
using Core.Application.Common;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.ICity;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.City.Queries.GetCityById
{
    public class GetCityByIdQueryHandler : IRequestHandler<GetCityByIdQuery, Result<CityDto>>
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

        public async Task<Result<CityDto>> Handle(GetCityByIdQuery request, CancellationToken cancellationToken)
        {   
            try
            {      
                var city = await _cityRepository.GetByIdAsync(request.Id);
                if (city == null)
                {
                    return Result<CityDto>.Failure($"City with ID {request.Id} not found.");
                }
                
                var cityDto = _mapper.Map<CityDto>(city);
                 //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: cityDto.CityCode,        
                    actionName: cityDto.CityName,                
                    details: $"City '{cityDto.CityName}' was created. CityCode: {cityDto.CityCode}",
                    module:"City"
                );

                await _mediator.Publish(domainEvent, cancellationToken);
                return Result<CityDto>.Success(cityDto);
            }
            catch (Exception ex)
            {                
                return Result<CityDto>.Failure($"An error occurred while fetching the City: {ex.Message}");
            }
        }
    }
}