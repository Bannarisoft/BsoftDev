using System;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Core.Application.Common.Interfaces.INotifications;
using Serilog;

namespace UserManagement.Infrastructure.Services
{
    public class RabbitMqPublisher : IRabbitMqPublisher
    {
        private readonly string _host = "localhost";
        private readonly string _forgotPasswordQueue = "forgot_password_notifications";
        private readonly string _userCreatedQueue = "user_logged_in_queue";

        public void PublishMessage<T>(T message, bool isUserCreated = false)
        {
            // Use the correct queue based on the flag
            var queueName = isUserCreated ? _userCreatedQueue : _forgotPasswordQueue;

            var factory = new ConnectionFactory { HostName = _host };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                // Declare the appropriate queue
                channel.QueueDeclare(queue: queueName,
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);

                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

                // Publish to the correct queue
                channel.BasicPublish(exchange: "",
                                    routingKey: queueName,
                                    basicProperties: null,
                                    body: body);

                Log.Information($" [x] Sent message to RabbitMQ queue: {queueName}");
            }
        }
    }
}
