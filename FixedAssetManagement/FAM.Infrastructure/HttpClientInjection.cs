
using Contracts.Interfaces.External.IUser;
using FAM.Infrastructure.GrpcClients;
using GrpcServices.UserManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            .AddGrpcPolicies();        

            services.AddScoped<IDepartmentGrpcClient, DepartmentGrpcClient>();
            // ✅ Register Session gRPC Client
            services.AddGrpcClient<SessionService.SessionServiceClient>(options =>
            {
                options.Address = new Uri(userManagementUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() => GrpcHttpHandler)
            .AddGrpcPolicies();
         
            services.AddScoped<IUserSessionGrpcClient, GrpcUserSessionClient>();
            
               // ✅ Register Session gRPC Client
            services.AddGrpcClient<UnitService.UnitServiceClient>(options =>
            {
                options.Address = new Uri(userManagementUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() => GrpcHttpHandler)
            .AddGrpcPolicies();            
            services.AddScoped<IUnitGrpcClient, UnitGrpcClient>();

            return services;
        }
    }
}
