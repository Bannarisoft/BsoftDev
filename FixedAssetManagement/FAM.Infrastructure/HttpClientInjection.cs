
using Contracts.Interfaces.External.IUser;
using FAM.Infrastructure.GrpcClients;
using GrpcServices.UserManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Infrastructure.HttpClientPolly;

namespace FAM.Infrastructure
{
    public static class HttpClientInjection
    {
        private static readonly HttpClientHandler GrpcHttpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
        public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            var userManagementUrl = configuration["GrpcSettings:UserManagementUrl"];

            // ✅ Register Department gRPC Client
            services.AddGrpcClient<DepartmentService.DepartmentServiceClient>(options =>
            {
                options.Address = new Uri(userManagementUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() => GrpcHttpHandler)
            .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());

            services.AddScoped<IDepartmentGrpcClient, DepartmentGrpcClient>();
            // ✅ Register Session gRPC Client
            services.AddGrpcClient<SessionService.SessionServiceClient>(options =>
            {
                options.Address = new Uri(userManagementUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() => GrpcHttpHandler)
            .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());

            services.AddScoped<IUserSessionGrpcClient, GrpcUserSessionClient>();

            // ✅ Register Unit gRPC Client
            services.AddGrpcClient<UnitService.UnitServiceClient>(options =>
            {
                options.Address = new Uri(userManagementUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() => GrpcHttpHandler)
            .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());
            services.AddScoped<IUnitGrpcClient, UnitGrpcClient>();

            // ✅ Register Company gRPC Client
            services.AddGrpcClient<CompanyService.CompanyServiceClient>(options =>
            {
                options.Address = new Uri(userManagementUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            })
            .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());

            services.AddScoped<ICompanyGrpcClient, CompanyGrpcClient>();

            // ✅ Register AllDepartment gRPC Client
            services.AddGrpcClient<DepartmentAllService.DepartmentAllServiceClient>(options =>
            {
                options.Address = new Uri(userManagementUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            })
            .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());

            services.AddScoped<IDepartmentAllGrpcClient, DepartmentAllGrpcClient>();
             // ✅ Register City gRPC Client
            services.AddGrpcClient<CityService.CityServiceClient>(options =>
            {
                options.Address = new Uri(userManagementUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            })
            .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());

            services.AddScoped<ICityGrpcClient,CityGrpcClient>();
        // ✅ Register State gRPC Client
              services.AddGrpcClient<StateService.StateServiceClient>(options =>
            {
                options.Address = new Uri(userManagementUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            })
            .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());

            services.AddScoped<IStatesGrpcClient,StateGrpcClient>();

             // ✅ Register Country gRPC Client
              services.AddGrpcClient<CountryService.CountryServiceClient>(options =>
            {
                options.Address = new Uri(userManagementUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            })
            .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
            .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());

            services.AddScoped<ICountryGrpcClient,CountryGrpcClient>();

            return services;
        }
    }
}
