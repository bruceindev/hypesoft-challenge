using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Hypesoft.Application.Products.Queries.GetProducts;

namespace Hypesoft.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DebugController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IWebHostEnvironment _environment;

    public DebugController(IMediator mediator, IWebHostEnvironment environment)
    {
        _mediator = mediator;
        _environment = environment;
    }

    [HttpGet("performance-test")]
    public async Task<IActionResult> PerformanceTest(CancellationToken cancellationToken)
    {
        if (!_environment.IsDevelopment())
        {
            return NotFound();
        }

        var iterations = 50;
        var executionTimes = new List<long>();
        var stopwatch = new Stopwatch();

        var query = new GetProductsQuery
        {
            PageNumber = 1,
            PageSize = 10
        };

        for (int i = 0; i < iterations; i++)
        {
            stopwatch.Restart();
            await _mediator.Send(query, cancellationToken);
            stopwatch.Stop();
            executionTimes.Add(stopwatch.ElapsedMilliseconds);
        }

        var averageTime = executionTimes.Average();
        var minTime = executionTimes.Min();
        var maxTime = executionTimes.Max();

        return Ok(new
        {
            iterations = iterations,
            averageExecutionTimeMs = Math.Round(averageTime, 2),
            minExecutionTimeMs = minTime,
            maxExecutionTimeMs = maxTime,
            status = averageTime < 500 ? "PASS" : "FAIL",
            threshold = "500ms"
        });
    }
}
