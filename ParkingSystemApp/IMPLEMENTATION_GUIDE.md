# C# Application Implementation - Complete Overview

## Project Summary

A comprehensive C# Console Application connecting to a MySQL 8.0 cloud database with Entity Framework Core, featuring secure credential management via User Secrets and advanced computed properties for parking fee calculations.

---

## What Was Created

### 1. **Entity Classes** (Models)

#### **Automobile.cs**
- Represents vehicles (PRQ_Automobiles table)
- Properties: color, year, manufacturer, type (enum: sedan, 4x4, motorcycle)
- Navigation property: CarEntries collection

#### **Parking.cs**
- Represents parking lots (PRQ_Parking table)
- Properties: province_name, parking_name, price_per_hour
- Navigation property: CarEntries collection

#### **CarEntry.cs** ⭐ (With Computed Properties)
- Represents parking sessions (PRQ_CarEntry table)
- Core properties: parking_id, automobile_id, entry_datetime, exit_datetime
- **Computed Properties:**
  - `StayDurationMinutes` - Minutes parked (NULL if still parked)
  - `StayDurationHours` - Hours parked as decimal (NULL if still parked)
  - `TotalAmountToPay` - Fee amount with hour rounding (NULL if still parked)
  - `IsCurrentlyParked` - Boolean: still parked?
  - `IsSessionCompleted` - Boolean: session ended?
  - `SessionStatus` - Formatted description

### 2. **Database Context** (Data Access)

#### **ParkingSystemDbContext.cs**
- Entity Framework Core DbContext for MySQL 8.0
- DbSets: Automobiles, ParkingLots, CarEntries
- Fluent API configuration for all entities
- Helper methods:
  - `GetCurrentlyParkedVehicles()` - Query active sessions
  - `GetCompletedSessions()` - Query finished sessions
  - `GetSessionsByParkingAndDate()` - Query by location and date
  - `GetParkingRevenue()` - Calculate parking lot revenue

### 3. **Configuration & Infrastructure**

#### **ConnectionStringHelper.cs**
- Build MySQL connection strings
- Read credentials from User Secrets
- Support for environment variables
- Connection string validation

#### **appsettings.json**
- Configuration template with placeholders
- Structure for MySQL connection parameters

#### **appsettings.Development.json**
- Development-specific settings
- Default localhost configuration for testing

### 4. **Main Application**

#### **Program.cs**
- Dependency injection setup
- DbContext registration with Pomelo.EntityFrameworkCore.MySql
- Connection testing with sample data queries
- Error handling with troubleshooting guidance
- Displays computed property examples

### 5. **Setup Scripts**

#### **setup.sh** (Linux/macOS)
- Interactive User Secrets configuration
- Package restoration
- Database connection testing

#### **setup.ps1** (Windows PowerShell)
- Windows-friendly setup script
- Secure password input
- Colored console output

### 6. **Documentation**

#### **README.md**
- Quick start guide
- Feature overview
- Code examples
- Troubleshooting tips

#### **SETUP_INSTRUCTIONS.md**
- Comprehensive 7-step setup guide
- User Secrets configuration detailed walkthrough
- Database configuration
- Troubleshooting section
- Security best practices

#### **UsageExamples.cs**
- 7 real-world code examples:
  1. Query completed sessions with fees
  2. Find currently parked vehicles
  3. Calculate parking lot revenue
  4. Query sessions by date
  5. Advanced LINQ filtering
  6. Automobile statistics
  7. Monthly revenue reports

### 7. **Project Configuration**

#### **ParkingSystemApp.csproj**
- .NET 8.0 target framework
- NuGet dependencies:
  - Pomelo.EntityFrameworkCore.MySql
  - Microsoft.EntityFrameworkCore.Design
  - Microsoft.Extensions.* (Configuration, DI, Logging)
- User Secrets configuration

#### **.gitignore**
- Excludes sensitive files from version control
- Prevents accidental credential commits
- Standard .NET and IDE patterns

---

## Key Features

### ✅ **Computed Properties with NULL Handling**

All computed properties in CarEntry return NULL if the vehicle hasn't exited:

```csharp
// Example: StayDurationMinutes
public int? StayDurationMinutes
{
    get
    {
        if (ExitDateTime == null) return null; // Not yet exited
        TimeSpan duration = ExitDateTime.Value - EntryDateTime;
        return (int)duration.TotalMinutes;
    }
}
```

### ✅ **Hourly Billing with Ceiling Rounding**

The fee calculation rounds UP to the next whole hour:

```csharp
// Example: 2.5 hours = 3 hours to charge
int hoursToCharge = (int)Math.Ceiling(duration.TotalHours);
decimal totalAmount = hoursToCharge * Parking.PricePerHour;
```

### ✅ **Secure Credential Management**

Two approaches supported:
1. **User Secrets** (🔒 Recommended) - Credentials stored outside project
2. **appsettings.json** - Configuration file

### ✅ **MySQL 8.0 Optimized**

- Pomelo.EntityFrameworkCore.MySql provider
- Native MySQL 8.0 collation: utf8mb4_0900_ai_ci
- Full CHECK constraint enforcement
- Optimized indexes and relationships

### ✅ **Dependency Injection**

- ServiceCollection for DI container
- DbContext registration with custom options
- Logging configuration
- Configuration access throughout app

---

## Quick Start Commands

### Setup on Windows (PowerShell)
```powershell
cd ParkingSystemApp
.\setup.ps1
```

### Setup on Linux/macOS (Bash)
```bash
cd ParkingSystemApp
chmod +x setup.sh
./setup.sh
```

