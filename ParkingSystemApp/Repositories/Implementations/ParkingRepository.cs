using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ParkingSystemApp.Data;
using ParkingSystemApp.Models;
using ParkingSystemApp.Repositories.Interfaces;
using System.Text.Json;
using DbContext = ParkingSystemApp.Data.ParkingSystemDbContext;

namespace ParkingSystemApp.Repositories.Implementations;

/// <summary>
/// Repository for Parking entity.
/// Provides CRUD operations and data retrieval from both database and JSON sources.
/// </summary>
public class ParkingRepository : IParkingRepository
{
    private readonly DbContext _context;
    private readonly ILogger<ParkingRepository> _logger;

    /// <summary>
    /// Constructor for dependency injection.
    /// </summary>
    /// <param name="context">The ParkingSystemDbContext instance.</param>
    /// <param name="logger">The logger instance.</param>
    public ParkingRepository(DbContext context, ILogger<ParkingRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // ========================================
    // CRUD Operations
    // ========================================

    /// <summary>
    /// Inserts a new parking record into the database.
    /// </summary>
    public async Task<long> InsertAsync(Parking parking)
    {
        if (parking == null)
        {
            throw new ArgumentNullException(nameof(parking));
        }

        try
        {
            // Set timestamps
            parking.CreatedAt = DateTime.UtcNow;
            parking.UpdatedAt = DateTime.UtcNow;

            // Add to context
            await _context.ParkingLots.AddAsync(parking);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Parking lot inserted successfully with ID: {parking.Id}");
            return parking.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error inserting parking lot: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Updates an existing parking record in the database.
    /// </summary>
    public async Task<bool> UpdateAsync(Parking parking)
    {
        if (parking == null)
        {
            throw new ArgumentNullException(nameof(parking));
        }

        try
        {
            // Check if parking exists
            var existing = await _context.ParkingLots.FindAsync(parking.Id);
            if (existing == null)
            {
                _logger.LogWarning($"Parking lot with ID {parking.Id} not found for update");
                return false;
            }

            // Update properties
            existing.ProvinceName = parking.ProvinceName;
            existing.ParkingName = parking.ParkingName;
            existing.PricePerHour = parking.PricePerHour;
            existing.UpdatedAt = DateTime.UtcNow;

            // Save changes
            _context.ParkingLots.Update(existing);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Parking lot with ID {parking.Id} updated successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating parking lot: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Deletes a parking record from the database by ID.
    /// </summary>
    public async Task<bool> DeleteAsync(long id)
    {
        try
        {
            var parking = await _context.ParkingLots.FindAsync(id);
            if (parking == null)
            {
                _logger.LogWarning($"Parking lot with ID {id} not found for deletion");
                return false;
            }

            _context.ParkingLots.Remove(parking);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Parking lot with ID {id} deleted successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting parking lot: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Retrieves a parking record by ID from the database.
    /// </summary>
    public async Task<Parking?> GetByIdAsync(long id)
    {
        try
        {
            var parking = await _context.ParkingLots.FirstOrDefaultAsync(p => p.Id == id);
            
            if (parking != null)
            {
                _logger.LogInformation($"Parking lot with ID {id} retrieved successfully");
            }
            else
            {
                _logger.LogWarning($"Parking lot with ID {id} not found");
            }

            return parking;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving parking lot by ID: {ex.Message}");
            throw;
        }
    }

    // ========================================
    // Data Retrieval from Database
    // ========================================

    /// <summary>
    /// Retrieves all parking records from the database.
    /// </summary>
    public async Task<IEnumerable<Parking>> GetAllFromDbAsync()
    {
        try
        {
            var parkings = await _context.ParkingLots.ToListAsync();
            _logger.LogInformation($"Retrieved {parkings.Count} parking lots from database");
            return parkings;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving parking lots from database: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Retrieves a parking record by province and parking name from the database.
    /// </summary>
    public async Task<Parking?> GetByLocationAsync(string provinceName, string parkingName)
    {
        if (string.IsNullOrWhiteSpace(provinceName) || string.IsNullOrWhiteSpace(parkingName))
        {
            _logger.LogWarning("Province name and parking name cannot be null or empty");
            return null;
        }

        try
        {
            var parking = await _context.ParkingLots
                .FirstOrDefaultAsync(p => p.ProvinceName == provinceName && p.ParkingName == parkingName);

            if (parking != null)
            {
                _logger.LogInformation($"Parking lot found: {provinceName} - {parkingName}");
            }
            else
            {
                _logger.LogWarning($"Parking lot not found: {provinceName} - {parkingName}");
            }

            return parking;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving parking lot by location: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Retrieves parking records with advanced filtering.
    /// Supports filtering by province, parking name, and price range.
    /// </summary>
    public async Task<IEnumerable<Parking>> GetFilteredAsync(string? province = null, string? name = null, decimal? priceMin = null, decimal? priceMax = null)
    {
        try
        {
            // Validate price range
            if ((priceMin.HasValue && priceMin < 0) || (priceMax.HasValue && priceMax < 0))
            {
                _logger.LogWarning("Price values cannot be negative");
                return new List<Parking>();
            }

            if (priceMin.HasValue && priceMax.HasValue && priceMin > priceMax)
            {
                _logger.LogWarning("priceMin cannot be greater than priceMax");
                return new List<Parking>();
            }

            // Build the query with optional filters
            var query = _context.ParkingLots.AsQueryable();

            // Filter by province (case-insensitive partial match)
            if (!string.IsNullOrWhiteSpace(province))
            {
                query = query.Where(p => p.ProvinceName.ToLower().Contains(province.ToLower()));
            }

            // Filter by parking name (case-insensitive partial match)
            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(p => p.ParkingName.ToLower().Contains(name.ToLower()));
            }

            // Filter by price range
            if (priceMin.HasValue)
            {
                query = query.Where(p => p.PricePerHour >= priceMin.Value);
            }

            if (priceMax.HasValue)
            {
                query = query.Where(p => p.PricePerHour <= priceMax.Value);
            }

            // Execute the query
            var parkings = await query.ToListAsync();
            
            _logger.LogInformation($"Retrieved {parkings.Count} parking lots matching filter criteria: province={province}, name={name}, priceMin={priceMin}, priceMax={priceMax}");
            return parkings;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error filtering parking lots: {ex.Message}");
            throw;
        }
    }

    // ========================================
    // Data Retrieval from JSON
    // ========================================

    /// <summary>
    /// Retrieves all parking records from the JSON file.
    /// </summary>
    public async Task<IEnumerable<Parking>> GetAllFromJsonAsync()
    {
        try
        {
            string filePath = GetJsonFilePath();

            if (!File.Exists(filePath))
            {
                _logger.LogWarning($"JSON file not found at {filePath}");
                return new List<Parking>();
            }

            string jsonContent = await File.ReadAllTextAsync(filePath);
            var parkings = JsonSerializer.Deserialize<List<Parking>>(jsonContent, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Parking>();

            _logger.LogInformation($"Retrieved {parkings.Count} parking lots from JSON file");
            return parkings;
        }
        catch (JsonException ex)
        {
            _logger.LogError($"Error deserializing JSON: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error reading parking lots from JSON: {ex.Message}");
            throw;
        }
    }

    // ========================================
    // JSON-based CRUD Operations
    // ========================================

    /// <summary>
    /// Inserts a new parking record into the JSON file.
    /// Reads the current JSON file, adds the new record, and persists the changes.
    /// </summary>
    public async Task<long> InsertToJsonAsync(Parking parking)
    {
        if (parking == null)
        {
            throw new ArgumentNullException(nameof(parking));
        }

        try
        {
            string filePath = GetJsonFilePath();

            // Read existing parking lots from JSON
            var parkings = new List<Parking>();
            if (File.Exists(filePath))
            {
                string jsonContent = await File.ReadAllTextAsync(filePath);
                parkings = JsonSerializer.Deserialize<List<Parking>>(jsonContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Parking>();
            }

            // Set timestamps
            parking.CreatedAt = DateTime.UtcNow;
            parking.UpdatedAt = DateTime.UtcNow;

            // Generate new ID (max existing ID + 1)
            parking.Id = parkings.Count > 0 ? parkings.Max(p => p.Id) + 1 : 1;

            // Add to list
            parkings.Add(parking);

            // Write back to JSON file
            var options = new JsonSerializerOptions { WriteIndented = true };
            string updatedJson = JsonSerializer.Serialize(parkings, options);
            await File.WriteAllTextAsync(filePath, updatedJson);

            _logger.LogInformation($"Parking lot inserted into JSON file with ID: {parking.Id}");
            return parking.Id;
        }
        catch (JsonException ex)
        {
            _logger.LogError($"Error serializing to JSON: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error inserting parking lot to JSON: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Updates an existing parking record in the JSON file.
    /// Reads the JSON file, finds and updates the record, and persists the changes.
    /// </summary>
    public async Task<bool> UpdateInJsonAsync(Parking parking)
    {
        if (parking == null)
        {
            throw new ArgumentNullException(nameof(parking));
        }

        try
        {
            string filePath = GetJsonFilePath();

            if (!File.Exists(filePath))
            {
                _logger.LogWarning($"JSON file not found at {filePath}");
                return false;
            }

            // Read existing parking lots from JSON
            string jsonContent = await File.ReadAllTextAsync(filePath);
            var parkings = JsonSerializer.Deserialize<List<Parking>>(jsonContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Parking>();

            // Find and update the parking lot
            var existingParking = parkings.FirstOrDefault(p => p.Id == parking.Id);
            if (existingParking == null)
            {
                _logger.LogWarning($"Parking lot with ID {parking.Id} not found in JSON file");
                return false;
            }

            // Update properties
            existingParking.ProvinceName = parking.ProvinceName;
            existingParking.ParkingName = parking.ParkingName;
            existingParking.PricePerHour = parking.PricePerHour;
            existingParking.UpdatedAt = DateTime.UtcNow;

            // Write back to JSON file
            var options = new JsonSerializerOptions { WriteIndented = true };
            string updatedJson = JsonSerializer.Serialize(parkings, options);
            await File.WriteAllTextAsync(filePath, updatedJson);

            _logger.LogInformation($"Parking lot with ID {parking.Id} updated in JSON file");
            return true;
        }
        catch (JsonException ex)
        {
            _logger.LogError($"Error deserializing from JSON: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating parking lot in JSON: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Deletes a parking record from the JSON file.
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

            // Read existing parking lots from JSON
            string jsonContent = await File.ReadAllTextAsync(filePath);
            var parkings = JsonSerializer.Deserialize<List<Parking>>(jsonContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Parking>();

            // Find and remove the parking lot
            var parkingToDelete = parkings.FirstOrDefault(p => p.Id == id);
            if (parkingToDelete == null)
            {
                _logger.LogWarning($"Parking lot with ID {id} not found in JSON file");
                return false;
            }

            parkings.Remove(parkingToDelete);

            // Write back to JSON file
            var options = new JsonSerializerOptions { WriteIndented = true };
            string updatedJson = JsonSerializer.Serialize(parkings, options);
            await File.WriteAllTextAsync(filePath, updatedJson);

            _logger.LogInformation($"Parking lot with ID {id} deleted from JSON file");
            return true;
        }
        catch (JsonException ex)
        {
            _logger.LogError($"Error deserializing from JSON: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting parking lot from JSON: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Gets the path to the Parking JSON file.
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

        return Path.Combine(projectDirectory, "PRQ_Parking.json");
    }
}
