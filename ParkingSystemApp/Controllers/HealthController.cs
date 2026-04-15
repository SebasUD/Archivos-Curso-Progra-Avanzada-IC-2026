using Microsoft.AspNetCore.Mvc;

namespace ParkingSystemApp.Controllers;

/// <summary>
/// Health check controller for API status verification
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;

    public HealthController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Simple health check endpoint that doesn't require database
    /// </summary>
    [HttpGet]
    public IActionResult GetHealth()
    {
        _logger.LogInformation("Health check requested");
        
        return Ok(new
        {
            status = "healthy",
            message = "Parking System API is running",
            timestamp = DateTime.UtcNow,
            version = "1.0.0"
        });
    }

    /// <summary>
    /// Detailed health check with environment info
    /// </summary>
    [HttpGet("detailed")]
    public IActionResult GetDetailedHealth()
    {
        _logger.LogInformation("Detailed health check requested");
        
        return Ok(new
        {
            status = "healthy",
            message = "Parking System API is running",
            environment = new
            {
                machineName = Environment.MachineName,
                osVersion = Environment.OSVersion,
                processorCount = Environment.ProcessorCount,
                dotnetVersion = Environment.Version
            },
            application = new
            {
                name = "Parking System API",
                version = "1.0.0",
                framework = ".NET 10.0"
            },
            timestamp = DateTime.UtcNow
        });
    }
}
