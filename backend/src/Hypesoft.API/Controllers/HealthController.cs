using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hypesoft.Infrastructure.Persistence;

namespace Hypesoft.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public HealthController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        try
        {
            await _context.Database.CanConnectAsync(cancellationToken);
            
            return Ok(new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow,
                database = "Connected"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(503, new
            {
                status = "Unhealthy",
                timestamp = DateTime.UtcNow,
                database = "Disconnected",
                error = ex.Message
            });
        }
    }
}
