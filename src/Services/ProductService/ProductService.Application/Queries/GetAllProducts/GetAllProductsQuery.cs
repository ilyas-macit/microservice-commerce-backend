using MediatR;
using ProductService.Application.DTOs;

namespace ProductService.Application.Queries.GetAllProducts;

public class GetAllProductsQuery : IRequest<IEnumerable<ProductDto>>
{
}
