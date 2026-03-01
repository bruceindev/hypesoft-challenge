using MediatR;
using Microsoft.AspNetCore.Mvc;
using Hypesoft.Application.Dashboard.Queries.GetDashboardStats;

namespace Hypesoft.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetDashboardStats(CancellationToken cancellationToken)
    {
        var query = new GetDashboardStatsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
