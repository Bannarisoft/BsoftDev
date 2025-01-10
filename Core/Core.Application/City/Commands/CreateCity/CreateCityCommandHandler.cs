using Core.Domain.Entities;
using Core.Application.Common.Interfaces;
using AutoMapper;
using MediatR;
using Core.Application.City.Queries.GetCities;
using Core.Application.Common;
using Core.Application.Common.Interfaces.ICity;
using Core.Domain.Events;

namespace Core.Application.City.Commands.CreateCity
{    
    public class CreateCityCommandHandler : IRequestHandler<CreateCityCommand, Result<CityDto>>
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

        public async Task<Result<CityDto>> Handle(CreateCityCommand request, CancellationToken cancellationToken)
        {
            var stateExists = await _cityRepository.StateExistsAsync(request.StateId);
            if (!stateExists)
            {
                return Result<CityDto>.Failure("Invalid StateId. The specified state does not exist or is inactive.");
            }
            var cityExists = await _cityRepository.GetCityByCodeAsync(request.CityCode, request.StateId);
            if (cityExists)
            {
            return Result<CityDto>.Failure("CityCode already exists in the specified state.");
            }
            // Map the CreateCityCommand to the City entity
            var cityEntity = _mapper.Map<Cities>(request);
            try
            {  
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
                return Result<CityDto>.Success(cityDto);
            }
            catch (Exception ex)
            {
                return Result<CityDto>.Failure($"An error occurred while creating the City: {ex.Message}");
            }
        }
    }
}
