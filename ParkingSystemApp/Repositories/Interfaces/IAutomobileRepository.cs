using ParkingSystemApp.Models;

namespace ParkingSystemApp.Repositories.Interfaces;

/// <summary>
/// Interface for Automobile repository operations.
/// Defines contracts for CRUD operations and data retrieval from database and JSON sources.
/// </summary>
public interface IAutomobileRepository
{
    // ========================================
    // CRUD Operations
    // ========================================

    /// <summary>
    /// Inserts a new automobile record into the database.
    /// </summary>
    /// <param name="automobile">The automobile entity to insert.</param>
    /// <returns>A task representing the asynchronous operation. Returns the ID of the inserted record.</returns>
    Task<long> InsertAsync(Automobile automobile);

    /// <summary>
    /// Updates an existing automobile record in the database.
    /// </summary>
    /// <param name="automobile">The automobile entity with updated values.</param>
    /// <returns>A task representing the asynchronous operation. Returns true if update was successful.</returns>
    Task<bool> UpdateAsync(Automobile automobile);

    /// <summary>
    /// Deletes an automobile record from the database by ID.
    /// </summary>
    /// <param name="id">The ID of the automobile to delete.</param>
    /// <returns>A task representing the asynchronous operation. Returns true if deletion was successful.</returns>
    Task<bool> DeleteAsync(long id);

    /// <summary>
    /// Retrieves an automobile record by ID from the database.
    /// </summary>
    /// <param name="id">The ID of the automobile to retrieve.</param>
    /// <returns>A task representing the asynchronous operation. Returns the automobile if found; otherwise null.</returns>
    Task<Automobile?> GetByIdAsync(long id);

    // ========================================
    // Data Retrieval from Database
    // ========================================

    /// <summary>
    /// Retrieves all automobile records from the database.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. Returns a collection of all automobiles.</returns>
    Task<IEnumerable<Automobile>> GetAllFromDbAsync();

    // ========================================
    // Data Retrieval from JSON
    // ========================================

    /// <summary>
    /// Retrieves all automobile records from the JSON file.
    /// The JSON file is expected to be located at the root of the solution.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. Returns a collection of automobiles from JSON.</returns>
    Task<IEnumerable<Automobile>> GetAllFromJsonAsync();

    /// <summary>
    /// Gets the path to the Automobiles JSON file.
    /// </summary>
    /// <returns>The full path to the PRQ_Automobiles.json file.</returns>
    string GetJsonFilePath();

    // ========================================
    // JSON-based CRUD Operations
    // ========================================

    /// <summary>
    /// Inserts a new automobile record into the JSON file.
    /// Reads the current JSON file, adds the new record, and persists the changes.
    /// </summary>
    /// <param name="automobile">The automobile entity to insert into JSON.</param>
    /// <returns>A task representing the asynchronous operation. Returns the ID of the inserted record.</returns>
    Task<long> InsertToJsonAsync(Automobile automobile);

    /// <summary>
    /// Updates an existing automobile record in the JSON file.
    /// Reads the JSON file, finds and updates the record, and persists the changes.
    /// </summary>
    /// <param name="automobile">The automobile entity with updated values.</param>
    /// <returns>A task representing the asynchronous operation. Returns true if update was successful.</returns>
    Task<bool> UpdateInJsonAsync(Automobile automobile);

    /// <summary>
    /// Deletes an automobile record from the JSON file.
    /// Reads the JSON file, removes the record, and persists the changes.
    /// </summary>
    /// <param name="id">The ID of the automobile to delete from JSON.</param>
    /// <returns>A task representing the asynchronous operation. Returns true if deletion was successful.</returns>
    Task<bool> DeleteFromJsonAsync(long id);
}
