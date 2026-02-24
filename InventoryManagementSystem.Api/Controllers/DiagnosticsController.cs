using InventoryManagementSystem.Api.Data;
using InventoryManagementSystem.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class DiagnosticsController : ControllerBase
{
    [HttpPost("echo")]
    [ProducesResponseType(typeof(EchoRequest), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Echo([FromBody] EchoRequest request)
    {
        return Ok(request);
    }

    [HttpGet("db-connection")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> VerifyDatabaseConnection(
        [FromServices] InventoryDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);

        if (!canConnect)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { message = "Database connectivity check failed." });
        }

        return Ok(new { message = "Database connectivity check succeeded." });
    }
}
