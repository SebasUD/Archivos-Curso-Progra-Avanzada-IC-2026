# Parking System Application

A comprehensive C# Console Application for managing a MySQL 8.0 cloud database with Entity Framework Core.

## Features

✅ **Full Entity Framework Core Setup**
- Auto-generated entity classes for PRQ_Automobiles, PRQ_Parking, PRQ_CarEntry
- Strongly-typed DbContext with configurations
- Proper relationships and constraints

✅ **Computed Properties in CarEntry**
- `StayDurationMinutes` - Total parking duration in minutes
- `StayDurationHours` - Total parking duration in hours
- `TotalAmountToPay` - Amount to charge (rounded up to nearest hour)
- `IsCurrentlyParked` - Indicates if vehicle is still parked
- `IsSessionCompleted` - Indicates if session is finished
- `SessionStatus` - Formatted status description

✅ **Secure Credential Management**
- User Secrets for development
- Configuration-based connection strings
- Support for environment variables

✅ **Database Helpers**
- Get currently parked vehicles
- Get completed parking sessions
- Query by parking lot and date
- Calculate parking revenue

✅ **MySQL 8.0 Optimized**
- Pomelo.EntityFrameworkCore.MySql provider
- Proper collation: utf8mb4_0900_ai_ci
- Full constraint enforcement
- Optimized indexes

## Quick Start

### 1. Restore packages
```bash
cd ParkingSystemApp
dotnet restore
```

### 2. Configure User Secrets
```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:ParkingSystemDb" "Server=your-host;Port=3306;Database=parking_system;Uid=your-user;Pwd=your-password;"
```

### 3. Run the application
```bash
dotnet run
```

## Project Structure

```
ParkingSystemApp/
├── Models/
│   ├── Automobile.cs           # Vehicle entity with navigation property
│   ├── Parking.cs              # Parking lot entity
│   └── CarEntry.cs             # Parking session with computed properties
├── Data/
│   └── ParkingSystemDbContext.cs  # EF Core context & helper methods
├── Configuration/
│   └── ConnectionStringHelper.cs  # Connection management
├── Program.cs                  # Main entry point
├── appsettings.json            # Configuration template
├── appsettings.Development.json # Development overrides
├── SETUP_INSTRUCTIONS.md       # Detailed setup guide
└── README.md                   # This file
```

## Computed Properties Details

### StayDurationMinutes
```csharp
public int? StayDurationMinutes
{
    get
    {
        if (ExitDateTime == null) return null;
        TimeSpan duration = ExitDateTime.Value - EntryDateTime;
        return (int)duration.TotalMinutes;
    }
}
```

### StayDurationHours
```csharp
public decimal? StayDurationHours
{
    get
    {
        if (ExitDateTime == null) return null;
        TimeSpan duration = ExitDateTime.Value - EntryDateTime;
        return (decimal)duration.TotalHours;
    }
}
```

### TotalAmountToPay
```csharp
public decimal? TotalAmountToPay
{
    get
    {
        if (ExitDateTime == null) return null;
        TimeSpan duration = ExitDateTime.Value - EntryDateTime;
        int hoursToCharge = (int)Math.Ceiling(duration.TotalHours);
        decimal totalAmount = hoursToCharge * Parking.PricePerHour;
        return totalAmount;
    }
}
```

**Hourly Rounding Example:**
- 0.5 hours → 1 hour to charge
- 1.0 hours → 1 hour to charge
- 1.5 hours → 2 hours to charge
- 2.25 hours → 3 hours to charge

## Code Examples

### Query All Parking Sessions
```csharp
using (var context = serviceProvider.GetRequiredService<ParkingSystemDbContext>())
{
    var allSessions = await context.CarEntries
        .Include(e => e.Parking)
        .Include(e => e.Automobile)
        .ToListAsync();
        
    foreach (var session in allSessions)
    {
        Console.WriteLine($"{session.Automobile.Manufacturer}: {session.SessionStatus}");
    }
}
```

### Calculate Revenue for a Parking Lot
```csharp
var revenue = context.GetParkingRevenue(parkingId: 1);
Console.WriteLine($"Total Revenue: ${revenue:F2}");
```

### Find Currently Parked Vehicles
```csharp
var parked = await context.GetCurrentlyParkedVehicles()
    .Include(e => e.Automobile)
    .ToListAsync();

Console.WriteLine($"Vehicles currently parked: {parked.Count}");
```

### Query Sessions by Date
```csharp
var today = DateTime.Today;
var todaysSessions = context.GetSessionsByParkingAndDate(parkingId: 1, today);
```

## Database Schema

### PRQ_Automobiles
| Column | Type | Notes |
|--------|------|-------|
| id | BIGINT (PK) | Auto-increment |
| color | VARCHAR(50) | Not null |
| year | INT | Range: 1900-2100 |
| manufacturer | VARCHAR(100) | Not null |
| type | ENUM | Values: sedan, 4x4, motorcycle |
| created_at | TIMESTAMP | Default: CURRENT_TIMESTAMP |
| updated_at | TIMESTAMP | Auto-updated |

### PRQ_Parking
| Column | Type | Notes |
|--------|------|-------|
| id | BIGINT (PK) | Auto-increment |
| province_name | VARCHAR(100) | Not null, unique with parking_name |
| parking_name | VARCHAR(150) | Not null |
| price_per_hour | DECIMAL(10,2) | Must be positive |
| created_at | TIMESTAMP | Default: CURRENT_TIMESTAMP |
| updated_at | TIMESTAMP | Auto-updated |

### PRQ_CarEntry
| Column | Type | Notes |
|--------|------|-------|
| consecutive | BIGINT (PK) | Auto-increment |
| parking_id | BIGINT (FK) | References PRQ_Parking |
| automobile_id | BIGINT (FK) | References PRQ_Automobiles |
| entry_datetime | DATETIME | Not null |
| exit_datetime | DATETIME | NULL if still parked |
| created_at | TIMESTAMP | Default: CURRENT_TIMESTAMP |
| updated_at | TIMESTAMP | Auto-updated |

## Requirements

- **.NET 10.0 SDK** or later
- **MySQL 8.0** cloud instance
- Valid database credentials
- Network access to MySQL host

## NuGet Dependencies

- `Pomelo.EntityFrameworkCore.MySql` (8.0.2) - MySQL provider
- `Microsoft.EntityFrameworkCore.Design` (8.0.0) - EF Core tools
- `Microsoft.Extensions.Configuration` (8.0.0) - Configuration
- `Microsoft.Extensions.DependencyInjection` (8.0.0) - DI container
- `Microsoft.Extensions.Logging.Console` (8.0.0) - Logging

## Troubleshooting

**Connection Failed?**
1. Check User Secrets: `dotnet user-secrets list`
2. Verify host and port are accessible
3. Confirm credentials are correct
4. Ensure database exists: `CREATE DATABASE parking_system;`

**Tables Don't Exist?**
1. Run design-bd.sql to create schema
2. Run insertar-registros.sql for sample data

**EF Core Tools Missing?**
```bash
dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design
```

## Additional Resources

- [SETUP_INSTRUCTIONS.md](SETUP_INSTRUCTIONS.md) - Detailed setup guide
- [.NET Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [Pomelo MySQL Provider](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql)

## License

This project is part of the Advanced Programming course at Universidad Autónoma de México (UAM).

## Support

For issues or questions, refer to SETUP_INSTRUCTIONS.md or check inline code comments.

---

**Created:** April 13, 2026
**Version:** 1.0
**Target Framework:** .NET 10.0
**Database:** MySQL 8.0
