using System;
using System.Collections.Generic;

namespace ParkingSystemApp.Models;

/// <summary>
/// Car entry/exit records table - MySQL 8.0 optimized
/// </summary>
public partial class PrqCarentry
{
    /// <summary>
    /// Unique consecutive ID for each parking session
    /// </summary>
    public long Consecutive { get; set; }

    /// <summary>
    /// Reference to parking lot (FK)
    /// </summary>
    public long ParkingId { get; set; }

    /// <summary>
    /// Reference to automobile (FK)
    /// </summary>
    public long AutomobileId { get; set; }

    /// <summary>
    /// Date and time when vehicle entered the parking lot
    /// </summary>
    public DateTime EntryDatetime { get; set; }

    /// <summary>
    /// Date and time when vehicle exited (NULL if still parked)
    /// </summary>
    public DateTime? ExitDatetime { get; set; }

    /// <summary>
    /// Record creation timestamp
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Last update timestamp
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    public virtual PrqAutomobile Automobile { get; set; } = null!;

    public virtual PrqParking Parking { get; set; } = null!;
}
