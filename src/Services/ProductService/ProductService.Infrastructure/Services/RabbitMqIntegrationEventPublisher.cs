using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using ProductService.Application.Interfaces;
using ProductService.Infrastructure.Configuration;
using RabbitMQ.Client;
using Shared.Contracts.IntegrationEvents;
using Shared.Contracts.Messaging;

namespace ProductService.Infrastructure.Services;

public class RabbitMqIntegrationEventPublisher : IIntegrationEventPublisher
{
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
            _logger.LogInformation(
                "Publishing ProductCreatedIntegrationEvent: ProductId={ProductId}, Name={ProductName}, Exchange={Exchange}, RoutingKey={RoutingKey}",
                integrationEvent.ProductId,
                integrationEvent.Name,
                ProductEventsTopology.Exchange,
                ProductEventsTopology.RoutingKeys.ProductCreated);

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
                exchange: ProductEventsTopology.Exchange,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false);

            channel.QueueDeclare(
                queue: ProductEventsTopology.Queues.LogProductCreated,
                durable: true,
                exclusive: false,
                autoDelete: false);

            channel.QueueBind(
                queue: ProductEventsTopology.Queues.LogProductCreated,
                exchange: ProductEventsTopology.Exchange,
                routingKey: ProductEventsTopology.RoutingKeys.ProductCreated);

            var payload = JsonSerializer.Serialize(integrationEvent);
            var body = Encoding.UTF8.GetBytes(payload);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";

            channel.BasicPublish(
                exchange: ProductEventsTopology.Exchange,
                routingKey: ProductEventsTopology.RoutingKeys.ProductCreated,
                basicProperties: properties,
                body: body);

            _logger.LogInformation(
                "ProductCreatedIntegrationEvent published successfully: ProductId={ProductId}, Queue={Queue}",
                integrationEvent.ProductId,
                ProductEventsTopology.Queues.LogProductCreated);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to publish ProductCreatedIntegrationEvent to RabbitMQ: ProductId={ProductId}, Name={ProductName}, Exchange={Exchange}, RoutingKey={RoutingKey}",
                integrationEvent.ProductId,
                integrationEvent.Name,
                ProductEventsTopology.Exchange,
                ProductEventsTopology.RoutingKeys.ProductCreated);
            throw;
        }

        return Task.CompletedTask;
    }
}
