using Microsoft.AspNetCore.Mvc;
using ParkingSystemApp.Models;
using ParkingSystemApp.Repositories.Interfaces;

namespace ParkingSystemApp.Controllers;

/// <summary>
/// REST API Controller for Parking Lots (PRQ_Parking table)
/// Provides endpoints for CRUD operations and filtered queries
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ParkingsController : ControllerBase
{
    private readonly IParkingRepository _repository;
    private readonly ILogger<ParkingsController> _logger;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="repository">The parking repository instance</param>
    /// <param name="logger">The logger instance</param>
    public ParkingsController(IParkingRepository repository, ILogger<ParkingsController> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // ========================================
    // GET Endpoints
    // ========================================

    /// <summary>
    /// Get all parking lots from the database
    /// </summary>
    /// <returns>List of all parking lots</returns>
    /// <response code="200">Returns the list of parking lots</response>
    /// <response code="500">Internal server error</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<IEnumerable<Parking>>>> GetAll()
    {
        try
        {
            _logger.LogInformation("GET /api/parkings - Fetching all parking lots");
            var parkings = await _repository.GetAllFromDbAsync();
            
            return Ok(new ApiResponse<IEnumerable<Parking>>
            {
                Success = true,
                Message = $"Retrieved {parkings.Count()} parking lots",
                Data = parkings,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching all parking lots: {ex.Message}");
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
    /// Get a parking lot by ID
    /// </summary>
    /// <param name="id">The parking lot ID</param>
    /// <returns>The parking lot with the specified ID</returns>
    /// <response code="200">Returns the parking lot</response>
    /// <response code="404">Parking lot not found</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<Parking>>> GetById(long id)
    {
        try
        {
            _logger.LogInformation($"GET /api/parkings/{id} - Fetching parking lot by ID");
            var parking = await _repository.GetByIdAsync(id);
            
            if (parking == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Parking lot with ID {id} not found",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            return Ok(new ApiResponse<Parking>
            {
                Success = true,
                Message = "Parking lot retrieved successfully",
                Data = parking,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching parking lot by ID {id}: {ex.Message}");
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
    /// Get parking lots with advanced filtering
    /// </summary>
    /// <param name="province">Filter by province name (case-insensitive partial match)</param>
    /// <param name="name">Filter by parking name (case-insensitive partial match)</param>
    /// <param name="priceMin">Filter by minimum price per hour</param>
    /// <param name="priceMax">Filter by maximum price per hour</param>
    /// <returns>Filtered list of parking lots</returns>
    /// <response code="200">Returns the filtered list of parking lots</response>
    /// <response code="400">Invalid query parameters</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("filter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<IEnumerable<Parking>>>> Filter(
        [FromQuery] string? province = null,
        [FromQuery] string? name = null,
        [FromQuery] decimal? priceMin = null,
        [FromQuery] decimal? priceMax = null)
    {
        try
        {
            // Validate price range
            if ((priceMin.HasValue && priceMin < 0) || (priceMax.HasValue && priceMax < 0))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Price values cannot be negative",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            if (priceMin.HasValue && priceMax.HasValue && priceMin > priceMax)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "priceMin cannot be greater than priceMax",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            _logger.LogInformation($"GET /api/parkings/filter - Applying filters: province={province}, name={name}, priceMin={priceMin}, priceMax={priceMax}");
            
            var parkings = await _repository.GetFilteredAsync(province, name, priceMin, priceMax);

            return Ok(new ApiResponse<IEnumerable<Parking>>
            {
                Success = true,
                Message = $"Retrieved {parkings.Count()} parking lots matching the filter criteria",
                Data = parkings,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error filtering parking lots: {ex.Message}");
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
    /// Create a new parking lot record
    /// </summary>
    /// <param name="dto">The parking lot to create</param>
    /// <returns>The created parking lot with generated ID</returns>
    /// <response code="201">Parking lot created successfully</response>
    /// <response code="400">Invalid parking lot data</response>
    /// <response code="500">Internal server error</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<Parking>>> Create([FromBody] CreateParkingDto dto)
    {
        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(dto.ProvinceName))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Province name is required",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            if (dto.ProvinceName.Length > 100)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Province name cannot exceed 100 characters",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            if (string.IsNullOrWhiteSpace(dto.ParkingName))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Parking name is required",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            if (dto.ParkingName.Length > 150)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Parking name cannot exceed 150 characters",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            if (dto.PricePerHour <= 0)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Price per hour must be greater than 0",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            var parking = new Parking
            {
                ProvinceName = dto.ProvinceName,
                ParkingName = dto.ParkingName,
                PricePerHour = dto.PricePerHour
            };

            _logger.LogInformation($"POST /api/parkings - Creating new parking lot: {dto.ParkingName} in {dto.ProvinceName}");
            var id = await _repository.InsertAsync(parking);
            parking.Id = id;

            return CreatedAtAction(nameof(GetById), new { id }, new ApiResponse<Parking>
            {
                Success = true,
                Message = "Parking lot created successfully",
                Data = parking,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating parking lot: {ex.Message}");
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
    /// Update an existing parking lot record
    /// </summary>
    /// <param name="id">The parking lot ID to update</param>
    /// <param name="dto">The updated parking lot data</param>
    /// <returns>The updated parking lot</returns>
    /// <response code="200">Parking lot updated successfully</response>
    /// <response code="400">Invalid parking lot data</response>
    /// <response code="404">Parking lot not found</response>
    /// <response code="500">Internal server error</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<Parking>>> Update(long id, [FromBody] UpdateParkingDto dto)
    {
        try
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(dto.ProvinceName))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Province name is required",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            if (dto.ProvinceName.Length > 100)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Province name cannot exceed 100 characters",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            if (string.IsNullOrWhiteSpace(dto.ParkingName))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Parking name is required",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            if (dto.ParkingName.Length > 150)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Parking name cannot exceed 150 characters",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            if (dto.PricePerHour <= 0)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Price per hour must be greater than 0",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            // Check if parking lot exists
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Parking lot with ID {id} not found",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            var parking = new Parking
            {
                Id = id,
                ProvinceName = dto.ProvinceName,
                ParkingName = dto.ParkingName,
                PricePerHour = dto.PricePerHour
            };

            _logger.LogInformation($"PUT /api/parkings/{id} - Updating parking lot");
            var result = await _repository.UpdateAsync(parking);

            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Failed to update parking lot with ID {id}",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            // Fetch the updated parking lot
            var updated = await _repository.GetByIdAsync(id);

            return Ok(new ApiResponse<Parking>
            {
                Success = true,
                Message = "Parking lot updated successfully",
                Data = updated,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating parking lot with ID {id}: {ex.Message}");
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
    /// Delete a parking lot record
    /// </summary>
    /// <param name="id">The parking lot ID to delete</param>
    /// <returns>Deletion confirmation</returns>
    /// <response code="200">Parking lot deleted successfully</response>
    /// <response code="404">Parking lot not found</response>
    /// <response code="500">Internal server error</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(long id)
    {
        try
        {
            _logger.LogInformation($"DELETE /api/parkings/{id} - Deleting parking lot");
            var result = await _repository.DeleteAsync(id);

            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Parking lot with ID {id} not found",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = $"Parking lot with ID {id} deleted successfully",
                Data = null,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting parking lot with ID {id}: {ex.Message}");
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
