using Core.Application.Common.Interfaces;
using MediatR;
using Core.Domain.Entities;
using AutoMapper;
using Core.Application.City.Queries.GetCities;
using Core.Application.Common;
using Core.Application.Common.Interfaces.ICity;
using Core.Domain.Events;
using Core.Application.Common.HttpResponse;
using Core.Domain.Enums.Common;

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
                    Message = "Invalid CityID. The specified City does not exist or is inactive."
                };

            var oldCityName = city.CityName;
            city.CityName = request.CityName;

            if (city == null || city.IsDeleted == Enums.IsDelete.Deleted )
            {
                return new ApiResponseDTO<CityDto>
                {
                    IsSuccess = false,
                    Message = "Invalid CityID. The specified City does not exist or is deleted."
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
 
            // Check if the city name already exists in the same state
            var cityExistsByName = await _cityRepository.GetCityByNameAsync(request.CityName ?? string.Empty,request.CityCode ?? string.Empty, request.StateId);
           /*  if (cityExistsByName)
            {
                return new ApiResponseDTO<CityDto> {
                    IsSuccess = false, 
                    Message = "City with the same name & code already exists in the specified state."
                };
            }   */
            if (cityExistsByName!= null)
            {  
                if ((byte)cityExistsByName.IsActive == request.IsActive)
                {     
                    if ((byte)cityExistsByName.IsActive==0)
                    {                
                        return new ApiResponseDTO<CityDto>
                        {
                            IsSuccess = false,
                            Message = $"CityCode already exists and is {(Enums.Status) request.IsActive}."
                        };
                    }
                }
                if ((byte)cityExistsByName.IsActive != request.IsActive)
                {
                    // Reactivate the country or handle it differently
                    cityExistsByName.IsActive =  (Enums.Status)request.IsActive;                    
                    cityExistsByName.StateId =  request.StateId;
                    cityExistsByName.CityCode =  request.CityCode;
                    cityExistsByName.CityName =  request.CityName;
                    await _cityRepository.UpdateAsync(cityExistsByName.Id, cityExistsByName);
                    if (request.IsActive==0)
                    {
                        return new ApiResponseDTO<CityDto>
                        {
                            IsSuccess = false,
                            Message = "CityCode DeActivated."
                        };
                        }
                        else{
                            return new ApiResponseDTO<CityDto>
                        {
                            IsSuccess = false,
                            Message = "CityCode Activated."
                        }; 
                    }                    
                }
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
                    actionCode: cityDto.CityCode ?? string.Empty,
                    actionName: cityDto.CityName ?? string.Empty,                            
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