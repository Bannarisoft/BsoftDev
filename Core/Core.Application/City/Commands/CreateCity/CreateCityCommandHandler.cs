using Core.Domain.Entities;
using Core.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using Core.Application.City.Queries.GetCities;
using Core.Application.Common;
using Core.Application.Common.Interfaces.ICity;
using Core.Domain.Events;
using Core.Application.Common.HttpResponse;

namespace Core.Application.City.Commands.CreateCity
{    
    public class CreateCityCommandHandler : IRequestHandler<CreateCityCommand, ApiResponseDTO<CityDto>>
    {
        private readonly IMapper _mapper;
        private readonly ICityCommandRepository _cityRepository;
        private readonly IMediator _mediator; 

        // Constructor Injection
        public CreateCityCommandHandler(IMapper mapper, ICityCommandRepository cityRepository, IMediator mediator)
        {
            _mapper = mapper;
            _cityRepository = cityRepository;
            _mediator = mediator;    
        }

        public async Task<ApiResponseDTO<CityDto>> Handle(CreateCityCommand request, CancellationToken cancellationToken)
        {
            var stateExists = await _cityRepository.StateExistsAsync(request.StateId);
            if (!stateExists)
            {
                 return new ApiResponseDTO<CityDto>{
                    IsSuccess = false, 
                    Message = "Invalid StateId. The specified state does not exist or is inactive."
                    };               
            }      
            // Check if the city name already exists in the same state
            var cityExistsByName = await _cityRepository.GetCityByNameAsync(request.CityName,request.CityCode, request.StateId);
            if (cityExistsByName)
            {
                return new ApiResponseDTO<CityDto> {
                    IsSuccess = false, 
                    Message = "City name & code already exists in the specified state."
                };
            }    
            var cityEntity = _mapper.Map<Cities>(request);            
            var result = await _cityRepository.CreateAsync(cityEntity);
            
            //Domain Event
            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "Create",
                actionCode: result.CityCode,
                actionName: result.CityName,
                details: $"City '{result.CityName}' was created. CityCode: {result.CityCode}",
                module:"City"
            );
            await _mediator.Publish(domainEvent, cancellationToken);
            
            var cityDto = _mapper.Map<CityDto>(result);
            if (cityDto.Id > 0)
            {
                return new ApiResponseDTO<CityDto>{
                    IsSuccess = true, 
                    Message = "City created successfully.",
                    Data = cityDto
                };
            }
            return  new ApiResponseDTO<CityDto>{
                IsSuccess = false, 
                Message = "City not created."
            };           
        }
    }
}
