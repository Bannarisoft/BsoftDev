using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Contracts.Commands.Users;
using MassTransit;
using Serilog;

namespace SagaOrchestrator.Infrastructure.Consumers
{
    public class DeleteUserCommandConsumer : IConsumer<DeleteUserCommand>
    {
          private readonly IHttpClientFactory _httpClientFactory;

        public DeleteUserCommandConsumer(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task Consume(ConsumeContext<DeleteUserCommand> context)
        {
            var client = _httpClientFactory.CreateClient("UserService");

            var content = new StringContent(
                JsonSerializer.Serialize(context.Message),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync("api/User/rollback-delete", content);

            if (response.IsSuccessStatusCode)
            {
                Log.Information("✅ [SagaOrchestrator] Rollback successful for UserId: {UserId}", context.Message.UserId);
            }
            else
            {
                Log.Error("❌ [SagaOrchestrator] Rollback FAILED for UserId: {UserId}, StatusCode: {StatusCode}",
                    context.Message.UserId, response.StatusCode);
            }
        }
    }
}