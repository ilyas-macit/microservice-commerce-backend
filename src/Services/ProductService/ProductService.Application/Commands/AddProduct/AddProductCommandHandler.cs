using MediatR;
using ProductService.Application.Interfaces;
using ProductService.Domain.Events;
using ProductService.Domain.Interfaces;

namespace ProductService.Application.Commands.AddProduct;

public class AddProductCommandHandler : IRequestHandler<AddProductCommand, Guid>
{
    private readonly IProductRepository _productRepository;
    private readonly IPublisher _publisher;
    private readonly ICacheService _cacheService;

    public AddProductCommandHandler(
        IProductRepository productRepository,
        IPublisher publisher,
        ICacheService cacheService)
    {
        _productRepository = productRepository;
        _publisher = publisher;
        _cacheService = cacheService;
    }

    public async Task<Guid> Handle(AddProductCommand request, CancellationToken cancellationToken)
    {
        var product = Domain.Entities.Product.Create(
            request.Name,
            request.Description,
            request.Price,
            request.Stock);

        await _productRepository.AddAsync(product);

        await _publisher.Publish(new ProductAddedEvent
        {
            ProductId = product.Id,
            ProductName = product.Name,
            Timestamp = DateTime.UtcNow
        }, cancellationToken);

        await _cacheService.RemoveAsync("products:all");

        return product.Id;
    }
}
