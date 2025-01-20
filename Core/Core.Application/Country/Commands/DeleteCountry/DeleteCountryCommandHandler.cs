using AutoMapper;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.ICountry;
using Core.Application.Country.Queries.GetCountries;
using Core.Domain.Entities;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.Country.Commands.DeleteCountry
{  
  public class DeleteCountryCommandHandler : IRequestHandler<DeleteCountryCommand, ApiResponseDTO<CountryDto>>
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
        public async Task<ApiResponseDTO<CountryDto>> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
        {
            var country = await _countryQueryRepository.GetByIdAsync(request.Id);
            if (country == null || country.IsActive != 1)
            {
                return new ApiResponseDTO<CountryDto>
                {
                    IsSuccess = false,
                    Message = "Invalid CountryID. The specified Country does not exist or is inactive."
                };
            }                       
            var countryUpdate = new Countries
            {
                Id = request.Id,
                CountryCode = country.CountryCode, 
                CountryName = country.CountryName, 
                IsActive = 0
            };
            
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
                return new ApiResponseDTO<CountryDto>
                {
                    IsSuccess = true,
                    Message = "Country deleted successfully.",
                    Data = countryDto
                };
            }
            return new ApiResponseDTO<CountryDto>
            {
                IsSuccess = false,
                Message = "Country deletion failed."
            };          
        }
    }
}