using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace ParkingSystemApp.Models;

public partial class ParkingSystemDbContext : DbContext
{
    public ParkingSystemDbContext()
    {
    }

    public ParkingSystemDbContext(DbContextOptions<ParkingSystemDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<PrqAutomobile> PrqAutomobiles { get; set; }

    public virtual DbSet<PrqCarentry> PrqCarentries { get; set; }

    public virtual DbSet<PrqParking> PrqParkings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // NOTE: Connection string is configured via dependency injection in Program.cs
        // Do NOT add hardcoded connection strings here for security reasons
        // Use appsettings.json or User Secrets instead
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<PrqAutomobile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("prq_automobiles", tb => tb.HasComment("Automobiles table - MySQL 8.0 optimized"));

            entity.HasIndex(e => e.Manufacturer, "idx_manufacturer");

            entity.HasIndex(e => e.Type, "idx_type");

            entity.HasIndex(e => e.Year, "idx_year");

            entity.Property(e => e.Id)
                .HasComment("Unique identifier for each automobile")
                .HasColumnName("id");
            entity.Property(e => e.Color)
                .HasMaxLength(50)
                .HasComment("Vehicle color")
                .HasColumnName("color");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Record creation timestamp")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Manufacturer)
                .HasMaxLength(100)
                .HasComment("Vehicle manufacturer/brand name")
                .HasColumnName("manufacturer");
            entity.Property(e => e.Type)
                .HasComment("Vehicle type classification")
                .HasColumnType("enum('sedan','4x4','motorcycle')")
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Last update timestamp")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
            entity.Property(e => e.Year)
                .HasComment("Manufacturing year (e.g., 2020)")
                .HasColumnName("year");
        });

        modelBuilder.Entity<PrqCarentry>(entity =>
        {
            entity.HasKey(e => e.Consecutive).HasName("PRIMARY");

            entity.ToTable("prq_carentry", tb => tb.HasComment("Car entry/exit records table - MySQL 8.0 optimized"));

            entity.HasIndex(e => e.AutomobileId, "idx_automobile_id");

            entity.HasIndex(e => e.EntryDatetime, "idx_entry_datetime");

            entity.HasIndex(e => new { e.EntryDatetime, e.ExitDatetime }, "idx_entry_exit_composite");

            entity.HasIndex(e => e.ExitDatetime, "idx_exit_datetime");

            entity.HasIndex(e => e.ParkingId, "idx_parking_id");

            entity.HasIndex(e => new { e.AutomobileId, e.ParkingId }, "idx_vehicle_parking");

            entity.Property(e => e.Consecutive)
                .HasComment("Unique consecutive ID for each parking session")
                .HasColumnName("consecutive");
            entity.Property(e => e.AutomobileId)
                .HasComment("Reference to automobile (FK)")
                .HasColumnName("automobile_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Record creation timestamp")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.EntryDatetime)
                .HasComment("Date and time when vehicle entered the parking lot")
                .HasColumnType("datetime")
                .HasColumnName("entry_datetime");
            entity.Property(e => e.ExitDatetime)
                .HasComment("Date and time when vehicle exited (NULL if still parked)")
                .HasColumnType("datetime")
                .HasColumnName("exit_datetime");
            entity.Property(e => e.ParkingId)
                .HasComment("Reference to parking lot (FK)")
                .HasColumnName("parking_id");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Last update timestamp")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Automobile).WithMany(p => p.PrqCarentries)
                .HasForeignKey(d => d.AutomobileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_automobile_id");

            entity.HasOne(d => d.Parking).WithMany(p => p.PrqCarentries)
                .HasForeignKey(d => d.ParkingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_parking_id");
        });

        modelBuilder.Entity<PrqParking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("prq_parking", tb => tb.HasComment("Parking lots table - MySQL 8.0 optimized"));

            entity.HasIndex(e => e.ParkingName, "idx_parking_name");

            entity.HasIndex(e => e.ProvinceName, "idx_province_name");

            entity.HasIndex(e => new { e.ProvinceName, e.ParkingName }, "uk_parking_location").IsUnique();

            entity.Property(e => e.Id)
                .HasComment("Unique identifier for each parking lot")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Record creation timestamp")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.ParkingName)
                .HasMaxLength(150)
                .HasComment("Name/identifier of the parking lot")
                .HasColumnName("parking_name");
            entity.Property(e => e.PricePerHour)
                .HasPrecision(10, 2)
                .HasComment("Hourly parking rate in currency units")
                .HasColumnName("price_per_hour");
            entity.Property(e => e.ProvinceName)
                .HasMaxLength(100)
                .HasComment("Province/State name where parking is located")
                .HasColumnName("province_name");
            entity.Property(e => e.UpdatedAt)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasComment("Last update timestamp")
                .HasColumnType("timestamp")
                .HasColumnName("updated_at");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
