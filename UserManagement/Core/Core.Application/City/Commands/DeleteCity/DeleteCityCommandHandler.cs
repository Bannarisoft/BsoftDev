using AutoMapper;
using Contracts.Interfaces.External.IFixedAssetManagement;
using Core.Application.City.Queries.GetCities;
using Core.Application.Common.HttpResponse;
using Core.Application.Common.Interfaces.ICity;
using Core.Domain.Entities;
using Core.Domain.Enums.Common;
using Core.Domain.Events;
using MediatR;

namespace Core.Application.City.Commands.DeleteCity
{
    public class DeleteCityCommandHandler : IRequestHandler<DeleteCityCommand, ApiResponseDTO<CityDto>>
    {
        private readonly ICityCommandRepository _cityRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 
        private readonly ICityQueryRepository _cityQueryRepository;
        private readonly IFixedAssetCityValidationGrpcClient _fixedAssetCityValidationGrpcClient;
        public DeleteCityCommandHandler(ICityCommandRepository cityRepository, IMapper mapper, ICityQueryRepository cityQueryRepository, IMediator mediator, IFixedAssetCityValidationGrpcClient fixedAssetCityValidationGrpcClient)
        {
            _cityRepository = cityRepository;
            _mapper = mapper;
            _cityQueryRepository = cityQueryRepository;
            _mediator = mediator;
            _fixedAssetCityValidationGrpcClient = fixedAssetCityValidationGrpcClient;
        }

        public async Task<ApiResponseDTO<CityDto>> Handle(DeleteCityCommand request, CancellationToken cancellationToken)
        {
            bool iscountryUsedInFixedAsset = await _fixedAssetCityValidationGrpcClient.CheckIfCityIsUsedForFixedAssetAsync(request.Id);  

            if (iscountryUsedInFixedAsset)
            {
                return new ApiResponseDTO<CityDto>
                {
                    IsSuccess = false,
                    Message = "Cannot delete Country. It is still in use in FixedAsset system."
                   
                };
            }

            // Fetch the city to be deleted
            var city = await _cityQueryRepository.GetByIdAsync(request.Id);
            if (city is null || city.IsDeleted is Enums.IsDelete.Deleted )
            {
                return new ApiResponseDTO<CityDto>
                {
                    IsSuccess = false,
                    Message = "Invalid CityID. The specified City does not exist or is inactive."
                };
            }
            var cityDelete = _mapper.Map<Cities>(request);           
           
            var updateResult = await _cityRepository.DeleteAsync(request.Id, cityDelete);
            if (updateResult > 0)
            {
                var cityDto = _mapper.Map<CityDto>(cityDelete);  
                //Domain Event  
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Delete",
                    actionCode: cityDto.CityCode ?? string.Empty,
                    actionName: cityDto.CityName ?? string.Empty,
                    details: $"City '{cityDto.CityName}' was created. CityCode: {cityDto.CityCode}",
                    module:"City"
                );               
                await _mediator.Publish(domainEvent, cancellationToken);                 
                return new ApiResponseDTO<CityDto>
                {
                    IsSuccess = true,
                    Message = "City deleted successfully.",
                    Data = cityDto
                };
            }

            return new ApiResponseDTO<CityDto>
            {
                IsSuccess = false,
                Message = "City deletion failed."                             
            };
           
        }
    }
}
