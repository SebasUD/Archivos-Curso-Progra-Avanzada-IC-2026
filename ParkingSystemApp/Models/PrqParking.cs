using System;
using System.Collections.Generic;

namespace ParkingSystemApp.Models;

/// <summary>
/// Parking lots table - MySQL 8.0 optimized
/// </summary>
public partial class PrqParking
{
    /// <summary>
    /// Unique identifier for each parking lot
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Province/State name where parking is located
    /// </summary>
    public string ProvinceName { get; set; } = null!;

    /// <summary>
    /// Name/identifier of the parking lot
    /// </summary>
    public string ParkingName { get; set; } = null!;

    /// <summary>
    /// Hourly parking rate in currency units
    /// </summary>
    public decimal PricePerHour { get; set; }

    /// <summary>
    /// Record creation timestamp
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Last update timestamp
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<PrqCarentry> PrqCarentries { get; set; } = new List<PrqCarentry>();
}
