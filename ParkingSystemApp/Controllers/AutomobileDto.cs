namespace ParkingSystemApp.Controllers;

/// <summary>
/// DTO for creating a new automobile
/// </summary>
public class CreateAutomobileDto
{
    /// <summary>
    /// The color of the vehicle
    /// </summary>
    public string Color { get; set; } = null!;

    /// <summary>
    /// The manufacturing year of the vehicle (1900-2100)
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// The manufacturer or brand name of the vehicle
    /// </summary>
    public string Manufacturer { get; set; } = null!;

    /// <summary>
    /// The type of vehicle (sedan, 4x4, motorcycle)
    /// </summary>
    public string Type { get; set; } = null!;
}

/// <summary>
/// DTO for updating an existing automobile
/// </summary>
public class UpdateAutomobileDto
{
    /// <summary>
    /// The color of the vehicle
    /// </summary>
    public string Color { get; set; } = null!;

    /// <summary>
    /// The manufacturing year of the vehicle (1900-2100)
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// The manufacturer or brand name of the vehicle
    /// </summary>
    public string Manufacturer { get; set; } = null!;

    /// <summary>
    /// The type of vehicle (sedan, 4x4, motorcycle)
    /// </summary>
    public string Type { get; set; } = null!;
}
