using Microsoft.EntityFrameworkCore;
using ParkingSystemApp.Models;

namespace ParkingSystemApp.Data;

/// <summary>
/// DbContext for the Parking System database.
/// Manages the entity configuration and database operations for the PRQ_ tables.
/// 
/// Supports MySQL 8.0+ with Pomelo Entity Framework Core provider.
/// </summary>
public class ParkingSystemDbContext : DbContext
{
    /// <summary>
    /// Constructor for dependency injection.
    /// </summary>
    /// <param name="options">DbContextOptions configured with MySQL connection string.</param>
    public ParkingSystemDbContext(DbContextOptions<ParkingSystemDbContext> options)
        : base(options)
    {
    }

    // ========================================
    // DbSets - Table References
    // ========================================

    /// <summary>
    /// DbSet for the PRQ_Automobiles table.
    /// Contains all vehicles in the parking system.
    /// </summary>
    public DbSet<Automobile> Automobiles { get; set; } = null!;

    /// <summary>
    /// DbSet for the PRQ_Parking table.
    /// Contains all parking lots in the system.
    /// </summary>
    public DbSet<Parking> ParkingLots { get; set; } = null!;

    /// <summary>
    /// DbSet for the PRQ_CarEntry table.
    /// Contains all parking session records (entry/exit logs).
    /// </summary>
    public DbSet<CarEntry> CarEntries { get; set; } = null!;

    // ========================================
    // Model Configuration
    // ========================================

