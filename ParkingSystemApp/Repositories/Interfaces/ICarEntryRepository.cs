using ParkingSystemApp.Models;

namespace ParkingSystemApp.Repositories.Interfaces;

/// <summary>
/// Interface for CarEntry repository operations.
/// Defines contracts for CRUD operations and data retrieval from database and JSON sources.
/// </summary>
public interface ICarEntryRepository
{
    // ========================================
    // CRUD Operations
    // ========================================

    /// <summary>
    /// Inserts a new car entry record into the database.
    /// </summary>
    /// <param name="carEntry">The car entry entity to insert.</param>
    /// <returns>A task representing the asynchronous operation. Returns the consecutive ID of the inserted record.</returns>
    Task<long> InsertAsync(CarEntry carEntry);

    /// <summary>
    /// Updates an existing car entry record in the database.
    /// </summary>
    /// <param name="carEntry">The car entry entity with updated values.</param>
    /// <returns>A task representing the asynchronous operation. Returns true if update was successful.</returns>
    Task<bool> UpdateAsync(CarEntry carEntry);

    /// <summary>
    /// Deletes a car entry record from the database by consecutive ID.
    /// </summary>
    /// <param name="consecutive">The consecutive ID of the car entry to delete.</param>
    /// <returns>A task representing the asynchronous operation. Returns true if deletion was successful.</returns>
    Task<bool> DeleteAsync(long consecutive);

    /// <summary>
    /// Retrieves a car entry record by consecutive ID from the database.
    /// </summary>
    /// <param name="consecutive">The consecutive ID of the car entry to retrieve.</param>
    /// <returns>A task representing the asynchronous operation. Returns the car entry if found; otherwise null.</returns>
    Task<CarEntry?> GetByIdAsync(long consecutive);

    // ========================================
    // Data Retrieval from Database
    // ========================================

    /// <summary>
    /// Retrieves all car entry records from the database.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. Returns a collection of all car entries.</returns>
    Task<IEnumerable<CarEntry>> GetAllFromDbAsync();

    /// <summary>
    /// Retrieves all currently parked vehicles from the database.
    /// A vehicle is considered parked if ExitDateTime is null.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. Returns a collection of active car entries.</returns>
    Task<IEnumerable<CarEntry>> GetCurrentlyParkedAsync();

    /// <summary>
    /// Retrieves all completed parking sessions from the database.
    /// A session is completed when ExitDateTime is not null.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. Returns a collection of completed car entries.</returns>
    Task<IEnumerable<CarEntry>> GetCompletedSessionsAsync();

    /// <summary>
    /// Retrieves all car entries for a specific automobile from the database.
    /// </summary>
    /// <param name="automobileId">The ID of the automobile.</param>
    /// <returns>A task representing the asynchronous operation. Returns a collection of car entries for the specified automobile.</returns>
    Task<IEnumerable<CarEntry>> GetByAutomobileAsync(long automobileId);

    /// <summary>
    /// Retrieves all car entries for a specific parking lot from the database.
    /// </summary>
    /// <param name="parkingId">The ID of the parking lot.</param>
    /// <returns>A task representing the asynchronous operation. Returns a collection of car entries for the specified parking lot.</returns>
    Task<IEnumerable<CarEntry>> GetByParkingAsync(long parkingId);

    /// <summary>
    /// Retrieves car entries filtered by vehicle type and date range.
    /// </summary>
    /// <param name="vehicleType">The vehicle type to filter by (e.g., sedan, 4x4, motorcycle).</param>
    /// <param name="dateStart">The start date for the filter (inclusive).</param>
    /// <param name="dateEnd">The end date for the filter (inclusive).</param>
    /// <returns>A task representing the asynchronous operation. Returns filtered car entries.</returns>
    Task<IEnumerable<CarEntry>> GetByVehicleTypeAndDateRangeAsync(string? vehicleType = null, DateTime? dateStart = null, DateTime? dateEnd = null);

    /// <summary>
    /// Retrieves car entries filtered by province and date range.
    /// </summary>
    /// <param name="province">The province name to filter by (case-insensitive).</param>
    /// <param name="dateStart">The start date for the filter (inclusive).</param>
    /// <param name="dateEnd">The end date for the filter (inclusive).</param>
    /// <returns>A task representing the asynchronous operation. Returns filtered car entries.</returns>
    Task<IEnumerable<CarEntry>> GetByProvinceAndDateRangeAsync(string? province = null, DateTime? dateStart = null, DateTime? dateEnd = null);

    // ========================================
    // Data Retrieval from JSON
    // ========================================

    /// <summary>
    /// Retrieves all car entry records from the JSON file.
    /// The JSON file is expected to be located at the root of the solution.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. Returns a collection of car entries from JSON.</returns>
    Task<IEnumerable<CarEntry>> GetAllFromJsonAsync();

    /// <summary>
    /// Gets the path to the CarEntry JSON file.
    /// </summary>
    /// <returns>The full path to the PRQ_CarEntry.json file.</returns>
    string GetJsonFilePath();

    // ========================================
    // JSON-based CRUD Operations
    // ========================================

    /// <summary>
    /// Inserts a new car entry record into the JSON file.
    /// Reads the current JSON file, adds the new record, and persists the changes.
    /// </summary>
    /// <param name="carEntry">The car entry entity to insert into JSON.</param>
    /// <returns>A task representing the asynchronous operation. Returns the Consecutive ID of the inserted record.</returns>
    Task<long> InsertToJsonAsync(CarEntry carEntry);

    /// <summary>
    /// Updates an existing car entry record in the JSON file.
    /// Reads the JSON file, finds and updates the record, and persists the changes.
    /// </summary>
    /// <param name="carEntry">The car entry entity with updated values.</param>
    /// <returns>A task representing the asynchronous operation. Returns true if update was successful.</returns>
    Task<bool> UpdateInJsonAsync(CarEntry carEntry);

    /// <summary>
    /// Deletes a car entry record from the JSON file.
    /// Reads the JSON file, removes the record, and persists the changes.
    /// </summary>
    /// <param name="consecutive">The Consecutive ID of the car entry to delete from JSON.</param>
    /// <returns>A task representing the asynchronous operation. Returns true if deletion was successful.</returns>
    Task<bool> DeleteFromJsonAsync(long consecutive);
}
