using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ParkingSystemApp.Data;
using ParkingSystemApp.Models;
using ParkingSystemApp.Repositories.Interfaces;
using System.Text.Json;
using DbContext = ParkingSystemApp.Data.ParkingSystemDbContext;

namespace ParkingSystemApp.Repositories.Implementations;

/// <summary>
/// Repository for CarEntry entity.
/// Provides CRUD operations and data retrieval from both database and JSON sources.
/// Includes specialized methods for querying parking sessions.
/// </summary>
public class CarEntryRepository : ICarEntryRepository
{
    private readonly DbContext _context;
    private readonly ILogger<CarEntryRepository> _logger;

    /// <summary>
    /// Constructor for dependency injection.
    /// </summary>
    /// <param name="context">The ParkingSystemDbContext instance.</param>
    /// <param name="logger">The logger instance.</param>
    public CarEntryRepository(DbContext context, ILogger<CarEntryRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // ========================================
    // CRUD Operations
    // ========================================

    /// <summary>
    /// Inserts a new car entry record into the database.
    /// </summary>
    public async Task<long> InsertAsync(CarEntry carEntry)
    {
        if (carEntry == null)
        {
            throw new ArgumentNullException(nameof(carEntry));
        }

        try
        {
            // Set timestamps
            carEntry.CreatedAt = DateTime.UtcNow;
            carEntry.UpdatedAt = DateTime.UtcNow;

            // Add to context
            await _context.CarEntries.AddAsync(carEntry);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Car entry inserted successfully with Consecutive ID: {carEntry.Consecutive}");
            return carEntry.Consecutive;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error inserting car entry: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Updates an existing car entry record in the database.
    /// </summary>
    public async Task<bool> UpdateAsync(CarEntry carEntry)
    {
        if (carEntry == null)
        {
            throw new ArgumentNullException(nameof(carEntry));
        }

        try
        {
            // Check if car entry exists
            var existing = await _context.CarEntries.FindAsync(carEntry.Consecutive);
            if (existing == null)
            {
                _logger.LogWarning($"Car entry with Consecutive {carEntry.Consecutive} not found for update");
                return false;
            }

            // Update properties
            existing.AutomobileId = carEntry.AutomobileId;
            existing.ParkingId = carEntry.ParkingId;
            existing.EntryDateTime = carEntry.EntryDateTime;
            existing.ExitDateTime = carEntry.ExitDateTime;
            existing.UpdatedAt = DateTime.UtcNow;

            // Save changes
            _context.CarEntries.Update(existing);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Car entry with Consecutive {carEntry.Consecutive} updated successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating car entry: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Deletes a car entry record from the database by consecutive ID.
    /// </summary>
    public async Task<bool> DeleteAsync(long consecutive)
    {
        try
        {
            var carEntry = await _context.CarEntries.FindAsync(consecutive);
            if (carEntry == null)
            {
                _logger.LogWarning($"Car entry with Consecutive {consecutive} not found for deletion");
                return false;
            }

            _context.CarEntries.Remove(carEntry);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Car entry with Consecutive {consecutive} deleted successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting car entry: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Retrieves a car entry record by consecutive ID from the database.
    /// </summary>
    public async Task<CarEntry?> GetByIdAsync(long consecutive)
    {
        try
        {
            var carEntry = await _context.CarEntries
                .Include(c => c.Automobile)
                .Include(c => c.Parking)
                .FirstOrDefaultAsync(c => c.Consecutive == consecutive);
            
            if (carEntry != null)
            {
                _logger.LogInformation($"Car entry with Consecutive {consecutive} retrieved successfully");
            }
            else
            {
                _logger.LogWarning($"Car entry with Consecutive {consecutive} not found");
            }

            return carEntry;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving car entry by ID: {ex.Message}");
            throw;
        }
    }

    // ========================================
    // Data Retrieval from Database
    // ========================================

    /// <summary>
    /// Retrieves all car entry records from the database.
    /// </summary>
    public async Task<IEnumerable<CarEntry>> GetAllFromDbAsync()
    {
        try
        {
            var carEntries = await _context.CarEntries
                .Include(c => c.Automobile)
                .Include(c => c.Parking)
                .ToListAsync();

            _logger.LogInformation($"Retrieved {carEntries.Count} car entries from database");
            return carEntries;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving car entries from database: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Retrieves all currently parked vehicles from the database.
    /// A vehicle is considered parked if ExitDateTime is null.
    /// </summary>
    public async Task<IEnumerable<CarEntry>> GetCurrentlyParkedAsync()
    {
        try
        {
            var currentlyParked = await _context.CarEntries
                .Where(c => c.ExitDateTime == null)
                .Include(c => c.Automobile)
                .Include(c => c.Parking)
                .ToListAsync();

            _logger.LogInformation($"Retrieved {currentlyParked.Count} currently parked vehicles from database");
            return currentlyParked;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving currently parked vehicles: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Retrieves all completed parking sessions from the database.
    /// A session is completed when ExitDateTime is not null.
    /// </summary>
    public async Task<IEnumerable<CarEntry>> GetCompletedSessionsAsync()
    {
        try
        {
            var completedSessions = await _context.CarEntries
                .Where(c => c.ExitDateTime != null)
                .Include(c => c.Automobile)
                .Include(c => c.Parking)
                .ToListAsync();

            _logger.LogInformation($"Retrieved {completedSessions.Count} completed sessions from database");
            return completedSessions;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving completed sessions: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Retrieves all car entries for a specific automobile from the database.
    /// </summary>
    public async Task<IEnumerable<CarEntry>> GetByAutomobileAsync(long automobileId)
    {
        try
        {
            var carEntries = await _context.CarEntries
                .Where(c => c.AutomobileId == automobileId)
                .Include(c => c.Parking)
                .ToListAsync();

            _logger.LogInformation($"Retrieved {carEntries.Count} car entries for automobile {automobileId}");
            return carEntries;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving car entries by automobile: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Retrieves all car entries for a specific parking lot from the database.
    /// </summary>
    public async Task<IEnumerable<CarEntry>> GetByParkingAsync(long parkingId)
    {
        try
        {
            var carEntries = await _context.CarEntries
                .Where(c => c.ParkingId == parkingId)
                .Include(c => c.Automobile)
                .ToListAsync();

            _logger.LogInformation($"Retrieved {carEntries.Count} car entries for parking lot {parkingId}");
            return carEntries;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving car entries by parking: {ex.Message}");
            throw;
        }
    }

    // ========================================
    // Data Retrieval from JSON
    // ========================================

    /// <summary>
    /// Retrieves all car entry records from the JSON file.
    /// </summary>
    public async Task<IEnumerable<CarEntry>> GetAllFromJsonAsync()
    {
        try
        {
            string filePath = GetJsonFilePath();

            if (!File.Exists(filePath))
            {
                _logger.LogWarning($"JSON file not found at {filePath}");
                return new List<CarEntry>();
            }

            string jsonContent = await File.ReadAllTextAsync(filePath);
            var carEntries = JsonSerializer.Deserialize<List<CarEntry>>(jsonContent, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<CarEntry>();

            _logger.LogInformation($"Retrieved {carEntries.Count} car entries from JSON file");
            return carEntries;
        }
        catch (JsonException ex)
        {
            _logger.LogError($"Error deserializing JSON: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error reading car entries from JSON: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Gets the path to the CarEntry JSON file.
    /// </summary>
    public string GetJsonFilePath()
    {
        // Navigate from /ParkingSystemApp to root directory
        var projectDirectory = Directory.GetCurrentDirectory();
        // If running from ParkingSystemApp, go up one level
        if (projectDirectory.EndsWith("ParkingSystemApp", StringComparison.OrdinalIgnoreCase))
        {
            projectDirectory = Directory.GetParent(projectDirectory)?.FullName ?? projectDirectory;
        }

        return Path.Combine(projectDirectory, "PRQ_CarEntry.json");
    }
}
