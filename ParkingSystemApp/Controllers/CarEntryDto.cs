namespace ParkingSystemApp.Controllers;

/// <summary>
/// DTO for car entry response with computed fields
/// </summary>
public class CarEntryResponseDto
{
    /// <summary>
    /// The consecutive/primary key ID
    /// </summary>
    public long Consecutive { get; set; }

    /// <summary>
    /// Foreign key reference to the parking lot
    /// </summary>
    public long ParkingId { get; set; }

    /// <summary>
    /// Foreign key reference to the automobile
    /// </summary>
    public long AutomobileId { get; set; }

    /// <summary>
    /// The date and time when the vehicle entered the parking lot
    /// </summary>
    public DateTime EntryDateTime { get; set; }

    /// <summary>
    /// The date and time when the vehicle exited the parking lot (NULL if still parked)
    /// </summary>
    public DateTime? ExitDateTime { get; set; }

    /// <summary>
    /// Computed field: Total duration of parking session in minutes
    /// NULL if the vehicle is still parked (ExitDateTime is NULL)
    /// </summary>
    public int? StayMinutes { get; set; }

    /// <summary>
    /// Computed field: Total duration of parking session in hours (decimal for partial hours)
    /// NULL if the vehicle is still parked (ExitDateTime is NULL)
    /// </summary>
    public decimal? StayHours { get; set; }

    /// <summary>
    /// Computed field: Total amount to pay for the parking session
    /// Calculated as: ceiling(StayHours) * PricePerHour
    /// NULL if the vehicle is still parked (ExitDateTime is NULL)
    /// </summary>
    public decimal? TotalAmount { get; set; }

    /// <summary>
    /// Timestamp when the record was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when the record was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// DTO for creating a new car entry
/// </summary>
public class CreateCarEntryDto
{
    /// <summary>
    /// Foreign key reference to the parking lot
    /// </summary>
    public long ParkingId { get; set; }

    /// <summary>
    /// Foreign key reference to the automobile
    /// </summary>
    public long AutomobileId { get; set; }

    /// <summary>
    /// The date and time when the vehicle entered the parking lot
    /// </summary>
    public DateTime EntryDateTime { get; set; }

    /// <summary>
    /// Optional: The date and time when the vehicle exited the parking lot
    /// Leave NULL if vehicle is still parked
    /// </summary>
    public DateTime? ExitDateTime { get; set; }
}

/// <summary>
/// DTO for updating an existing car entry
/// </summary>
public class UpdateCarEntryDto
{
    /// <summary>
    /// Foreign key reference to the parking lot
    /// </summary>
    public long ParkingId { get; set; }

    /// <summary>
    /// Foreign key reference to the automobile
    /// </summary>
    public long AutomobileId { get; set; }

    /// <summary>
    /// The date and time when the vehicle entered the parking lot
    /// </summary>
    public DateTime EntryDateTime { get; set; }

    /// <summary>
    /// The date and time when the vehicle exited the parking lot
    /// Set to NULL to mark the vehicle as still parked
    /// </summary>
    public DateTime? ExitDateTime { get; set; }
}
