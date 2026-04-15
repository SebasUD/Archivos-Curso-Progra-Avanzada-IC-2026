using ParkingSystemApp.Models;

namespace ParkingSystemApp.Repositories.Interfaces;

/// <summary>
/// Interface for Parking repository operations.
/// Defines contracts for CRUD operations and data retrieval from database and JSON sources.
/// </summary>
public interface IParkingRepository
{
    // ========================================
    // CRUD Operations
    // ========================================

    /// <summary>
    /// Inserts a new parking record into the database.
    /// </summary>
    /// <param name="parking">The parking entity to insert.</param>
    /// <returns>A task representing the asynchronous operation. Returns the ID of the inserted record.</returns>
    Task<long> InsertAsync(Parking parking);

    /// <summary>
    /// Updates an existing parking record in the database.
    /// </summary>
    /// <param name="parking">The parking entity with updated values.</param>
    /// <returns>A task representing the asynchronous operation. Returns true if update was successful.</returns>
    Task<bool> UpdateAsync(Parking parking);

    /// <summary>
    /// Deletes a parking record from the database by ID.
    /// </summary>
    /// <param name="id">The ID of the parking lot to delete.</param>
    /// <returns>A task representing the asynchronous operation. Returns true if deletion was successful.</returns>
    Task<bool> DeleteAsync(long id);

    /// <summary>
    /// Retrieves a parking record by ID from the database.
    /// </summary>
    /// <param name="id">The ID of the parking lot to retrieve.</param>
    /// <returns>A task representing the asynchronous operation. Returns the parking if found; otherwise null.</returns>
    Task<Parking?> GetByIdAsync(long id);

    // ========================================
    // Data Retrieval from Database
    // ========================================

    /// <summary>
    /// Retrieves all parking records from the database.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. Returns a collection of all parking lots.</returns>
    Task<IEnumerable<Parking>> GetAllFromDbAsync();

    /// <summary>
    /// Retrieves a parking record by province and parking name from the database.
    /// </summary>
    /// <param name="provinceName">The province name.</param>
    /// <param name="parkingName">The parking lot name.</param>
    /// <returns>A task representing the asynchronous operation. Returns the parking if found; otherwise null.</returns>
    Task<Parking?> GetByLocationAsync(string provinceName, string parkingName);

    /// <summary>
    /// Retrieves parking records with advanced filtering.
    /// Supports filtering by province, parking name, and price range.
    /// </summary>
    /// <param name="province">Filter by province name (case-insensitive partial match).</param>
    /// <param name="name">Filter by parking name (case-insensitive partial match).</param>
    /// <param name="priceMin">Filter by minimum price per hour.</param>
    /// <param name="priceMax">Filter by maximum price per hour.</param>
    /// <returns>A task representing the asynchronous operation. Returns filtered parking lots.</returns>
    Task<IEnumerable<Parking>> GetFilteredAsync(string? province = null, string? name = null, decimal? priceMin = null, decimal? priceMax = null);

    // ========================================
    // Data Retrieval from JSON
    // ========================================

    /// <summary>
    /// Retrieves all parking records from the JSON file.
    /// The JSON file is expected to be located at the root of the solution.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. Returns a collection of parking lots from JSON.</returns>
    Task<IEnumerable<Parking>> GetAllFromJsonAsync();

    /// <summary>
    /// Gets the path to the Parking JSON file.
    /// </summary>
    /// <returns>The full path to the PRQ_Parking.json file.</returns>
    string GetJsonFilePath();

    // ========================================
    // JSON-based CRUD Operations
    // ========================================

    /// <summary>
    /// Inserts a new parking record into the JSON file.
    /// Reads the current JSON file, adds the new record, and persists the changes.
    /// </summary>
    /// <param name="parking">The parking entity to insert into JSON.</param>
    /// <returns>A task representing the asynchronous operation. Returns the ID of the inserted record.</returns>
    Task<long> InsertToJsonAsync(Parking parking);

    /// <summary>
    /// Updates an existing parking record in the JSON file.
    /// Reads the JSON file, finds and updates the record, and persists the changes.
    /// </summary>
    /// <param name="parking">The parking entity with updated values.</param>
    /// <returns>A task representing the asynchronous operation. Returns true if update was successful.</returns>
    Task<bool> UpdateInJsonAsync(Parking parking);

    /// <summary>
    /// Deletes a parking record from the JSON file.
    /// Reads the JSON file, removes the record, and persists the changes.
    /// </summary>
    /// <param name="id">The ID of the parking lot to delete from JSON.</param>
    /// <returns>A task representing the asynchronous operation. Returns true if deletion was successful.</returns>
    Task<bool> DeleteFromJsonAsync(long id);
}
