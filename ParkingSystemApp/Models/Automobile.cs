using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingSystemApp.Models;

/// <summary>
/// Represents an automobile/vehicle in the parking system.
/// Corresponds to the PRQ_Automobiles table in the database.
/// </summary>
[Table("PRQ_Automobiles")]
public class Automobile
{
    /// <summary>
    /// Primary Key: Unique identifier for each automobile.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    /// <summary>
    /// The color of the vehicle.
    /// </summary>
    [Required]
    [StringLength(50)]
    public string Color { get; set; } = null!;

    /// <summary>
    /// The manufacturing year of the vehicle.
    /// Range: 1900-2100
    /// </summary>
    [Required]
    public int Year { get; set; }

    /// <summary>
    /// The manufacturer or brand name of the vehicle.
    /// Examples: Toyota, Honda, BMW, Jeep, Yamaha, Harley-Davidson
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Manufacturer { get; set; } = null!;

    /// <summary>
    /// The type of vehicle.
    /// Valid values: sedan, 4x4, motorcycle
    /// </summary>
    [Required]
    [StringLength(20)]
    public string Type { get; set; } = null!;

    /// <summary>
    /// Timestamp when the record was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when the record was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    // ========================================
    // Navigation Properties
    // ========================================

    /// <summary>
    /// Collection of all parking sessions for this automobile.
    /// </summary>
    public ICollection<CarEntry> CarEntries { get; set; } = new List<CarEntry>();
}
