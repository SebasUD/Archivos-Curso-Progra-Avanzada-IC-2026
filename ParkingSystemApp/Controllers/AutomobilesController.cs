using Microsoft.AspNetCore.Mvc;
using ParkingSystemApp.Models;
using ParkingSystemApp.Repositories.Interfaces;

namespace ParkingSystemApp.Controllers;

/// <summary>
/// REST API Controller for Automobiles (PRQ_Automobiles table)
/// Provides endpoints for CRUD operations and filtered queries
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AutomobilesController : ControllerBase
{
    private readonly IAutomobileRepository _repository;
    private readonly ILogger<AutomobilesController> _logger;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="repository">The automobile repository instance</param>
    /// <param name="logger">The logger instance</param>
    public AutomobilesController(IAutomobileRepository repository, ILogger<AutomobilesController> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // ========================================
    // GET Endpoints
    // ========================================

    /// <summary>
    /// Get all automobiles from the database
    /// </summary>
    /// <returns>List of all automobiles</returns>
    /// <response code="200">Returns the list of automobiles</response>
    /// <response code="500">Internal server error</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<IEnumerable<Automobile>>>> GetAll()
    {
        try
        {
            _logger.LogInformation("GET /api/automobiles - Fetching all automobiles");
            var automobiles = await _repository.GetAllFromDbAsync();
            
            return Ok(new ApiResponse<IEnumerable<Automobile>>
            {
                Success = true,
                Message = $"Retrieved {automobiles.Count()} automobiles",
                Data = automobiles,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching all automobiles: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                Success = false,
                Message = $"Error: {ex.Message}",
                Data = null,
                Timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Get an automobile by ID
    /// </summary>
    /// <param name="id">The automobile ID</param>
    /// <returns>The automobile with the specified ID</returns>
    /// <response code="200">Returns the automobile</response>
    /// <response code="404">Automobile not found</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<Automobile>>> GetById(long id)
    {
        try
        {
            _logger.LogInformation($"GET /api/automobiles/{id} - Fetching automobile by ID");
            var automobile = await _repository.GetByIdAsync(id);
            
            if (automobile == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Automobile with ID {id} not found",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            return Ok(new ApiResponse<Automobile>
            {
                Success = true,
                Message = "Automobile retrieved successfully",
                Data = automobile,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching automobile by ID {id}: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                Success = false,
                Message = $"Error: {ex.Message}",
                Data = null,
                Timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Get automobiles with advanced filtering
    /// </summary>
    /// <param name="color">Filter by vehicle color (case-insensitive partial match)</param>
    /// <param name="yearStart">Filter by minimum year</param>
    /// <param name="yearEnd">Filter by maximum year</param>
    /// <param name="manufacturer">Filter by manufacturer (case-insensitive partial match)</param>
    /// <param name="type">Filter by vehicle type (case-insensitive partial match, e.g., sedan, 4x4, motorcycle)</param>
    /// <returns>Filtered list of automobiles</returns>
    /// <response code="200">Returns the filtered list of automobiles</response>
    /// <response code="400">Invalid query parameters</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("filter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<IEnumerable<Automobile>>>> Filter(
        [FromQuery] string? color = null,
        [FromQuery] int? yearStart = null,
        [FromQuery] int? yearEnd = null,
        [FromQuery] string? manufacturer = null,
        [FromQuery] string? type = null)
    {
        try
        {
            // Validate year range
            if ((yearStart.HasValue && yearStart < 1900) || (yearStart.HasValue && yearStart > 2100))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "yearStart must be between 1900 and 2100",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            if ((yearEnd.HasValue && yearEnd < 1900) || (yearEnd.HasValue && yearEnd > 2100))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "yearEnd must be between 1900 and 2100",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            if (yearStart.HasValue && yearEnd.HasValue && yearStart > yearEnd)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "yearStart cannot be greater than yearEnd",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            _logger.LogInformation($"GET /api/automobiles/filter - Applying filters: color={color}, yearStart={yearStart}, yearEnd={yearEnd}, manufacturer={manufacturer}, type={type}");
            
            var automobiles = await _repository.GetFilteredAsync(color, yearStart, yearEnd, manufacturer, type);

            return Ok(new ApiResponse<IEnumerable<Automobile>>
            {
                Success = true,
                Message = $"Retrieved {automobiles.Count()} automobiles matching the filter criteria",
                Data = automobiles,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error filtering automobiles: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                Success = false,
                Message = $"Error: {ex.Message}",
                Data = null,
                Timestamp = DateTime.UtcNow
            });
        }
    }

    // ========================================
    // POST Endpoint
    // ========================================

    /// <summary>
    /// Create a new automobile record
    /// </summary>
    /// <param name="automobile">The automobile to create</param>
    /// <returns>The created automobile with generated ID</returns>
    /// <response code="201">Automobile created successfully</response>
    /// <response code="400">Invalid automobile data</response>
    /// <response code="500">Internal server error</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<Automobile>>> Create([FromBody] CreateAutomobileDto dto)
    {
        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(dto.Color))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Color is required",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            if (dto.Year < 1900 || dto.Year > 2100)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Year must be between 1900 and 2100",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            if (string.IsNullOrWhiteSpace(dto.Manufacturer))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Manufacturer is required",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            if (string.IsNullOrWhiteSpace(dto.Type))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Type is required",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            var validTypes = new[] { "sedan", "4x4", "motorcycle" };
            if (!validTypes.Contains(dto.Type.ToLower()))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Type must be one of: sedan, 4x4, motorcycle",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            var automobile = new Automobile
            {
                Color = dto.Color,
                Year = dto.Year,
                Manufacturer = dto.Manufacturer,
                Type = dto.Type
            };

            _logger.LogInformation($"POST /api/automobiles - Creating new automobile: {dto.Manufacturer} {dto.Color} {dto.Year}");
            var id = await _repository.InsertAsync(automobile);
            automobile.Id = id;

            return CreatedAtAction(nameof(GetById), new { id }, new ApiResponse<Automobile>
            {
                Success = true,
                Message = "Automobile created successfully",
                Data = automobile,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating automobile: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                Success = false,
                Message = $"Error: {ex.Message}",
                Data = null,
                Timestamp = DateTime.UtcNow
            });
        }
    }

    // ========================================
    // PUT Endpoint
    // ========================================

    /// <summary>
    /// Update an existing automobile record
    /// </summary>
    /// <param name="id">The automobile ID to update</param>
    /// <param name="dto">The updated automobile data</param>
    /// <returns>The updated automobile</returns>
    /// <response code="200">Automobile updated successfully</response>
    /// <response code="400">Invalid automobile data</response>
    /// <response code="404">Automobile not found</response>
    /// <response code="500">Internal server error</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<Automobile>>> Update(long id, [FromBody] UpdateAutomobileDto dto)
    {
        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(dto.Color))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Color is required",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            if (dto.Year < 1900 || dto.Year > 2100)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Year must be between 1900 and 2100",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            if (string.IsNullOrWhiteSpace(dto.Manufacturer))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Manufacturer is required",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            if (string.IsNullOrWhiteSpace(dto.Type))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Type is required",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            var validTypes = new[] { "sedan", "4x4", "motorcycle" };
            if (!validTypes.Contains(dto.Type.ToLower()))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Type must be one of: sedan, 4x4, motorcycle",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            // Check if automobile exists
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Automobile with ID {id} not found",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            var automobile = new Automobile
            {
                Id = id,
                Color = dto.Color,
                Year = dto.Year,
                Manufacturer = dto.Manufacturer,
                Type = dto.Type
            };

            _logger.LogInformation($"PUT /api/automobiles/{id} - Updating automobile");
            var result = await _repository.UpdateAsync(automobile);

            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Failed to update automobile with ID {id}",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            // Fetch the updated automobile
            var updated = await _repository.GetByIdAsync(id);

            return Ok(new ApiResponse<Automobile>
            {
                Success = true,
                Message = "Automobile updated successfully",
                Data = updated,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating automobile with ID {id}: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                Success = false,
                Message = $"Error: {ex.Message}",
                Data = null,
                Timestamp = DateTime.UtcNow
            });
        }
    }

    // ========================================
    // DELETE Endpoint
    // ========================================

    /// <summary>
    /// Delete an automobile record
    /// </summary>
    /// <param name="id">The automobile ID to delete</param>
    /// <returns>Deletion confirmation</returns>
    /// <response code="200">Automobile deleted successfully</response>
    /// <response code="404">Automobile not found</response>
    /// <response code="500">Internal server error</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(long id)
    {
        try
        {
            _logger.LogInformation($"DELETE /api/automobiles/{id} - Deleting automobile");
            var result = await _repository.DeleteAsync(id);

            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Automobile with ID {id} not found",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = $"Automobile with ID {id} deleted successfully",
                Data = null,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting automobile with ID {id}: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse<object>
            {
                Success = false,
                Message = $"Error: {ex.Message}",
                Data = null,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
