using MediatR;
using ProductService.Application.Interfaces;
using ProductService.Domain.Interfaces;
using Shared.Contracts.IntegrationEvents;

namespace ProductService.Application.Commands.AddProduct;

public class AddProductCommandHandler : IRequestHandler<AddProductCommand, Guid>
{
    private readonly IProductRepository _productRepository;
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    private readonly ICacheService _cacheService;

    public AddProductCommandHandler(
        IProductRepository productRepository,
        IIntegrationEventPublisher integrationEventPublisher,
        ICacheService cacheService)
    {
        _productRepository = productRepository;
        _integrationEventPublisher = integrationEventPublisher;
        _cacheService = cacheService;
    }

    public async Task<Guid> Handle(AddProductCommand request, CancellationToken cancellationToken)
    {
        var product = Domain.Entities.Product.Create(
            request.Name,
            request.Description,
            request.Price,
            request.Stock);

        // Persist first; publish integration event only after successful save.
        await _productRepository.AddAsync(product);

        await _integrationEventPublisher.PublishProductCreatedAsync(
            MapToIntegrationEvent(product),
            cancellationToken);

        await _cacheService.RemoveAsync("products:all");

        return product.Id;
    }

    private static ProductCreatedIntegrationEvent MapToIntegrationEvent(Domain.Entities.Product product)
    {
        return new ProductCreatedIntegrationEvent
        {
            ProductId = product.Id,
            Name = product.Name,
            Price = product.Price,
            CreatedAtUtc = product.CreatedAt
        };
    }
}
