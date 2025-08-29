using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BackgroundService.Domain.Common;
using Core.Application.Common.Interfaces.ICity;
using Core.Application.Common.Interfaces.ICountry;
using Core.Application.Common.Interfaces.IState;
using Core.Domain.Entities;
using Grpc.Core;
using Hangfire.States;
using MassTransit.Futures.Contracts;
using Shared.Grpc;

namespace UserManagement.API.GrpcServices
{
    public class GetorCreatelocationGrpService : LocationService.LocationServiceBase
    {
        private readonly ICountryQueryRepository _countryQueryRepository;
        private readonly ICountryCommandRepository _countryCommandRepository;
        private readonly IStateQueryRepository _stateQueryRepository;
        private readonly IStateCommandRepository _stateCommandRepository;
        private readonly ICityQueryRepository _cityQueryRepository;
        private readonly ICityCommandRepository _cityCommandRepository;

        public GetorCreatelocationGrpService(ICountryQueryRepository countryQueryRepository, IStateQueryRepository stateQueryRepository, ICityQueryRepository cityQueryRepository, ICountryCommandRepository countryCommandRepository, IStateCommandRepository stateCommandRepository, ICityCommandRepository cityCommandRepository)
        {
            _countryQueryRepository = countryQueryRepository;
            _stateQueryRepository = stateQueryRepository;
            _cityQueryRepository = cityQueryRepository;
            _countryCommandRepository = countryCommandRepository;
            _stateCommandRepository = stateCommandRepository;
            _cityCommandRepository = cityCommandRepository;
        }
        //    public override async Task<LocationResponse> GetOrCreateLocation(LocationRequest request, ServerCallContext context)
        //     {
        //         // Step 1: Check or create Country
        //         var countries = await _countryQueryRepository.GetByCountryNameAsync(request.Country);
        //         var country = countries.FirstOrDefault();

        //         if (country == null)
        //         {
        //             country = new Countries
        //             {
        //                 CountryName = request.Country,
        //                 CountryCode = request.Country.Substring(0, 3).ToUpper() // Example: first 3 letters
        //             };

        //             country = await _countryCommandRepository.CreateAsync(country);
        //         }

        //         // Step 2: Check or create State
        //         var states = await _stateQueryRepository.GetByStateNameAsync(request.State);
        //         var state = states.FirstOrDefault();

        //         if (state == null)
        //         {
        //             state = new States
        //             {
        //                 StateCode = request.State.Substring(0, 3).ToUpper(),
        //                 StateName = request.State,
        //                 CountryId = country.Id

        //             };

        //             state = await _stateCommandRepository.CreateAsync(state);
        //         }

        //         // Step 3: Check or create City
        //         var cities = await _cityQueryRepository.GetByCityNameAsync(request.City);
        //         var city = cities.FirstOrDefault();

        //         if (city == null)
        //         {
        //             city = new Cities
        //             {
        //                 StateId = state.Id,
        //                 CityCode = request.City.Substring(0, 3).ToUpper(),
        //                 CityName = request.City,
        //             };

        //             city = await _cityCommandRepository.CreateAsync(city);
        //         }

        //         // Step 4: Return response
        //         return new LocationResponse
        //         {
        //             CityId = city.Id,
        //             StateId = state.Id,
        //             CountryId = country.Id,       
        //         };
        //     }
    
        public override async Task<LocationResponse> GetOrCreateLocation(LocationRequest request, ServerCallContext context)
        {
            // 🔹 Normalization helper method → Trim + ToLower + Remove spaces.
            string Normalize(string input) =>
                string.IsNullOrWhiteSpace(input)
                    ? string.Empty
                    : input.Trim().ToLower().Replace(" ", "");

            // --- Step 1: Country ---
            var normalizedCountry = Normalize(request.Country);
            var countries = await _countryQueryRepository.GetByCountryNameAsync(request.Country);
            var country = countries.FirstOrDefault(c => Normalize(c.CountryName??" ") == normalizedCountry);

            if (country is null)
            {
                country = new Countries
                {
                    CountryName = request.Country.TrimEnd(),
                    CountryCode = request.Country.Substring(0, Math.Min(3, request.Country.Length)).ToUpper(),
                    IsActive=Core.Domain.Enums.Common.Enums.Status.Active
                };

                country = await _countryCommandRepository.CreateAsync(country);
            }

            // --- Step 2: State ---
            var normalizedState = Normalize(request.State);
            var states = await _stateQueryRepository.GetByStateNameAsync(request.State);
            var state = states.FirstOrDefault(s => Normalize(s.StateName??" ") == normalizedState 
                                                && s.CountryId == country.Id);

            if (state is null)
            {
                state = new States
                {
                    StateCode = request.State.Substring(0, Math.Min(3, request.State.Length)).ToUpper(),
                    StateName = request.State.TrimEnd(),
                    CountryId = country.Id,
                    IsActive=Core.Domain.Enums.Common.Enums.Status.Active
                };

                state = await _stateCommandRepository.CreateAsync(state);
            }

            // --- Step 3: City ---
            var normalizedCity = Normalize(request.City);
            var cities = await _cityQueryRepository.GetByCityNameAsync(request.City);
            var city = cities.FirstOrDefault(c => Normalize(c.CityName??" ") == normalizedCity 
                                                && c.StateId == state.Id);

            if (city is null)
            {
                city = new Cities
                {
                    CityCode = request.City.Substring(0, Math.Min(3, request.City.Length)).ToUpper(),
                    CityName = request.City.TrimEnd(),
                    StateId = state.Id,
                    IsActive=Core.Domain.Enums.Common.Enums.Status.Active
                };

                city = await _cityCommandRepository.CreateAsync(city);
            }

            // --- Step 4: Return response ---
            return new LocationResponse
            {
                CountryId = country.Id,
                StateId = state.Id,
                CityId = city.Id
            };
        }

    }
}