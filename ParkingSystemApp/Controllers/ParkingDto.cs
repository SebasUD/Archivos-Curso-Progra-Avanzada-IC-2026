namespace ParkingSystemApp.Controllers;

/// <summary>
/// DTO for creating a new parking lot
/// </summary>
public class CreateParkingDto
{
    /// <summary>
    /// The name of the province or state where the parking lot is located
    /// Examples: San José, Alajuela, Cartago, Heredia
    /// </summary>
    public string ProvinceName { get; set; } = null!;

    /// <summary>
    /// The name or identifier of the parking lot
    /// Examples: "Parking Central Downtown", "Alajuela Mall Parking"
    /// </summary>
    public string ParkingName { get; set; } = null!;

    /// <summary>
    /// The hourly parking rate in currency units (e.g., USD, CRC)
    /// Must be a positive value
    /// </summary>
    public decimal PricePerHour { get; set; }
}

/// <summary>
/// DTO for updating an existing parking lot
/// </summary>
public class UpdateParkingDto
{
    /// <summary>
    /// The name of the province or state where the parking lot is located
    /// Examples: San José, Alajuela, Cartago, Heredia
    /// </summary>
    public string ProvinceName { get; set; } = null!;

    /// <summary>
    /// The name or identifier of the parking lot
    /// Examples: "Parking Central Downtown", "Alajuela Mall Parking"
    /// </summary>
    public string ParkingName { get; set; } = null!;

    /// <summary>
    /// The hourly parking rate in currency units (e.g., USD, CRC)
    /// Must be a positive value
    /// </summary>
    public decimal PricePerHour { get; set; }
}
