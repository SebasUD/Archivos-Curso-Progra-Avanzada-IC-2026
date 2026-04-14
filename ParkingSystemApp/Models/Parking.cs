using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingSystemApp.Models;

/// <summary>
/// Represents a parking lot in the parking system.
/// Corresponds to the PRQ_Parking table in the database.
/// </summary>
[Table("PRQ_Parking")]
public class Parking
{
    /// <summary>
    /// Primary Key: Unique identifier for each parking lot.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    /// <summary>
    /// The name of the province or state where the parking lot is located.
    /// Examples: San José, Alajuela, Cartago, Heredia
    /// </summary>
    [Required]
    [StringLength(100)]
    public string ProvinceName { get; set; } = null!;

    /// <summary>
    /// The name or identifier of the parking lot.
    /// Examples: "Parking Central Downtown", "Alajuela Mall Parking"
    /// </summary>
    [Required]
    [StringLength(150)]
    public string ParkingName { get; set; } = null!;

    /// <summary>
    /// The hourly parking rate in currency units (e.g., USD, CRC).
    /// Must be a positive value.
    /// </summary>
    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal PricePerHour { get; set; }

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
    /// Collection of all parking session records for this parking lot.
    /// </summary>
    public ICollection<CarEntry> CarEntries { get; set; } = new List<CarEntry>();
}
