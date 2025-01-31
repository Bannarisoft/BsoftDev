using Core.Application.Common.Interfaces;
using MediatR;
using Core.Domain.Entities;
using AutoMapper;
using Core.Application.City.Queries.GetCities;
using Core.Application.Common;
using Core.Application.Common.Interfaces.ICity;
using Core.Domain.Events;
using Core.Application.Common.HttpResponse;

namespace Core.Application.City.Commands.UpdateCity
{       
    public class UpdateCityCommandHandler : IRequestHandler<UpdateCityCommand, ApiResponseDTO<CityDto>>
    {
        private readonly ICityCommandRepository _cityRepository;
        private readonly ICityQueryRepository _cityQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator; 

        public UpdateCityCommandHandler(ICityCommandRepository cityRepository, IMapper mapper,ICityQueryRepository cityQueryRepository, IMediator mediator)
        {
            _cityRepository = cityRepository;
            _mapper = mapper;
            _cityQueryRepository = cityQueryRepository;
            _mediator = mediator;
        }
        public async Task<ApiResponseDTO<CityDto>> Handle(UpdateCityCommand request, CancellationToken cancellationToken)
        {
            var city = await _cityQueryRepository.GetByIdAsync(request.Id);
            if (city == null)
                return new ApiResponseDTO<CityDto>
                {
                    IsSuccess = false,
                    Message = "City not found"
                };

            var oldCityName = city.CityName;
            city.CityName = request.CityName;

            if (city == null || city.IsDeleted != Domain.Enums.Common.Enums.IsDelete.Deleted)
            {
                return new ApiResponseDTO<CityDto>
                {
                    IsSuccess = false,
                    Message = "Invalid CityID. The specified City does not exist or is inactive."
                };
            }

            var stateExists = await _cityRepository.StateExistsAsync(request.StateId);
            if (!stateExists)
            {
                return new ApiResponseDTO<CityDto>
                {
                    IsSuccess = false,
                    Message = "Invalid StateId. The specified state does not exist or is inactive."
                };
            }

            var cityExists = await _cityRepository.GetCityByCodeAsync(request.CityCode, request.StateId);
            if (cityExists)
            {
                return new ApiResponseDTO<CityDto>
                {
                    IsSuccess = false,
                    Message = "CityCode already exists in the specified State."
                };
            }

            var updatedCityEntity = _mapper.Map<Cities>(request);                   
            var updateResult = await _cityRepository.UpdateAsync(request.Id, updatedCityEntity);            

            var updatedCity =  await _cityQueryRepository.GetByIdAsync(request.Id);    
            if (updatedCity != null)
            {
                var cityDto = _mapper.Map<CityDto>(updatedCity);
                //Domain Event
                var domainEvent = new AuditLogsDomainEvent(
                    actionDetail: "Update",
                    actionCode: cityDto.CityCode,
                    actionName: cityDto.CityName,                            
                    details: $"State '{oldCityName}' was updated to '{cityDto.CityName}'.  StateCode: {cityDto.CityCode}",
                    module:"State"
                );            
                await _mediator.Publish(domainEvent, cancellationToken);
                if(updateResult>0)
                {
                    return new ApiResponseDTO<CityDto>
                    {
                        IsSuccess = true,
                        Message = "City updated successfully.",
                        Data = cityDto
                    };
                }
                return new ApiResponseDTO<CityDto>
                {
                    IsSuccess = false,
                    Message = "City not updated."
                };                
            }
            else
            {
                return new ApiResponseDTO<CityDto>{
                    IsSuccess = false,
                    Message = "City not found."
                };
            }
            
        }
    }
}