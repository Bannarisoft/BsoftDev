using System;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Logging;
using BackgroundService.Application.Interfaces;
using BackgroundService.Application.Models;

namespace BackgroundService.Infrastructure.Services
{
    public class RabbitMqConsumer : IRabbitMqConsumer, IDisposable
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<RabbitMqConsumer> _logger;
        private readonly string _queueName = "forgot_password_notifications";
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqConsumer(INotificationService notificationService, ILogger<RabbitMqConsumer> logger)
        {
            _notificationService = notificationService;
            _logger = logger;

            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = "rabbitmq", // ✅ Ensure this matches your Docker service name
                    DispatchConsumersAsync = true, // ✅ Enable async processing
                    AutomaticRecoveryEnabled = true, // ✅ Enables automatic reconnection
                    NetworkRecoveryInterval = TimeSpan.FromSeconds(5) // ✅ Sets reconnection retry interval
                };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _logger.LogInformation("✅ RabbitMQ connection and channel successfully established.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Failed to connect to RabbitMQ: {ex.Message}");
                throw;
            }
        }

        public void StartConsuming()
        {
            try
            {
                _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
                
                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.Received += async (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        _logger.LogInformation($"📩 Received message: {message}");

                      var otpMessage = JsonSerializer.Deserialize<ForgotPasswordMessage>(message, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true, // ✅ Allows case-insensitive property matching
                            AllowTrailingCommas = true
                        });

                        if (otpMessage == null)
                        {
                            _logger.LogError("❌ Deserialization failed: Message is null or incorrectly formatted.");
                            _channel.BasicNack(ea.DeliveryTag, false, false); // Reject message
                            return;
                        }
                        // ✅ Log full object to debug missing data
                        _logger.LogInformation($"📩 Deserialized Message: Provider={otpMessage.Provider}, ToEmail={otpMessage.ToEmail}, Subject={otpMessage.Subject}, HtmlContent={otpMessage.HtmlContent}");
                        if (string.IsNullOrWhiteSpace(otpMessage.ToEmail))
                        {
                            _logger.LogError($"❌ Deserialized message has an EMPTY ToEmail! Skipping...");
                            _channel.BasicNack(ea.DeliveryTag, false, false); // Reject invalid message
                            return;
                        }
                        _logger.LogInformation($"📩 Proceeding with email to: {otpMessage.ToEmail}");
                        var provider = otpMessage.Provider ?? "Gmail"; // ✅ Ensures Provider is not null
                        var success = await _notificationService.SendEmailAsync(
                            otpMessage.ToEmail,
                            otpMessage.Subject ?? "No Subject",
                            otpMessage.HtmlContent ?? "No Content",
                            provider
                        );
                        if (!success)
                        {
                            _logger.LogError($"❌ Failed to send email to {otpMessage.ToEmail}");
                        }
                        else
                        {
                            _logger.LogInformation($"📧 Email successfully sent to {otpMessage.ToEmail}");
                        }
                        // ✅ Acknowledge message after successful processing
                        _channel.BasicAck(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"❌ Error processing message: {ex.Message}");
                        _channel.BasicNack(ea.DeliveryTag, false, true); // ❗ Requeue message if processing failed
                    }
                };
                _channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
                _logger.LogInformation("✅ RabbitMQ Consumer is listening for messages...");
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ RabbitMQ Consumer encountered an error: {ex.Message}");
            }
        }

        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
            _logger.LogInformation("🔻 RabbitMQ connection and channel closed.");
        }
    }
}