    /// <summary>
    /// Configures the entity models and database schema.
    /// Sets up relationships, constraints, and indexing.
    /// </summary>
    /// <param name="modelBuilder">The model builder used to configure entities.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ========================================
        // PRQ_Automobiles Configuration
        // ========================================
        modelBuilder.Entity<Automobile>(entity =>
        {
            // Table name mapping
            entity.ToTable("PRQ_Automobiles");

            // Primary Key
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnType("bigint")
                .ValueGeneratedOnAdd();

            // Column configurations
            entity.Property(e => e.Color)
                .HasMaxLength(50)
                .IsRequired()
                .HasComment("Vehicle color");

            entity.Property(e => e.Year)
                .HasColumnType("int")
                .IsRequired()
                .HasComment("Manufacturing year (1900-2100)");

            entity.Property(e => e.Manufacturer)
                .HasMaxLength(100)
                .IsRequired()
                .HasComment("Vehicle manufacturer/brand name");

            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .IsRequired()
                .HasComment("Vehicle type: sedan, 4x4, motorcycle");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Record creation timestamp");

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Last update timestamp");

            // Indexes
            entity.HasIndex(e => e.Manufacturer).HasDatabaseName("idx_manufacturer");
            entity.HasIndex(e => e.Type).HasDatabaseName("idx_type");
            entity.HasIndex(e => e.Year).HasDatabaseName("idx_year");

            // Relationships
            entity.HasMany(e => e.CarEntries)
                .WithOne(ce => ce.Automobile)
                .HasForeignKey(ce => ce.AutomobileId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ========================================
        // PRQ_Parking Configuration
        // ========================================
        modelBuilder.Entity<Parking>(entity =>
        {
            // Table name mapping
            entity.ToTable("PRQ_Parking");

            // Primary Key
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnType("bigint")
                .ValueGeneratedOnAdd();

            // Column configurations
            entity.Property(e => e.ProvinceName)
                .HasColumnName("province_name")
                .HasMaxLength(100)
                .IsRequired()
                .HasComment("Province/state name");

            entity.Property(e => e.ParkingName)
                .HasColumnName("parking_name")
                .HasMaxLength(150)
                .IsRequired()
                .HasComment("Name of parking lot");

            entity.Property(e => e.PricePerHour)
                .HasColumnName("price_per_hour")
                .HasColumnType("decimal(10,2)")
                .IsRequired()
                .HasComment("Hourly parking rate");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Record creation timestamp");

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Last update timestamp");

            // Unique constraint
            entity.HasIndex(e => new { e.ProvinceName, e.ParkingName })
                .HasDatabaseName("uk_parking_location")
                .IsUnique();

            // Indexes
            entity.HasIndex(e => e.ProvinceName).HasDatabaseName("idx_province_name");
            entity.HasIndex(e => e.ParkingName).HasDatabaseName("idx_parking_name");

            // Relationships
            entity.HasMany(e => e.CarEntries)
                .WithOne(ce => ce.Parking)
                .HasForeignKey(ce => ce.ParkingId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ========================================
        // PRQ_CarEntry Configuration
        // ========================================
        modelBuilder.Entity<CarEntry>(entity =>
        {
            // Table name mapping
            entity.ToTable("PRQ_CarEntry");

            // Primary Key
            entity.HasKey(e => e.Consecutive);
            entity.Property(e => e.Consecutive)
                .HasColumnType("bigint")
                .ValueGeneratedOnAdd();

            // Foreign Keys
            entity.Property(e => e.ParkingId)
                .HasColumnName("parking_id")
                .HasColumnType("bigint")
                .IsRequired();

            entity.Property(e => e.AutomobileId)
                .HasColumnName("automobile_id")
                .HasColumnType("bigint")
                .IsRequired();

            // Column configurations
            entity.Property(e => e.EntryDateTime)
                .HasColumnName("entry_datetime")
                .HasColumnType("datetime")
                .IsRequired()
                .HasComment("Vehicle entry timestamp");

            entity.Property(e => e.ExitDateTime)
                .HasColumnName("exit_datetime")
                .HasColumnType("datetime")
                .HasComment("Vehicle exit timestamp (NULL if still parked)");

            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Record creation timestamp");

            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at")
                .HasColumnType("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Last update timestamp");

            // Foreign Key Relationships
            entity.HasOne(e => e.Parking)
                .WithMany(p => p.CarEntries)
                .HasForeignKey(e => e.ParkingId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Automobile)
                .WithMany(a => a.CarEntries)
                .HasForeignKey(e => e.AutomobileId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes for query performance
            entity.HasIndex(e => e.ParkingId).HasDatabaseName("idx_parking_id");
            entity.HasIndex(e => e.AutomobileId).HasDatabaseName("idx_automobile_id");
            entity.HasIndex(e => e.EntryDateTime).HasDatabaseName("idx_entry_datetime");
            entity.HasIndex(e => e.ExitDateTime).HasDatabaseName("idx_exit_datetime");
            entity.HasIndex(e => new { e.AutomobileId, e.ParkingId })
                .HasDatabaseName("idx_vehicle_parking");
            entity.HasIndex(e => new { e.EntryDateTime, e.ExitDateTime })
                .HasDatabaseName("idx_entry_exit_composite");
        });
    }

    // ========================================
    // Helper Methods
    // ========================================

    /// <summary>
    /// Gets all currently parked vehicles (vehicles with no exit datetime).
    /// </summary>
    /// <returns>IQueryable of currently parked vehicles.</returns>
    public IQueryable<CarEntry> GetCurrentlyParkedVehicles()
    {
        return CarEntries.Where(ce => ce.ExitDateTime == null);
    }

    /// <summary>
    /// Gets all completed parking sessions (vehicles that have exited).
    /// </summary>
    /// <returns>IQueryable of completed parking sessions.</returns>
    public IQueryable<CarEntry> GetCompletedSessions()
    {
        return CarEntries.Where(ce => ce.ExitDateTime != null);
    }

    /// <summary>
    /// Gets all parking sessions for a specific parking lot on a given date.
    /// </summary>
    /// <param name="parkingId">The ID of the parking lot.</param>
    /// <param name="date">The date to query.</param>
    /// <returns>IQueryable of parking sessions for the specified parking lot and date.</returns>
    public IQueryable<CarEntry> GetSessionsByParkingAndDate(long parkingId, DateTime date)
    {
        var nextDay = date.AddDays(1);
        return CarEntries
            .Where(ce => ce.ParkingId == parkingId &&
                        ce.EntryDateTime >= date &&
                        ce.EntryDateTime < nextDay)
            .Include(ce => ce.Parking)
            .Include(ce => ce.Automobile);
    }

    /// <summary>
    /// Gets the revenue generated for a specific parking lot.
    /// Only counts completed sessions.
    /// </summary>
    /// <param name="parkingId">The ID of the parking lot.</param>
    /// <returns>The total revenue as a decimal.</returns>
    public decimal GetParkingRevenue(long parkingId)
    {
        return CarEntries
            .Where(ce => ce.ParkingId == parkingId && ce.ExitDateTime != null)
            .AsEnumerable() // Switch to LINQ to Objects because TotalAmountToPay is calculated
            .Sum(ce => ce.TotalAmountToPay ?? 0);
    }
}
