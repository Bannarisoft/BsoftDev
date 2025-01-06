using AutoMapper;
using Core.Application.Common;
using Core.Application.Common.Interfaces;
using Core.Application.Common.Interfaces.ICountry;
using Core.Application.Country.Commands.CreateCountry;
using Core.Application.Country.Queries.GetCountries;
using Core.Domain.Entities;
using MediatR;

public class CreateCountryCommandHandler : IRequestHandler<CreateCountryCommand, Result<CountryDto>>
{
    private readonly IMapper _mapper;
    private readonly ICountryCommandRepository _countryRepository;
    private readonly IAuditLogRepository _auditLogRepository;    

    // Constructor Injection
    public CreateCountryCommandHandler(IMapper mapper, ICountryCommandRepository countryRepository, IAuditLogRepository auditLogRepository)
    {
        _mapper = mapper;
        _countryRepository = countryRepository;
        _auditLogRepository = auditLogRepository;        
    }

    public async Task<Result<CountryDto>> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
    {
        var countryExists = await _countryRepository.GetCountryByCodeAsync(request.CountryCode);
        if (countryExists)
        {
            return Result<CountryDto>.Failure("CountryCode already exists");
        }
        var countryEntity = _mapper.Map<Countries>(request);        
        var result = await _countryRepository.CreateAsync(countryEntity);
        
        var auditLog = new AuditLogs
        {
            Action = "Create",
            Details = $"Country created: {result.CountryName} (Code: {result.CountryCode})",
            Module = "Country",
            CreatedAt = DateTime.UtcNow
        };
        await _auditLogRepository.CreateAsync(auditLog);
        
        var countryDto = _mapper.Map<CountryDto>(result);
        return Result<CountryDto>.Success(countryDto);
    }
}
