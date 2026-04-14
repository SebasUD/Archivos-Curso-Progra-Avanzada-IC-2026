using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ParkingSystemApp.Data;
using ParkingSystemApp.Models;
using ParkingSystemApp.Repositories.Interfaces;
using System.Text.Json;
using DbContext = ParkingSystemApp.Data.ParkingSystemDbContext;

namespace ParkingSystemApp.Repositories.Implementations;

/// <summary>
/// Repository for Automobile entity.
/// Provides CRUD operations and data retrieval from both database and JSON sources.
/// </summary>
public class AutomobileRepository : IAutomobileRepository
{
    private readonly DbContext _context;
    private readonly ILogger<AutomobileRepository> _logger;

    /// <summary>
    /// Constructor for dependency injection.
    /// </summary>
    /// <param name="context">The ParkingSystemDbContext instance.</param>
    /// <param name="logger">The logger instance.</param>
    public AutomobileRepository(DbContext context, ILogger<AutomobileRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // ========================================
    // CRUD Operations
    // ========================================

    /// <summary>
    /// Inserts a new automobile record into the database.
    /// </summary>
    public async Task<long> InsertAsync(Automobile automobile)
    {
        if (automobile == null)
        {
            throw new ArgumentNullException(nameof(automobile));
        }

        try
        {
            // Set timestamps
            automobile.CreatedAt = DateTime.UtcNow;
            automobile.UpdatedAt = DateTime.UtcNow;

            // Add to context
            await _context.Automobiles.AddAsync(automobile);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Automobile inserted successfully with ID: {automobile.Id}");
            return automobile.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error inserting automobile: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Updates an existing automobile record in the database.
    /// </summary>
    public async Task<bool> UpdateAsync(Automobile automobile)
    {
        if (automobile == null)
        {
            throw new ArgumentNullException(nameof(automobile));
        }

        try
        {
            // Check if automobile exists
            var existing = await _context.Automobiles.FindAsync(automobile.Id);
            if (existing == null)
            {
                _logger.LogWarning($"Automobile with ID {automobile.Id} not found for update");
                return false;
            }

            // Update properties
            existing.Color = automobile.Color;
            existing.Year = automobile.Year;
            existing.Manufacturer = automobile.Manufacturer;
            existing.Type = automobile.Type;
            existing.UpdatedAt = DateTime.UtcNow;

            // Save changes
            _context.Automobiles.Update(existing);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Automobile with ID {automobile.Id} updated successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating automobile: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Deletes an automobile record from the database by ID.
    /// </summary>
    public async Task<bool> DeleteAsync(long id)
    {
        try
        {
            var automobile = await _context.Automobiles.FindAsync(id);
            if (automobile == null)
            {
                _logger.LogWarning($"Automobile with ID {id} not found for deletion");
                return false;
            }

            _context.Automobiles.Remove(automobile);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Automobile with ID {id} deleted successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting automobile: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Retrieves an automobile record by ID from the database.
    /// </summary>
    public async Task<Automobile?> GetByIdAsync(long id)
    {
        try
        {
            var automobile = await _context.Automobiles.FirstOrDefaultAsync(a => a.Id == id);
            
            if (automobile != null)
            {
                _logger.LogInformation($"Automobile with ID {id} retrieved successfully");
            }
            else
            {
                _logger.LogWarning($"Automobile with ID {id} not found");
            }

            return automobile;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving automobile by ID: {ex.Message}");
            throw;
        }
    }

    // ========================================
    // Data Retrieval from Database
    // ========================================

    /// <summary>
    /// Retrieves all automobile records from the database.
    /// </summary>
    public async Task<IEnumerable<Automobile>> GetAllFromDbAsync()
    {
        try
        {
            var automobiles = await _context.Automobiles.ToListAsync();
            _logger.LogInformation($"Retrieved {automobiles.Count} automobiles from database");
            return automobiles;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving automobiles from database: {ex.Message}");
            throw;
        }
    }

    // ========================================
    // Data Retrieval from JSON
    // ========================================

    /// <summary>
    /// Retrieves all automobile records from the JSON file.
    /// </summary>
    public async Task<IEnumerable<Automobile>> GetAllFromJsonAsync()
    {
        try
        {
            string filePath = GetJsonFilePath();

            if (!File.Exists(filePath))
            {
                _logger.LogWarning($"JSON file not found at {filePath}");
                return new List<Automobile>();
            }

            string jsonContent = await File.ReadAllTextAsync(filePath);
            var automobiles = JsonSerializer.Deserialize<List<Automobile>>(jsonContent, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Automobile>();

            _logger.LogInformation($"Retrieved {automobiles.Count} automobiles from JSON file");
            return automobiles;
        }
        catch (JsonException ex)
        {
            _logger.LogError($"Error deserializing JSON: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error reading automobiles from JSON: {ex.Message}");
            throw;
        }
    }

    // ========================================
    // JSON-based CRUD Operations
    // ========================================

    /// <summary>
    /// Inserts a new automobile record into the JSON file.
    /// Reads the current JSON file, adds the new record, and persists the changes.
    /// </summary>
    public async Task<long> InsertToJsonAsync(Automobile automobile)
    {
        if (automobile == null)
        {
            throw new ArgumentNullException(nameof(automobile));
        }

        try
        {
            string filePath = GetJsonFilePath();

            // Read existing automobiles from JSON
            var automobiles = new List<Automobile>();
            if (File.Exists(filePath))
            {
                string jsonContent = await File.ReadAllTextAsync(filePath);
                automobiles = JsonSerializer.Deserialize<List<Automobile>>(jsonContent, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Automobile>();
            }

            // Set timestamps
            automobile.CreatedAt = DateTime.UtcNow;
            automobile.UpdatedAt = DateTime.UtcNow;

            // Generate new ID (max existing ID + 1)
            automobile.Id = automobiles.Count > 0 ? automobiles.Max(a => a.Id) + 1 : 1;

            // Add to list
            automobiles.Add(automobile);

            // Write back to JSON file
            var options = new JsonSerializerOptions { WriteIndented = true };
            string updatedJson = JsonSerializer.Serialize(automobiles, options);
            await File.WriteAllTextAsync(filePath, updatedJson);

            _logger.LogInformation($"Automobile inserted into JSON file with ID: {automobile.Id}");
            return automobile.Id;
        }
        catch (JsonException ex)
        {
            _logger.LogError($"Error serializing to JSON: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error inserting automobile to JSON: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Updates an existing automobile record in the JSON file.
    /// Reads the JSON file, finds and updates the record, and persists the changes.
    /// </summary>
    public async Task<bool> UpdateInJsonAsync(Automobile automobile)
    {
        if (automobile == null)
        {
            throw new ArgumentNullException(nameof(automobile));
        }

        try
        {
            string filePath = GetJsonFilePath();

            if (!File.Exists(filePath))
            {
                _logger.LogWarning($"JSON file not found at {filePath}");
                return false;
            }

            // Read existing automobiles from JSON
            string jsonContent = await File.ReadAllTextAsync(filePath);
            var automobiles = JsonSerializer.Deserialize<List<Automobile>>(jsonContent, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Automobile>();

            // Find and update the automobile
            var existingAutomobile = automobiles.FirstOrDefault(a => a.Id == automobile.Id);
            if (existingAutomobile == null)
            {
                _logger.LogWarning($"Automobile with ID {automobile.Id} not found in JSON file");
                return false;
            }

            // Update properties
            existingAutomobile.Color = automobile.Color;
            existingAutomobile.Year = automobile.Year;
            existingAutomobile.Manufacturer = automobile.Manufacturer;
            existingAutomobile.Type = automobile.Type;
            existingAutomobile.UpdatedAt = DateTime.UtcNow;

            // Write back to JSON file
            var options = new JsonSerializerOptions { WriteIndented = true };
            string updatedJson = JsonSerializer.Serialize(automobiles, options);
            await File.WriteAllTextAsync(filePath, updatedJson);

            _logger.LogInformation($"Automobile with ID {automobile.Id} updated in JSON file");
            return true;
        }
        catch (JsonException ex)
        {
            _logger.LogError($"Error deserializing from JSON: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating automobile in JSON: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Deletes an automobile record from the JSON file.
    /// Reads the JSON file, removes the record, and persists the changes.
    /// </summary>
    public async Task<bool> DeleteFromJsonAsync(long id)
    {
        try
        {
            string filePath = GetJsonFilePath();

            if (!File.Exists(filePath))
            {
                _logger.LogWarning($"JSON file not found at {filePath}");
                return false;
            }

            // Read existing automobiles from JSON
            string jsonContent = await File.ReadAllTextAsync(filePath);
            var automobiles = JsonSerializer.Deserialize<List<Automobile>>(jsonContent, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Automobile>();

            // Find and remove the automobile
            var automobileToDelete = automobiles.FirstOrDefault(a => a.Id == id);
            if (automobileToDelete == null)
            {
                _logger.LogWarning($"Automobile with ID {id} not found in JSON file");
                return false;
            }

            automobiles.Remove(automobileToDelete);

            // Write back to JSON file
            var options = new JsonSerializerOptions { WriteIndented = true };
            string updatedJson = JsonSerializer.Serialize(automobiles, options);
            await File.WriteAllTextAsync(filePath, updatedJson);

            _logger.LogInformation($"Automobile with ID {id} deleted from JSON file");
            return true;
        }
        catch (JsonException ex)
        {
            _logger.LogError($"Error deserializing from JSON: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting automobile from JSON: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Gets the path to the Automobiles JSON file.
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

        return Path.Combine(projectDirectory, "PRQ_Automobiles.json");
    }
}
