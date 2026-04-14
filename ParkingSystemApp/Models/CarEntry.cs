using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingSystemApp.Models;

/// <summary>
/// Represents a parking session record (vehicle entry and exit).
/// Corresponds to the PRQ_CarEntry table in the database.
/// Includes computed properties for duration calculations and payment amount.
/// </summary>
[Table("PRQ_CarEntry")]
public class CarEntry
{
    /// <summary>
    /// Primary Key: Consecutive ID for each parking session record.
    /// Auto-incremented to ensure unique tracking of each session.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Consecutive { get; set; }

    /// <summary>
    /// Foreign Key: References the parking lot where the vehicle is parked.
    /// </summary>
    [Required]
    public long ParkingId { get; set; }

    /// <summary>
    /// Foreign Key: References the automobile that entered the parking lot.
    /// </summary>
    [Required]
    public long AutomobileId { get; set; }

    /// <summary>
    /// The date and time when the vehicle entered the parking lot.
    /// Required field - every session must have an entry time.
    /// </summary>
    [Required]
    public DateTime EntryDateTime { get; set; }

    /// <summary>
    /// The date and time when the vehicle exited the parking lot.
    /// Nullable - NULL indicates the vehicle is still parked.
    /// </summary>
    public DateTime? ExitDateTime { get; set; }

    /// <summary>
    /// Timestamp when this record was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when this record was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    // ========================================
    // Navigation Properties
    // ========================================

    /// <summary>
    /// Reference to the parking lot where this session occurred.
    /// </summary>
    [ForeignKey(nameof(ParkingId))]
    public Parking Parking { get; set; } = null!;

    /// <summary>
    /// Reference to the automobile involved in this parking session.
    /// </summary>
    [ForeignKey(nameof(AutomobileId))]
    public Automobile Automobile { get; set; } = null!;

    // ========================================
    // Computed Properties
    // These properties calculate derived values based on entry and exit times
    // All return NULL if the vehicle is still parked (ExitDateTime is NULL)
    // ========================================

    /// <summary>
    /// Calculates the total duration of the parking session in minutes.
    /// Returns NULL if the vehicle has not exited yet (ExitDateTime is NULL).
    /// 
    /// Formula: (ExitDateTime - EntryDateTime).TotalMinutes
    /// </summary>
    [NotMapped]
    public int? StayDurationMinutes
    {
        get
        {
            // If the vehicle hasn't exited, return NULL
            if (ExitDateTime == null)
                return null;

            // Calculate the time difference in minutes
            TimeSpan duration = ExitDateTime.Value - EntryDateTime;
            return (int)duration.TotalMinutes;
        }
    }

    /// <summary>
    /// Calculates the total duration of the parking session in hours.
    /// Returns NULL if the vehicle has not exited yet (ExitDateTime is NULL).
    /// 
    /// Formula: (ExitDateTime - EntryDateTime).TotalHours
    /// Note: This returns a decimal to account for partial hours (e.g., 2.5 hours)
    /// </summary>
    [NotMapped]
    public decimal? StayDurationHours
    {
        get
        {
            // If the vehicle hasn't exited, return NULL
            if (ExitDateTime == null)
                return null;

            // Calculate the time difference in hours as a decimal
            TimeSpan duration = ExitDateTime.Value - EntryDateTime;
            return (decimal)duration.TotalHours;
        }
    }

    /// <summary>
    /// Calculates the total amount to be paid for the parking session.
    /// Returns NULL if the vehicle has not exited yet (ExitDateTime is NULL).
    /// 
    /// Formula: StayDurationHours * Parking.PricePerHour
    /// 
    /// The calculation rounds UP to the next whole hour if there are any partial hours.
    /// For example:
    /// - 2 hours 15 minutes = 3 hours to pay (rounded up)
    /// - 2 hours 0 minutes = 2 hours to pay
    /// </summary>
    [NotMapped]
    public decimal? TotalAmountToPay
    {
        get
        {
            // If the vehicle hasn't exited, return NULL
            if (ExitDateTime == null)
                return null;

            // Calculate the duration
            TimeSpan duration = ExitDateTime.Value - EntryDateTime;

            // Round up to the next whole hour (ceiling)
            // Math.Ceiling rounds 0.1 to 1, 1.0 to 1, 1.1 to 2, etc.
            int hoursToCharge = (int)Math.Ceiling(duration.TotalHours);

            // Calculate the total amount: hours to charge * price per hour
            decimal totalAmount = hoursToCharge * Parking.PricePerHour;

            return totalAmount;
        }
    }

    // ========================================
    // Helper Methods
    // ========================================

    /// <summary>
    /// Determines whether the vehicle is currently parked (has not exited).
    /// </summary>
    /// <returns>True if ExitDateTime is NULL; false otherwise.</returns>
    [NotMapped]
    public bool IsCurrentlyParked => ExitDateTime == null;

    /// <summary>
    /// Determines whether the parking session has been completed.
    /// </summary>
    /// <returns>True if ExitDateTime is not NULL; false otherwise.</returns>
    [NotMapped]
    public bool IsSessionCompleted => ExitDateTime != null;

    /// <summary>
    /// Gets a formatted description of the parking session status.
    /// </summary>
    /// <returns>
    /// "Currently parked" if the vehicle hasn't exited.
    /// "Completed: [hours] hours [minutes] minutes" if the session is finished.
    /// </returns>
    [NotMapped]
    public string SessionStatus
    {
        get
        {
            if (IsCurrentlyParked)
                return "Currently parked";

            TimeSpan duration = ExitDateTime!.Value - EntryDateTime;
            int hours = (int)duration.TotalHours;
            int minutes = duration.Minutes;

            return $"Completed: {hours} hours {minutes} minutes";
        }
    }
}
