namespace ParkingSystemApp.Controllers;

/// <summary>
/// Generic API response wrapper for standardized JSON responses
/// </summary>
/// <typeparam name="T">The type of data in the response</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Indicates whether the request was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// A descriptive message about the response
    /// </summary>
    public string Message { get; set; } = null!;

    /// <summary>
    /// The response data payload
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// The UTC timestamp when the response was generated
    /// </summary>
    public DateTime Timestamp { get; set; }
}
