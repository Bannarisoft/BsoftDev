using MediatR;
using Core.Domain.Entities;
using Core.Application.Country.Queries.GetCountries;
using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.Interfaces.ICountry;
using Core.Domain.Events;


namespace Core.Application.Country.Commands.UpdateCountry
{    
    public class UpdateCountryCommandHandler : IRequestHandler<UpdateCountryCommand,Result<CountryDto>>    
    {
        private readonly ICountryCommandRepository _countryRepository;
        private readonly IMapper _mapper;
        private readonly ICountryQueryRepository _countryQueryRepository;
        private readonly IMediator _mediator; 

        public UpdateCountryCommandHandler(ICountryCommandRepository countryRepository, IMapper mapper, ICountryQueryRepository countryQueryRepository, IMediator mediator)
        {
            _countryRepository = countryRepository;
             _mapper = mapper;
            _countryQueryRepository = countryQueryRepository;
            _mediator = mediator;
        }       
        public async Task<Result<CountryDto>> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
        {
            var country = await _countryQueryRepository.GetByIdAsync(request.Id);
            if (country == null)
                return Result<CountryDto>.Failure("Country not found.");

            var oldCountryName = country.CountryName;
            country.CountryName = request.CountryName;
            if (country == null || country.IsActive != 1)
            {
                return Result<CountryDto>.Failure("Invalid CountryID. The specified Country does not exist or is inactive.");
            }           
            var countryExists = await _countryRepository.GetCountryByCodeAsync(request.CountryCode);
            if (countryExists)
            {
                return Result<CountryDto>.Failure("CountryCode already exists");
            }
            var updatedCountryEntity = _mapper.Map<Countries>(request);
            try
            {
                var updateResult = await _countryRepository.UpdateAsync(request.Id, updatedCountryEntity);            
                var updatedCountry = await _countryQueryRepository.GetByIdAsync(request.Id);
                
                if (updatedCountry != null)
                {
                    var countryDto = _mapper.Map<CountryDto>(updatedCountry);
                    //Domain Event
                    var domainEvent = new AuditLogsDomainEvent(
                        actionDetail: "Update",
                        actionCode: countryDto.CountryCode,
                        actionName: countryDto.CountryName,                            
                        details: $"State '{oldCountryName}' was updated to '{countryDto.CountryName}'.  StateCode: {countryDto.CountryCode}",
                        module:"State"
                    );            
                    await _mediator.Publish(domainEvent, cancellationToken);
                    return Result<CountryDto>.Success(countryDto);
                }
                else
                {
                    return Result<CountryDto>.Failure("Country update failed.");
                }
            }
            catch (Exception ex)
            {
                return Result<CountryDto>.Failure($"An error occurred while updating the Country: {ex.Message}");
            }          
      }
    }
}