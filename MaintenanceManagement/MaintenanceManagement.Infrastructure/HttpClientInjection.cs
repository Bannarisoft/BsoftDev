// using Contracts.Interfaces.External.IUser;
// using GrpcServices.Maintenance;
// using GrpcServices.UserManagement;
// using MaintenanceManagement.Infrastructure.GrpcClients;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.DependencyInjection;
// using Shared.Infrastructure.HttpClientPolly;

// namespace MaintenanceManagement.Infrastructure
// {
    // public static class HttpClientInjection
    // {
    //     public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration)
    //     {
    //         // Register gRPC Client
    //         var userManagementUrl = configuration["GrpcSettings:UserManagementUrl"];
    //         services.AddGrpcClient<DepartmentService.DepartmentServiceClient>(o =>
    //         {
    //             o.Address = new Uri(userManagementUrl); // 👈 UserManagement HTTPS URL
    //         })
    //         .ConfigurePrimaryHttpMessageHandler(() =>
    //         {
    //             // Optional: Customize the HTTP handler if needed
    //             return new HttpClientHandler
    //             {
    //                 ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true // For localhost dev certificate
    //             };
    //         })
    //         .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())            // 👈 Retry Policy
    //         .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());  // 👈 Circuit Breaker Policy
    //         services.AddScoped<IDepartmentGrpcClient, DepartmentGrpcClient>();

    //         services.AddGrpcClient<SessionService.SessionServiceClient>(o =>
    //         {
    //             o.Address = new Uri(userManagementUrl); // ➔ UserManagement HTTPS URL
    //         })
    //          .ConfigurePrimaryHttpMessageHandler(() =>
    //         {
    //             // Optional: Customize the HTTP handler if needed
    //             return new HttpClientHandler
    //             {
    //                 ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true // For localhost dev certificate
    //             };
    //         })
    //         .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())            // 👈 Retry Policy
    //         .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());  // 👈 Circuit Breaker Policy
    //         services.AddScoped<IUserSessionGrpcClient, GrpcUserSessionClient>();



    //         // DepartmentClient
    //         // services.AddHttpClient("DepartmentClient", client =>
    //         // {
    //         //     client.BaseAddress = new Uri(configuration["HttpClientSettings:DepartmentService"]);
    //         //     client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    //         // })
    //         //  .AddHttpMessageHandler<AuthTokenHandler>()
    //         // .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
    //         // .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());
    //         // services.AddScoped<IUserDepartmentService, UserDepartmentService>();

    //         // UserSessionClient
    //         // services.AddHttpClient("UserSessionClient", client =>
    //         // {
    //         //     client.BaseAddress = new Uri(configuration["HttpClientSettings:UserSessionService"]);
    //         //     client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    //         // })
    //         // // .AddHttpMessageHandler<AuthTokenHandler>()
    //         // .AddPolicyHandler(HttpClientPolicyExtensions.GetRetryPolicy())
    //         // .AddPolicyHandler(HttpClientPolicyExtensions.GetCircuitBreakerPolicy());
    //         // services.AddScoped<IUserSessionService, UserSessionService>();

    //         return services;
    //     }
    // }
// }