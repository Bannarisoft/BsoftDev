using AutoMapper;
using Contracts.Interfaces.External.IFixedAssetManagement;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.ICountry;
using Core.Application.Country.Queries.GetCountries;
using Core.Domain.Entities;
using Core.Domain.Enums.Common;
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
        private readonly IFixedAssetCountryValidationGrpcClient _fixedAssetCountryValidationGrpcClient;

        public DeleteCountryCommandHandler(ICountryCommandRepository countryRepository, IMapper mapper, ICountryQueryRepository countryQueryRepository, IMediator mediator, IFixedAssetCountryValidationGrpcClient fixedAssetCountry)
        {
            _countryRepository = countryRepository;
            _mapper = mapper;
            _countryQueryRepository = countryQueryRepository;
            _mediator = mediator;
            _fixedAssetCountryValidationGrpcClient = fixedAssetCountry;
        }       
        public async Task<ApiResponseDTO<CountryDto>> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
        {
            bool iscountryUsedInFixedAsset = await _fixedAssetCountryValidationGrpcClient.CheckIfCountryIsUsedForFixedAssetAsync(request.Id);

            if (iscountryUsedInFixedAsset)
            {
                return new ApiResponseDTO<CountryDto>
                {
                    IsSuccess = false,
                    Message = "Cannot delete Country. It is still in use in FixedAsset system."
                   
                };
            }

            var country = await _countryQueryRepository.GetByIdAsync(request.Id);
            if (country is null || country.IsDeleted is Enums.IsDelete.Deleted)
            {
                return new ApiResponseDTO<CountryDto>
                {
                    IsSuccess = false,
                    Message = "Invalid CountryID. The specified Country does not exist or is inactive."
                };
            }         

            var state = await _countryQueryRepository.GetStateByCountryIdAsync(request.Id);            
            if (state.Count>0)
            {                
                 return new ApiResponseDTO<CountryDto>
                {
                    IsSuccess = false,
                    Message = "State already exists for this country.Cannot delete the country."
                };
            }
                                    
            var countryDelete = _mapper.Map<Countries>(request);
            var updateResult = await _countryRepository.DeleteAsync(request.Id, countryDelete);
            if (updateResult > 0)
            {
                var countryDto = _mapper.Map<CountryDto>(countryDelete); 
                //Domain Event  
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Delete",
                    actionCode: countryDto.CountryCode ?? string.Empty,
                    actionName: countryDto.CountryName ?? string.Empty,
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