### Manual Setup
```bash
cd ParkingSystemApp
dotnet restore
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:ParkingSystemDb" "Server=your-host;Port=3306;Database=parking_system;Uid=user;Pwd=password;"
dotnet run
```

---

## Computed Properties Reference

| Property | Type | Description | Returns NULL When |
|----------|------|-------------|-------------------|
| `StayDurationMinutes` | `int?` | Duration in minutes | Vehicle still parked |
| `StayDurationHours` | `decimal?` | Duration in hours | Vehicle still parked |
| `TotalAmountToPay` | `decimal?` | Fee (rounded up hourly) | Vehicle still parked |
| `IsCurrentlyParked` | `bool` | Is vehicle active? | N/A (bool) |
| `IsSessionCompleted` | `bool` | Is session done? | N/A (bool) |
| `SessionStatus` | `string` | Formatted status | N/A (always returns) |

---

## Database Integration

### Tables Mapped
- **PRQ_Automobiles** → `Automobile` class
- **PRQ_Parking** → `Parking` class
- **PRQ_CarEntry** → `CarEntry` class

### Relationships
```
PRQ_Automobiles ←—— PRQ_CarEntry ——→ PRQ_Parking
     (1)              (Many)            (1)
```

### Foreign Key Constraints
- `CarEntry.automobile_id` → `Automobiles.id` (RESTRICT delete)
- `CarEntry.parking_id` → `Parking.id` (RESTRICT delete)

---

## File Structure

```
ParkingSystemApp/
├── Models/
│   ├── Automobile.cs              ✅ Entity model
│   ├── Parking.cs                 ✅ Entity model
│   └── CarEntry.cs                ✅ Entity with computed properties
│
├── Data/
│   └── ParkingSystemDbContext.cs  ✅ EF Core context
│
├── Configuration/
│   └── ConnectionStringHelper.cs  ✅ Connection management
│
├── Examples/
│   └── UsageExamples.cs           ✅ 7 code examples
│
├── Program.cs                     ✅ Main entry point
│
├── appsettings.json              ✅ Config template
├── appsettings.Development.json  ✅ Dev config
│
├── ParkingSystemApp.csproj       ✅ Project file
├── README.md                      ✅ Quick start
├── SETUP_INSTRUCTIONS.md         ✅ Detailed guide
├── .gitignore                     ✅ Git ignore rules
│
├── setup.sh                       ✅ Linux/macOS setup
└── setup.ps1                      ✅ Windows setup
```

---

## NuGet Dependencies

```
Pomelo.EntityFrameworkCore.MySql          v10.0.0  ← MySQL provider
Microsoft.EntityFrameworkCore.Design      v10.0.0  ← EF Core tools
Microsoft.Extensions.Configuration        v10.0.0  ← Config
Microsoft.Extensions.Configuration.Json   v10.0.0  ← JSON config
Microsoft.Extensions.Configuration.UserSecrets v10.0.0 ← Secrets
Microsoft.Extensions.DependencyInjection  v10.0.0  ← DI container
Microsoft.Extensions.Logging              v10.0.0  ← Logging
Microsoft.Extensions.Logging.Console      v10.0.0  ← Console log
```

---

## Configuration Priority

Credentials are loaded in this order:
1. **User Secrets** (highest priority - development)
2. **Environment Variables**
3. **appsettings.json** (lowest priority)

---

## Security Considerations

✅ **DO:**
- Use User Secrets for development
- Use environment variables in production
- Never commit appsettings with real credentials
- Rotate database passwords regularly
- Use HTTPS/SSL for connections

❌ **DON'T:**
- Store passwords in source code
- Commit credentials to git
- Use simple passwords
- Share credentials in plaintext
- Leave debug output in production

---

## Important Notes

### About Computed Properties

All NULL-returning computed properties are marked with `[NotMapped]` attribute, meaning they:
- ❌ Are NOT stored in the database
- ✅ Are calculated at runtime when accessed
- ✅ Return NULL for incomplete sessions (no exit datetime)
- ✅ Support LINQ to Objects but not database queries

### About Rounding

The `TotalAmountToPay` calculation uses `Math.Ceiling()`:
- 0.1 hours → 1 hour to charge
- 1.0 hours → 1 hour to charge
- 1.01 hours → 2 hours to charge

This ensures no partial-hour discounts.

### About Foreign Keys

Both foreign key relationships use `OnDelete(DeleteBehavior.Restrict)`:
- Prevents deletion of automobiles with active sessions
- Prevents deletion of parking lots with active sessions
- Maintains data integrity

---

## Next Steps

1. **Install .NET 8.0 SDK** if not already installed
2. **Run setup script** (setup.ps1 or setup.sh)
3. **Provide MySQL credentials** when prompted
4. **Test connection** - application will verify database
5. **Review UsageExamples.cs** for advanced usage
6. **Build your own queries** using the DbContext

---

## Troubleshooting Resources

- **SETUP_INSTRUCTIONS.md** - Comprehensive setup guide with 5+ solutions
- **README.md** - Quick reference and common issues
- **Inline Code Comments** - Detailed explanations in entity classes
- **UsageExamples.cs** - 7 working code examples

---

## Support & Questions

If you encounter issues:
1. Check **Troubleshooting section** in SETUP_INSTRUCTIONS.md
2. Review **inline code comments** for detailed explanations
3. Check **User Secrets:** `dotnet user-secrets list`
4. Verify **MySQL connection** manually
5. Ensure **database tables exist** (run design-bd.sql)

---

**Version:** 1.0  
**Created:** April 13, 2026  
**Target:** .NET 10.0 + MySQL 8.0  
**Author:** Advanced Programming Course - UAM
