using Microsoft.AspNetCore.Mvc;
using ParkingSystemApp.Models;
using ParkingSystemApp.Repositories.Interfaces;

namespace ParkingSystemApp.Controllers;

/// <summary>
/// REST API Controller for Car Entries (PRQ_CarEntry table)
/// Provides endpoints for CRUD operations and filtered queries with computed fields
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CarEntriesController : ControllerBase
{
    private readonly ICarEntryRepository _repository;
    private readonly IParkingRepository _parkingRepository;
    private readonly ILogger<CarEntriesController> _logger;

    /// <summary>
    /// Constructor for dependency injection
    /// </summary>
    /// <param name="repository">The car entry repository instance</param>
    /// <param name="parkingRepository">The parking repository instance (for pricing calculations)</param>
    /// <param name="logger">The logger instance</param>
    public CarEntriesController(
        ICarEntryRepository repository, 
        IParkingRepository parkingRepository,
        ILogger<CarEntriesController> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _parkingRepository = parkingRepository ?? throw new ArgumentNullException(nameof(parkingRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // ========================================
    // Helper Methods
    // ========================================

    /// <summary>
    /// Maps a CarEntry entity to a CarEntryResponseDto with computed fields
    /// </summary>
    private CarEntryResponseDto MapToResponseDto(CarEntry carEntry)
    {
        return new CarEntryResponseDto
        {
            Consecutive = carEntry.Consecutive,
            ParkingId = carEntry.ParkingId,
            AutomobileId = carEntry.AutomobileId,
            EntryDateTime = carEntry.EntryDateTime,
            ExitDateTime = carEntry.ExitDateTime,
            StayMinutes = carEntry.StayDurationMinutes,
            StayHours = carEntry.StayDurationHours,
            TotalAmount = carEntry.TotalAmountToPay,
            CreatedAt = carEntry.CreatedAt,
            UpdatedAt = carEntry.UpdatedAt
        };
    }

    // ========================================
    // GET Endpoints
    // ========================================

    /// <summary>
    /// Get all car entries from the database with computed fields
    /// </summary>
    /// <returns>List of all car entries with computed fields</returns>
    /// <response code="200">Returns the list of car entries</response>
    /// <response code="500">Internal server error</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<IEnumerable<CarEntryResponseDto>>>> GetAll()
    {
        try
        {
            _logger.LogInformation("GET /api/carEntries - Fetching all car entries");
            var carEntries = await _repository.GetAllFromDbAsync();
            var responseDtos = carEntries.Select(MapToResponseDto).ToList();
            
            return Ok(new ApiResponse<IEnumerable<CarEntryResponseDto>>
            {
                Success = true,
                Message = $"Retrieved {responseDtos.Count} car entries",
                Data = responseDtos,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching all car entries: {ex.Message}");
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
    /// Get a car entry by consecutive ID with computed fields
    /// </summary>
    /// <param name="consecutive">The car entry consecutive ID</param>
    /// <returns>The car entry with computed fields</returns>
    /// <response code="200">Returns the car entry</response>
    /// <response code="404">Car entry not found</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{consecutive}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<CarEntryResponseDto>>> GetByConsecutive(long consecutive)
    {
        try
        {
            _logger.LogInformation($"GET /api/carEntries/{consecutive} - Fetching car entry by consecutive ID");
            var carEntry = await _repository.GetByIdAsync(consecutive);
            
            if (carEntry == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Car entry with consecutive ID {consecutive} not found",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            var responseDto = MapToResponseDto(carEntry);

            return Ok(new ApiResponse<CarEntryResponseDto>
            {
                Success = true,
                Message = "Car entry retrieved successfully",
                Data = responseDto,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching car entry by consecutive ID {consecutive}: {ex.Message}");
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
    /// Get all car entries for a specific automobile with computed fields.
    /// </summary>
    /// <param name="automobileId">The automobile ID</param>
    /// <returns>List of parking entries for the specified automobile</returns>
    /// <response code="200">Returns the list of car entries</response>
    /// <response code="400">Invalid automobile ID</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("byAutomobile/{automobileId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<IEnumerable<CarEntryResponseDto>>>> GetByAutomobile(long automobileId)
    {
        try
        {
            if (automobileId <= 0)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "automobileId must be greater than 0",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            _logger.LogInformation($"GET /api/carEntries/byAutomobile/{automobileId} - Fetching entries for automobile");
            var carEntries = await _repository.GetByAutomobileAsync(automobileId);
            var responseDtos = carEntries.Select(MapToResponseDto).ToList();

            return Ok(new ApiResponse<IEnumerable<CarEntryResponseDto>>
            {
                Success = true,
                Message = $"Retrieved {responseDtos.Count} parking entries for automobile {automobileId}",
                Data = responseDtos,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching car entries for automobile {automobileId}: {ex.Message}");
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
    /// Get car entries with advanced filtering by vehicle type and date range,
    /// or by province and date range
    /// </summary>
    /// <param name="type">Filter by vehicle type (sedan, 4x4, motorcycle)</param>
    /// <param name="province">Filter by province name (case-insensitive partial match)</param>
    /// <param name="dateStart">Start date for the filter range (inclusive)</param>
    /// <param name="dateEnd">End date for the filter range (inclusive)</param>
    /// <returns>Filtered list of car entries with computed fields</returns>
    /// <response code="200">Returns the filtered list of car entries</response>
    /// <response code="400">Invalid query parameters</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("filter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<IEnumerable<CarEntryResponseDto>>>> Filter(
        [FromQuery] string? type = null,
        [FromQuery] string? province = null,
        [FromQuery] DateTime? dateStart = null,
        [FromQuery] DateTime? dateEnd = null)
    {
        try
        {
            // Validate date range
            if (dateStart.HasValue && dateEnd.HasValue && dateStart > dateEnd)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "dateStart cannot be greater than dateEnd",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            IEnumerable<CarEntry> carEntries;
            string filterDescription;

            // Determine which filter to apply
            if (!string.IsNullOrWhiteSpace(type) && string.IsNullOrWhiteSpace(province))
            {
                // Filter by vehicle type and date range
                _logger.LogInformation($"GET /api/carEntries/filter - Applying type filter: type={type}, dateStart={dateStart}, dateEnd={dateEnd}");
                carEntries = await _repository.GetByVehicleTypeAndDateRangeAsync(type, dateStart, dateEnd);
                filterDescription = $"vehicle type '{type}'";
            }
            else if (!string.IsNullOrWhiteSpace(province) && string.IsNullOrWhiteSpace(type))
            {
                // Filter by province and date range
                _logger.LogInformation($"GET /api/carEntries/filter - Applying province filter: province={province}, dateStart={dateStart}, dateEnd={dateEnd}");
                carEntries = await _repository.GetByProvinceAndDateRangeAsync(province, dateStart, dateEnd);
                filterDescription = $"province '{province}'";
            }
            else if (!string.IsNullOrWhiteSpace(type) && !string.IsNullOrWhiteSpace(province))
            {
                // Both provided - error
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Provide either 'type' or 'province', not both",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }
            else
            {
                // Neither provided - error
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Provide either 'type' (vehicle type filter) or 'province' (province filter)",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            var responseDtos = carEntries.Select(MapToResponseDto).ToList();

            string dateRangeText = "";
            if (dateStart.HasValue && dateEnd.HasValue)
                dateRangeText = $" from {dateStart:yyyy-MM-dd} to {dateEnd:yyyy-MM-dd}";
            else if (dateStart.HasValue)
                dateRangeText = $" from {dateStart:yyyy-MM-dd}";
            else if (dateEnd.HasValue)
                dateRangeText = $" until {dateEnd:yyyy-MM-dd}";

            return Ok(new ApiResponse<IEnumerable<CarEntryResponseDto>>
            {
                Success = true,
                Message = $"Retrieved {responseDtos.Count} car entries for {filterDescription}{dateRangeText}",
                Data = responseDtos,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error filtering car entries: {ex.Message}");
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
    /// Create a new car entry record
    /// </summary>
    /// <param name="dto">The car entry to create</param>
    /// <returns>The created car entry with generated consecutive ID</returns>
    /// <response code="201">Car entry created successfully</response>
    /// <response code="400">Invalid car entry data</response>
    /// <response code="500">Internal server error</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<CarEntryResponseDto>>> Create([FromBody] CreateCarEntryDto dto)
    {
        try
        {
            // Validate input
            if (dto.ParkingId <= 0)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "ParkingId must be greater than 0",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            if (dto.AutomobileId <= 0)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "AutomobileId must be greater than 0",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            // Validate that the parking lot and automobile exist
            var parking = await _parkingRepository.GetByIdAsync(dto.ParkingId);
            if (parking == null)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Parking lot with ID {dto.ParkingId} not found",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            if (dto.ExitDateTime.HasValue && dto.ExitDateTime.Value <= dto.EntryDateTime)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "ExitDateTime must be after EntryDateTime",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            var carEntry = new CarEntry
            {
                ParkingId = dto.ParkingId,
                AutomobileId = dto.AutomobileId,
                EntryDateTime = dto.EntryDateTime,
                ExitDateTime = dto.ExitDateTime
            };

            _logger.LogInformation($"POST /api/carEntries - Creating new car entry: ParkingId={dto.ParkingId}, AutomobileId={dto.AutomobileId}");
            var consecutiveId = await _repository.InsertAsync(carEntry);
            carEntry.Consecutive = consecutiveId;

            // Reload to get the Parking and Automobile data for computed fields
            carEntry = await _repository.GetByIdAsync(consecutiveId);
            var responseDto = MapToResponseDto(carEntry!);

            return CreatedAtAction(nameof(GetByConsecutive), new { consecutive = consecutiveId }, new ApiResponse<CarEntryResponseDto>
            {
                Success = true,
                Message = "Car entry created successfully",
                Data = responseDto,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating car entry: {ex.Message}");
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
    /// Update an existing car entry record
    /// </summary>
    /// <param name="consecutive">The car entry consecutive ID to update</param>
    /// <param name="dto">The updated car entry data</param>
    /// <returns>The updated car entry with computed fields</returns>
    /// <response code="200">Car entry updated successfully</response>
    /// <response code="400">Invalid car entry data</response>
    /// <response code="404">Car entry not found</response>
    /// <response code="500">Internal server error</response>
    [HttpPut("{consecutive}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<CarEntryResponseDto>>> Update(long consecutive, [FromBody] UpdateCarEntryDto dto)
    {
        try
        {
            // Validate input
            if (dto.ParkingId <= 0)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "ParkingId must be greater than 0",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            if (dto.AutomobileId <= 0)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "AutomobileId must be greater than 0",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            if (dto.ExitDateTime.HasValue && dto.ExitDateTime.Value <= dto.EntryDateTime)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "ExitDateTime must be after EntryDateTime",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            // Check if car entry exists
            var existing = await _repository.GetByIdAsync(consecutive);
            if (existing == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Car entry with consecutive ID {consecutive} not found",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            var carEntry = new CarEntry
            {
                Consecutive = consecutive,
                ParkingId = dto.ParkingId,
                AutomobileId = dto.AutomobileId,
                EntryDateTime = dto.EntryDateTime,
                ExitDateTime = dto.ExitDateTime
            };

            _logger.LogInformation($"PUT /api/carEntries/{consecutive} - Updating car entry");
            var result = await _repository.UpdateAsync(carEntry);

            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Failed to update car entry with consecutive ID {consecutive}",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            // Fetch the updated car entry
            var updated = await _repository.GetByIdAsync(consecutive);
            var responseDto = MapToResponseDto(updated!);

            return Ok(new ApiResponse<CarEntryResponseDto>
            {
                Success = true,
                Message = "Car entry updated successfully",
                Data = responseDto,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating car entry with consecutive ID {consecutive}: {ex.Message}");
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
    /// Delete a car entry record
    /// </summary>
    /// <param name="consecutive">The car entry consecutive ID to delete</param>
    /// <returns>Deletion confirmation</returns>
    /// <response code="200">Car entry deleted successfully</response>
    /// <response code="404">Car entry not found</response>
    /// <response code="500">Internal server error</response>
    [HttpDelete("{consecutive}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(long consecutive)
    {
        try
        {
            _logger.LogInformation($"DELETE /api/carEntries/{consecutive} - Deleting car entry");
            var result = await _repository.DeleteAsync(consecutive);

            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Car entry with consecutive ID {consecutive} not found",
                    Data = null,
                    Timestamp = DateTime.UtcNow
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = $"Car entry with consecutive ID {consecutive} deleted successfully",
                Data = null,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting car entry with consecutive ID {consecutive}: {ex.Message}");
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
