using MediatR;
using ProductService.Application.DTOs;

namespace ProductService.Application.Queries.GetProductById;

public class GetProductByIdQuery : IRequest<ProductDto?>
{
    public GetProductByIdQuery(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}
