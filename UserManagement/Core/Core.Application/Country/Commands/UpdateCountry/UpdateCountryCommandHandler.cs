using MediatR;
using Core.Domain.Entities;
using Core.Application.Country.Queries.GetCountries;
using AutoMapper;
using Core.Application.Common.Interfaces.ICountry;
using Core.Domain.Events;
using Core.Application.Common.HttpResponse;
using Core.Domain.Enums.Common;
using FluentValidation;


namespace Core.Application.Country.Commands.UpdateCountry
{    
    public class UpdateCountryCommandHandler : IRequestHandler<UpdateCountryCommand,CountryDto>
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
        public async Task<CountryDto> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
        {
            var country = await _countryQueryRepository.GetByIdAsync(request.Id);
            if (country is null)
            throw new ValidationException("Country not found");
              
            var oldCountryName = country.CountryName;
            country.CountryName = request.CountryName;
            if (country is null || country.IsDeleted is Enums.IsDelete.Deleted)
            {
                throw new ValidationException("Invalid CountryID. The specified Country does not exist or is inactive.");
              
            }   
                      
            if ((byte)country.IsActive != request.IsActive)
            {    
                 country.IsActive =  (Enums.Status)request.IsActive;             
                await _countryRepository.UpdateAsync(country.Id, country);
                if (request.IsActive is 0)
                {
                    throw new ValidationException("CountryCode DeActivated.");
                  
                }
                else{
                    throw new ValidationException("CountryCode Activated.");
                  
                }                                     
            }
            var countryExists = await _countryRepository.GetCountryByCodeAsync(request.CountryName ?? string.Empty,request.CountryCode ?? string.Empty);            
            if (countryExists.Id !=0)
            {                   
                await _countryRepository.UpdateAsync(countryExists.Id, countryExists); 
                throw new ValidationException($"CountryCode already exists and is {(Enums.Status)request.IsActive}.");
                                                    
                
            }            
            var updatedCountryEntity = _mapper.Map<Countries>(request);
            
            var updateResult = await _countryRepository.UpdateAsync(request.Id, updatedCountryEntity);            
            var updatedCountry = await _countryQueryRepository.GetByIdAsync(request.Id);
            
            if (updatedCountry != null)
            {
                var countryDto = _mapper.Map<CountryDto>(updatedCountry);
                //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Update",
                    actionCode: countryDto.CountryCode ?? string.Empty,
                    actionName: countryDto.CountryName ?? string.Empty,                            
                    details: $"State '{oldCountryName}' was updated to '{countryDto.CountryName}'.  StateCode: {countryDto.CountryCode}",
                    module:"State"
                );            
                await _mediator.Publish(domainEvent, cancellationToken);
                if(updateResult>0)
                {
                    return  countryDto;
                }
                throw new Exception("Country not updated.");
                
            }
            else
            {
                throw new ValidationException("Country update failed");
               
            }                   
        }
    }
}