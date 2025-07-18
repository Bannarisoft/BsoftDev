using Core.Application.Country.Queries.GetCountries;
using AutoMapper;
using MediatR;
using Core.Application.Common.Interfaces.ICountry;
using Core.Domain.Events;
using Core.Application.Common.HttpResponse;

namespace Core.Application.Country.Queries.GetCountryAutoComplete
{
    public class GetCountryAutoCompleteQueryHandler : IRequestHandler<GetCountryAutoCompleteQuery, ApiResponseDTO<List<CountryAutoCompleteDTO>>>
    {
        private readonly ICountryQueryRepository _countryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

        public GetCountryAutoCompleteQueryHandler(ICountryQueryRepository countryRepository, IMapper mapper, IMediator mediator)
        {
            _countryRepository =countryRepository;
            _mapper =mapper;
            _mediator = mediator;
        }

        public async Task<ApiResponseDTO<List<CountryAutoCompleteDTO>>> Handle(GetCountryAutoCompleteQuery request, CancellationToken cancellationToken)
        {   
                   
            var result = await _countryRepository.GetByCountryNameAsync(request.SearchPattern ?? string.Empty);
            if (result is null || result.Count is 0)
            {
                return new ApiResponseDTO<List<CountryAutoCompleteDTO>>
                {
                    IsSuccess = false,
                    Message = "No countries found matching the search pattern."
                };
            }
            var countryDto = _mapper.Map<List<CountryAutoCompleteDTO>>(result);
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetAutoComplete",
                actionCode:"",        
                actionName: request.SearchPattern ?? string.Empty,                
                details: $"Country '{request.SearchPattern}' was searched",
                module:"Country"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            return new ApiResponseDTO<List<CountryAutoCompleteDTO>>
            {
                IsSuccess = true,
                Message = "Countries found successfully.",
                Data = countryDto
            };                        
        }
    }
  
}