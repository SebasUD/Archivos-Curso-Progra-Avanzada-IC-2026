using System;
using System.Collections.Generic;

namespace ParkingSystemApp.Models;

/// <summary>
/// Automobiles table - MySQL 8.0 optimized
/// </summary>
public partial class PrqAutomobile
{
    /// <summary>
    /// Unique identifier for each automobile
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Vehicle color
    /// </summary>
    public string Color { get; set; } = null!;

    /// <summary>
    /// Manufacturing year (e.g., 2020)
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Vehicle manufacturer/brand name
    /// </summary>
    public string Manufacturer { get; set; } = null!;

    /// <summary>
    /// Vehicle type classification
    /// </summary>
    public string Type { get; set; } = null!;

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
