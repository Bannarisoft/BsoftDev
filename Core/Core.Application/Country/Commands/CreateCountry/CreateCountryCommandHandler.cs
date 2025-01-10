using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.Interfaces.ICountry;
using Core.Application.Country.Commands.CreateCountry;
using Core.Application.Country.Queries.GetCountries;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;

public class CreateCountryCommandHandler : IRequestHandler<CreateCountryCommand, Result<CountryDto>>
{
    private readonly IMapper _mapper;
    private readonly ICountryCommandRepository _countryRepository;    
    private readonly IMediator _mediator; 

    // Constructor Injection
    public CreateCountryCommandHandler(IMapper mapper, ICountryCommandRepository countryRepository, IMediator mediator)
    {
        _mapper = mapper;
        _countryRepository = countryRepository; 
        _mediator = mediator;               
    }

    public async Task<Result<CountryDto>> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
    {
        var countryExists = await _countryRepository.GetCountryByCodeAsync(request.CountryCode);
        if (countryExists)
        {
            return Result<CountryDto>.Failure("CountryCode already exists");
        }
        var countryEntity = _mapper.Map<Countries>(request);    
        try
        {    
            var result = await _countryRepository.CreateAsync(countryEntity);
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: result.CountryCode,
                actionName: result.CountryName,
                details: $"Country '{result.CountryName}' was created. CountryCode: {result.CountryCode}",
                module:"Country"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            
            var countryDto = _mapper.Map<CountryDto>(result);
            return Result<CountryDto>.Success(countryDto);
        }
        catch (Exception ex)
        {
            return Result<CountryDto>.Failure($"An error occurred while creating the Country: {ex.Message}");
        }
    }
}
