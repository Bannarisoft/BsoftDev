using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Contracts.Interfaces.External.IUser;
using Core.Application.Common.Interfaces.IPartyMaster;
using Core.Domain.Events;
using MediatR;
using static Core.Application.PartyMaster.Queries.GetPartyMasterById.PartyMasterDto;

namespace Core.Application.PartyMaster.Queries.GetPartyMasterById
{
    public class GetPartyMasterByIdQueryHandler : IRequestHandler<GetPartyMasterByIdQuery, PartyMasterDto>
    {
        private readonly IPartyMasterQueryRepository _ipartyMasterQueryRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ICityGrpcClient _cityGrpcClient;
        private readonly IStatesGrpcClient _stateGrpcClient;
        private readonly ICountryGrpcClient _countryGrpcClient;

        public GetPartyMasterByIdQueryHandler(IPartyMasterQueryRepository ipartyMasterQueryRepository, IMapper mapper, IMediator mediator, ICityGrpcClient cityGrpcClient, IStatesGrpcClient stateGrpcClient, ICountryGrpcClient countryGrpcClient)
        {
            _ipartyMasterQueryRepository = ipartyMasterQueryRepository;
            _mapper = mapper;
            _mediator = mediator;
            _cityGrpcClient = cityGrpcClient;
            _stateGrpcClient = stateGrpcClient;
            _countryGrpcClient = countryGrpcClient;
        }
        public async Task<PartyMasterDto> Handle(GetPartyMasterByIdQuery request, CancellationToken cancellationToken)
        {
           var dto = await _ipartyMasterQueryRepository.GetByIdPartyMasterAsync(request.PartyId);
           
              // Run gRPC calls concurrently
                var cityTask    = _cityGrpcClient.GetAllCityAsync();
                var stateTask   = _stateGrpcClient.GetAllStateAsync();
                var countryTask = _countryGrpcClient.GetAllCountryAsync();

                await Task.WhenAll(cityTask, stateTask, countryTask);

                var cityDict    = cityTask.Result.ToDictionary(x => x.CityId, x => x.CityName);
                var stateDict   = stateTask.Result.ToDictionary(x => x.StateId, x => x.StateName);
                var countryDict = countryTask.Result.ToDictionary(x => x.CountryId, x => x.CountryName);

                // Map address City/State/Country names
                if (dto.PartyAddresses != null && dto.PartyAddresses.Any())
                {
                    foreach (var addr in dto.PartyAddresses)
                    {
                        if (addr.CityId.HasValue && cityDict.TryGetValue(addr.CityId.Value, out var cityName))
                            addr.City = cityName;

                        if (addr.StateId.HasValue && stateDict.TryGetValue(addr.StateId.Value, out var stateName))
                            addr.State = stateName;

                        if (addr.CountryId.HasValue && countryDict.TryGetValue(addr.CountryId.Value, out var countryName))
                            addr.Country = countryName;
                    }
                }
                        

            if (dto == null)
                throw new KeyNotFoundException("PartyId not found");

            var domainEvent = new AuditLogsDomainEvent(
                actionDetail: "GetById",
                actionCode: "GetPartyMasterByIdQuery",
                actionName: dto.Id.ToString(),
                details: $"Party details {dto.Id} fetched.",
                module: "PartyMaster"
            );

            await _mediator.Publish(domainEvent, cancellationToken);

            return dto;
        }
    }
}