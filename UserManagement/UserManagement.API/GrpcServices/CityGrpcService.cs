using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Application.Common.Interfaces.ICity;
using Core.Application.Common.Interfaces.IUnit;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcServices.UserManagement;

namespace UserManagement.API.GrpcServices
{
    public class CityGrpcService : CityService.CityServiceBase
    {
        private readonly ICityQueryRepository _cityQueryRepository;
        public CityGrpcService(ICityQueryRepository cityQueryRepository)
        {
            _cityQueryRepository = cityQueryRepository;
        }
        public override async Task<CityListReponse> GetAllCity(Empty request, ServerCallContext context)
        {
            var (cities, _) = await _cityQueryRepository.GetAllCityAsync(1, int.MaxValue, null);

            var response = new CityListReponse();
            foreach (var city in cities)
            {
                response.Cities.Add(new CityDto
                {
                    CityId = city.Id,
                    CityCode = city.CityCode,
                    CityName = city.CityName
                  
                   
                });
            }

            return response;
        }

    }
}