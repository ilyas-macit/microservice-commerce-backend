using System.Text;
using System.Text.Json;
using LogService.Application.Commands.CreateLog;
using LogService.Infrastructure.Configuration;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.Contracts.IntegrationEvents;
using Shared.Contracts.Messaging;

namespace LogService.Infrastructure.Messaging;

public class ProductCreatedIntegrationEventConsumer : BackgroundService
{
    private readonly RabbitMqSettings _settings;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ProductCreatedIntegrationEventConsumer> _logger;

    private IConnection? _connection;
    private IModel? _channel;

    public ProductCreatedIntegrationEventConsumer(
        RabbitMqSettings settings,
        IServiceScopeFactory scopeFactory,
        ILogger<ProductCreatedIntegrationEventConsumer> logger)
    {
        _settings = settings;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                EnsureBrokerObjects();

                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.Received += async (_, eventArgs) =>
                {
                    await HandleMessageAsync(eventArgs, stoppingToken);
                };

                _channel!.BasicConsume(
                    queue: ProductEventsTopology.Queues.LogProductCreated,
                    autoAck: false,
                    consumer: consumer);

                _logger.LogInformation(
                    "RabbitMQ consumer started for queue {QueueName}.",
                    ProductEventsTopology.Queues.LogProductCreated);

                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RabbitMQ consumer failed. Retrying in 5 seconds.");
                Cleanup();
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        Cleanup();
        return base.StopAsync(cancellationToken);
    }

    private void EnsureBrokerObjects()
    {
        if (_connection is { IsOpen: true } && _channel is { IsOpen: true })
        {
            return;
        }

        Cleanup();

        var factory = new ConnectionFactory
        {
            HostName = _settings.Host,
            Port = _settings.Port,
            UserName = _settings.Username,
            Password = _settings.Password,
            DispatchConsumersAsync = true,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(
            exchange: ProductEventsTopology.Exchange,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false);

        _channel.QueueDeclare(
            queue: ProductEventsTopology.Queues.LogProductCreated,
            durable: true,
            exclusive: false,
            autoDelete: false);

        _channel.QueueBind(
            queue: ProductEventsTopology.Queues.LogProductCreated,
            exchange: ProductEventsTopology.Exchange,
            routingKey: ProductEventsTopology.RoutingKeys.ProductCreated);
    }

    private async Task HandleMessageAsync(BasicDeliverEventArgs eventArgs, CancellationToken cancellationToken)
    {
        if (_channel is null)
        {
            return;
        }

        try
        {
            _logger.LogInformation(
                "Received message from queue {QueueName}, DeliveryTag={DeliveryTag}",
                ProductEventsTopology.Queues.LogProductCreated,
                eventArgs.DeliveryTag);

            var body = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            var integrationEvent = JsonSerializer.Deserialize<ProductCreatedIntegrationEvent>(body);

            if (integrationEvent is null)
            {
                _logger.LogWarning(
                    "Received invalid ProductCreatedIntegrationEvent payload. DeliveryTag={DeliveryTag}, RawPayload={RawPayload}",
                    eventArgs.DeliveryTag,
                    body);
                _channel.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: false);
                return;
            }

            _logger.LogInformation(
                "Processing ProductCreatedIntegrationEvent: ProductId={ProductId}, Name={ProductName}, DeliveryTag={DeliveryTag}",
                integrationEvent.ProductId,
                integrationEvent.Name,
                eventArgs.DeliveryTag);

            using var scope = _scopeFactory.CreateScope();
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();

            await sender.Send(new CreateLogCommand
            {
                Level = "INFO",
                Message = $"Product created: {integrationEvent.Name} (Id: {integrationEvent.ProductId})",
                ServiceName = "ProductService"
            }, cancellationToken);

            _channel.BasicAck(eventArgs.DeliveryTag, multiple: false);

            _logger.LogInformation(
                "ProductCreatedIntegrationEvent processed successfully: ProductId={ProductId}, DeliveryTag={DeliveryTag}",
                integrationEvent.ProductId,
                eventArgs.DeliveryTag);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error processing ProductCreatedIntegrationEvent message. DeliveryTag={DeliveryTag}, Queue={QueueName}",
                eventArgs.DeliveryTag,
                ProductEventsTopology.Queues.LogProductCreated);
            _channel.BasicNack(eventArgs.DeliveryTag, multiple: false, requeue: true);
        }
    }

    private void Cleanup()
    {
        try
        {
            _channel?.Close();
            _channel?.Dispose();
        }
        catch
        {
            // Best effort cleanup.
        }

        try
        {
            _connection?.Close();
            _connection?.Dispose();
        }
        catch
        {
            // Best effort cleanup.
        }

        _channel = null;
        _connection = null;
    }
}
