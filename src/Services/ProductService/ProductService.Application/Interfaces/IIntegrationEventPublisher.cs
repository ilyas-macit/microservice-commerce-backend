using Shared.Contracts.IntegrationEvents;

namespace ProductService.Application.Interfaces;

public interface IIntegrationEventPublisher
{
    Task PublishProductCreatedAsync(ProductCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken = default);
}
