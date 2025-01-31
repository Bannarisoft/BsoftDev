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
            var countryExists = await _countryRepository.GetCountryByCodeAsync(request.CountryName,request.CountryCode);
            if (countryExists != null)
            {
                  // Handle the case where the country exists
                if ((byte)countryExists.IsActive == (byte)Enums.Status.Inactive)
                {
                    // Reactivate the country or handle it differently
                    countryExists.IsActive = Enums.Status.Active;
                    await _countryRepository.UpdateAsync(countryExists.Id, countryExists);

                    return new ApiResponseDTO<CountryDto>
                    {
                        IsSuccess = false,
                        Message = "CountryCode already exists but was inactive. It has now been reactivated."
                    };
                }

                return new ApiResponseDTO<CountryDto>
                {
                    IsSuccess = false,
                    Message = "CountryCode already exists and is active."
                };
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
                    actionCode: countryDto.CountryCode,
                    actionName: countryDto.CountryName,                            
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