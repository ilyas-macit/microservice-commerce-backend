using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using ProductService.Application.Interfaces;
using ProductService.Infrastructure.Configuration;
using RabbitMQ.Client;
using Shared.Contracts.IntegrationEvents;

namespace ProductService.Infrastructure.Services;

public class RabbitMqIntegrationEventPublisher : IIntegrationEventPublisher
{
    private const string ExchangeName = "product.events";
    private const string QueueName = "log.product.created";
    private const string RoutingKey = "product.created";

    private readonly RabbitMqSettings _settings;
    private readonly ILogger<RabbitMqIntegrationEventPublisher> _logger;

    public RabbitMqIntegrationEventPublisher(
        RabbitMqSettings settings,
        ILogger<RabbitMqIntegrationEventPublisher> logger)
    {
        _settings = settings;
        _logger = logger;
    }

    public Task PublishProductCreatedAsync(ProductCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.Host,
                Port = _settings.Port,
                UserName = _settings.Username,
                Password = _settings.Password
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(
                exchange: ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false);

            channel.QueueDeclare(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false);

            channel.QueueBind(
                queue: QueueName,
                exchange: ExchangeName,
                routingKey: RoutingKey);

            var payload = JsonSerializer.Serialize(integrationEvent);
            var body = Encoding.UTF8.GetBytes(payload);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";

            channel.BasicPublish(
                exchange: ExchangeName,
                routingKey: RoutingKey,
                basicProperties: properties,
                body: body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish ProductCreatedIntegrationEvent to RabbitMQ.");
        }

        return Task.CompletedTask;
    }
}
