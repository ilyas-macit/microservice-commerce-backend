using MediatR;
using ProductService.Application.Interfaces;
using ProductService.Domain.Interfaces;

namespace ProductService.Application.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
{
    private readonly IProductRepository _productRepository;
    private readonly ICacheService _cacheService;

    public UpdateProductCommandHandler(IProductRepository productRepository, ICacheService cacheService)
    {
        _productRepository = productRepository;
        _cacheService = cacheService;
    }

    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id);
        if (product is null)
        {
            throw new KeyNotFoundException($"Product with id {request.Id} not found");
        }

        product.Update(request.Name, request.Description, request.Price, request.Stock);

        await _productRepository.UpdateAsync(product);
        await _cacheService.RemoveAsync("products:all");
        await _cacheService.RemoveAsync($"products:{request.Id}");

        return true;
    }
}
