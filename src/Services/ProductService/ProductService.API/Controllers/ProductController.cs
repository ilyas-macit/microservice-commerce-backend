using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Commands.AddProduct;
using ProductService.Application.Commands.UpdateProduct;
using ProductService.Application.Queries.GetAllProducts;
using ProductService.Application.Queries.GetProductById;

namespace ProductService.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    private readonly ISender _sender;

    public ProductController(ISender sender)
    {
        _sender = sender;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _sender.Send(new GetAllProductsQuery());
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _sender.Send(new GetProductByIdQuery(id));
        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddProductCommand command)
    {
        var id = await _sender.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductCommand command)
    {
        command.Id = id;
        var updated = await _sender.Send(command);
        return Ok(updated);
    }
}
