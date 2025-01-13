using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.Interfaces.ICountry;
using Core.Application.Country.Queries.GetCountries;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Country.Commands.DeleteCountry
{  
  public class DeleteCountryCommandHandler : IRequestHandler<DeleteCountryCommand, Result<CountryDto>>
    {
        private readonly ICountryCommandRepository _countryRepository;
        private readonly ICountryQueryRepository _countryQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 
        
        public DeleteCountryCommandHandler(ICountryCommandRepository countryRepository, IMapper mapper, ICountryQueryRepository countryQueryRepository, IMediator mediator)
        {
            _countryRepository = countryRepository;
            _mapper = mapper;
            _countryQueryRepository = countryQueryRepository;
            _mediator = mediator;
        }       
        public async Task<Result<CountryDto>> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
        {
            var country = await _countryQueryRepository.GetByIdAsync(request.Id);
            if (country == null || country.IsActive != 1)
            {
                return Result<CountryDto>.Failure("Invalid CountryID. The specified Country does not exist or is inactive.");
            }                       
            var countryUpdate = new Countries
            {
                Id = request.Id,
                CountryCode = country.CountryCode, 
                CountryName = country.CountryName, 
                IsActive = 0
            };
            try
            {
                var updateResult = await _countryRepository.DeleteAsync(request.Id, countryUpdate);
                if (updateResult > 0)
                {
                    var countryDto = _mapper.Map<CountryDto>(countryUpdate); 
                    //Domain Event  
                    var domainEvent = new AuditLogsDomainEvent(
                        actionDetail: "Delete",
                        actionCode: countryDto.CountryCode,
                        actionName: countryDto.CountryName,
                        details: $"Country '{countryDto.CountryName}' was created. CountryCode: {countryDto.CountryCode}",
                        module:"Country"
                    );               
                    await _mediator.Publish(domainEvent, cancellationToken);              
                    return Result<CountryDto>.Success(countryDto);
                }
                return Result<CountryDto>.Failure("Country deletion failed.");
            }
            catch (Exception ex)
            {
                return Result<CountryDto>.Failure($"An error occurred while deleting the Country: {ex.Message}");
            }
        }
    }
}