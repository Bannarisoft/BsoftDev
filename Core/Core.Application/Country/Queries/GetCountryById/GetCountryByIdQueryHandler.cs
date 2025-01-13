using Core.Application.Country.Queries.GetCountries;
using Core.Application.Common.Interfaces;
using MediatR;
using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.Interfaces.ICountry;
using Core.Domain.Events;

namespace Core.Application.Country.Queries.GetCountryById
{
    public class GetCountryByIdQueryHandler : IRequestHandler<GetCountryByIdQuery, Result<CountryDto>>
    {
        private readonly ICountryQueryRepository _countryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetCountryByIdQueryHandler(ICountryQueryRepository countryRepository, IMapper mapper, IMediator mediator)
        {
            _countryRepository = countryRepository;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<Result<CountryDto>> Handle(GetCountryByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var country = await _countryRepository.GetByIdAsync(request.Id);
                if (country == null)
                {
                    return Result<CountryDto>.Failure($"Country with ID {request.Id} not found.");
                }
                
                var countryDto = _mapper.Map<CountryDto>(country);
                  
                //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "GetById",
                    actionCode: countryDto.CountryCode,        
                    actionName: countryDto.CountryName,                
                    details: $"Country '{countryDto.CountryName}' was created. CountryCode: {countryDto.CountryCode}",
                    module:"Country"
                );

                await _mediator.Publish(domainEvent, cancellationToken);
                return Result<CountryDto>.Success(countryDto);
            }
            catch (Exception ex)
            {
                // Handle any unexpected exceptions
                return Result<CountryDto>.Failure($"An error occurred while fetching the Country: {ex.Message}");
            }
        }
    }

}