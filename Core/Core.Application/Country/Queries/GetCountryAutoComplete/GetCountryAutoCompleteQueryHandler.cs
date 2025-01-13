using Core.Application.Country.Queries.GetCountries;
using AutoMapper;
using MediatR;
using Core.Application.Common;
using Core.Application.Common.Interfaces.ICountry;
using Core.Domain.Events;

namespace Core.Application.Country.Queries.GetCountryAutoComplete
{
    public class GetCountryAutoCompleteQueryHandler : IRequestHandler<GetCountryAutoCompleteQuery, Result<List<CountryDto>>>
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

        public async Task<Result<List<CountryDto>>> Handle(GetCountryAutoCompleteQuery request, CancellationToken cancellationToken)
        {   
            try
            {         
                var result = await _countryRepository.GetByCountryNameAsync(request.SearchPattern);
                if (!result.IsSuccess || result.Data == null || !result.Data.Any())
                {
                    return Result<List<CountryDto>>.Failure("No countries found matching the search pattern.");
                }
                var countryDto = _mapper.Map<List<CountryDto>>(result.Data);
                //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetAutoComplete",
                    actionCode:"",        
                    actionName: request.SearchPattern,                
                    details: $"Country '{request.SearchPattern}' was searched",
                    module:"Country"
                );
                await _mediator.Publish(domainEvent, cancellationToken);
                return Result<List<CountryDto>>.Success(countryDto);
            }
            catch (Exception ex)
            {
                return Result<List<CountryDto>>.Failure($"An error occurred while fetching the Country: {ex.Message}");
            }            
        }
    }
  
}