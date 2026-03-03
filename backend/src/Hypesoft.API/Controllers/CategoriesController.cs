using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Hypesoft.Application.Categories.Commands.CreateCategory;
using Hypesoft.Application.Categories.Commands.UpdateCategory;
using Hypesoft.Application.Categories.Commands.DeleteCategory;
using Hypesoft.Application.Categories.Queries.GetCategories;
using Hypesoft.Application.Categories.Queries.GetCategoryById;

namespace Hypesoft.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "User")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(Policy = "Manager")]
    public async Task<IActionResult> CreateCategory(
        [FromBody] CreateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetCategoryById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
    {
        var query = new GetCategoriesQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById(
        string id,
        CancellationToken cancellationToken)
    {
        var query = new GetCategoryByIdQuery { Id = id };
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "Manager")]
    public async Task<IActionResult> UpdateCategory(
        string id,
        [FromBody] UpdateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        command.Id = id;
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> DeleteCategory(
        string id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteCategoryCommand { Id = id };
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
