using Core.Application.Common.Interfaces.ICountry;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcServices.UserManagement;

namespace UserManagement.API.GrpcServices
{
    public class CountryGrpcService : CountryService.CountryServiceBase
    {
        private readonly ICountryQueryRepository _countryQueryRepository;
        public CountryGrpcService(ICountryQueryRepository countryQueryRepository)
        {
            _countryQueryRepository = countryQueryRepository;
        }
        public override async Task<CountryListResponse> GetAllCountry(Empty request, ServerCallContext context)
        {
            var (countries, _) = await _countryQueryRepository.GetAllCountriesAsync(1, int.MaxValue, null);

            var response = new CountryListResponse();
            foreach (var country in countries)
            {
                response.Countries.Add(new CountryDto
                {
                    CountryId = country.Id,
                    CountryCode = country.CountryCode,
                    CountryName = country.CountryName                     
                });
            }
            return response;
        }
    }
}
  