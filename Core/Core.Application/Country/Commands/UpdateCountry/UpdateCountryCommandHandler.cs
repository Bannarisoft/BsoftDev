using MediatR;
using Core.Domain.Entities;
using Core.Application.Country.Queries.GetCountries;
using AutoMapper;
using Core.Application.Common.Interfaces.ICountry;
using Core.Domain.Events;
using Core.Application.Common.HttpResponse;
using Core.Domain.Enums.Common;


namespace Core.Application.Country.Commands.UpdateCountry
{    
    public class UpdateCountryCommandHandler : IRequestHandler<UpdateCountryCommand,ApiResponseDTO<CountryDto>>    
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
        public async Task<ApiResponseDTO<CountryDto>> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
        {
            var country = await _countryQueryRepository.GetByIdAsync(request.Id);
            if (country == null)
                return new ApiResponseDTO<CountryDto>
                {
                    IsSuccess = false,
                    Message = "Country not found"
                };

            var oldCountryName = country.CountryName;
            country.CountryName = request.CountryName;
            if (country == null || country.IsDeleted == Enums.IsDelete.Deleted)
            {
                return new ApiResponseDTO<CountryDto>
                {
                    IsSuccess = false,
                    Message = "Invalid CountryID. The specified Country does not exist or is inactive."
                };
            }           
            var countryExists = await _countryRepository.GetCountryByCodeAsync(request.CountryName ?? string.Empty,request.CountryCode ?? string.Empty);
            if (countryExists != null)
            {
                  // Handle the case where the country exists
                if ((byte)countryExists.IsActive == request.IsActive)
                {
                    if ((byte)countryExists.IsActive==0)
                    {
                    //    await _countryRepository.UpdateAsync(countryExists.Id, countryExists); 
                        return new ApiResponseDTO<CountryDto>
                        {
                            IsSuccess = false,
                            Message = $"CountryCode already exists and is {(Enums.Status) request.IsActive}."
                        };
                    }                    
                }
                
                if ((byte)countryExists.IsActive != request.IsActive)
                {
                    // Reactivate the country or handle it differently
                    countryExists.IsActive =  (Enums.Status)request.IsActive;
                    countryExists.CountryName =  request.CountryName;
                    countryExists.CountryCode =  request.CountryCode;
                    await _countryRepository.UpdateAsync(countryExists.Id, countryExists);
                    if (request.IsActive==0)
                    {
                        return new ApiResponseDTO<CountryDto>
                        {
                            IsSuccess = false,
                            Message = "CountryCode DeActivated."
                        };
                        }
                        else{
                            return new ApiResponseDTO<CountryDto>
                        {
                            IsSuccess = false,
                            Message = "CountryCode Activated."
                        }; 
                    }                    
                }               
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
                    return new ApiResponseDTO<CountryDto>
                    {
                        IsSuccess = true,
                        Message = "Country updated successfully",
                        Data = countryDto
                    };
                }
                return new ApiResponseDTO<CountryDto>
                {
                    IsSuccess = false,
                    Message = "Country not updated."
                }; 
            }
            else
            {
                return new ApiResponseDTO<CountryDto>
                {
                    IsSuccess = false,
                    Message = "Country update failed"
                };
            }                   
        }
    }
}