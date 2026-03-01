using MediatR;
using Microsoft.AspNetCore.Mvc;
using Hypesoft.Application.Products.Commands.CreateProduct;
using Hypesoft.Application.Products.Commands.UpdateProduct;
using Hypesoft.Application.Products.Commands.DeleteProduct;
using Hypesoft.Application.Products.Commands.UpdateProductStock;
using Hypesoft.Application.Products.Queries.GetProducts;
using Hypesoft.Application.Products.Queries.GetProductById;
using Hypesoft.Application.Products.Queries.GetLowStockProducts;
using Hypesoft.Application.Products.Queries.GetTotalStockValue;
using Hypesoft.Application.Products.Queries.GetTotalProductsCount;

namespace Hypesoft.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct(
        [FromBody] CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetProductById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] Guid? categoryId = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetProductsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm,
            CategoryId = categoryId
        };

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetProductByIdQuery { Id = id };
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(
        Guid id,
        [FromBody] UpdateProductCommand command,
        CancellationToken cancellationToken)
    {
        command.Id = id;
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteProductCommand { Id = id };
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id}/stock")]
    public async Task<IActionResult> UpdateProductStock(
        Guid id,
        [FromBody] UpdateProductStockCommand command,
        CancellationToken cancellationToken)
    {
        command.Id = id;
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStockProducts(
        [FromQuery] int threshold = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetLowStockProductsQuery { Threshold = threshold };
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("total-stock-value")]
    public async Task<IActionResult> GetTotalStockValue(CancellationToken cancellationToken)
    {
        var query = new GetTotalStockValueQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(new { totalStockValue = result });
    }

    [HttpGet("count")]
    public async Task<IActionResult> GetTotalProductsCount(CancellationToken cancellationToken)
    {
        var query = new GetTotalProductsCountQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(new { totalProducts = result });
    }
}
