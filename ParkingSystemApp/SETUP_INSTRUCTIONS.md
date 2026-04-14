# Parking System Application - Setup Instructions

## Overview
This C# Console Application connects to a MySQL 8.0 cloud database instance and provides Entity Framework Core models for the Parking System.

## Project Structure
```
ParkingSystemApp/
├── Models/                           # Entity classes
│   ├── Automobile.cs                 # PRQ_Automobiles entity
│   ├── Parking.cs                    # PRQ_Parking entity
│   └── CarEntry.cs                   # PRQ_CarEntry entity (with computed properties)
├── Data/
│   └── ParkingSystemDbContext.cs     # Entity Framework DbContext
├── Configuration/
│   └── ConnectionStringHelper.cs     # Connection string management
├── Program.cs                        # Main application entry point
├── appsettings.json                  # Configuration template
├── appsettings.Development.json      # Development overrides (optional)
└── ParkingSystemApp.csproj          # Project file with NuGet dependencies
```

## Prerequisites
- .NET 10.0 SDK or later installed
- Access to a MySQL 8.0 cloud instance
- Username and password for the MySQL instance
- Database and tables created (run design-bd.sql and insertar-registros.sql first)

## Step 1: Restore NuGet Packages

Navigate to the project directory and restore packages:
```bash
cd ParkingSystemApp
dotnet restore
```

**Packages installed:**
- `Pomelo.EntityFrameworkCore.MySql` - MySQL provider for EF Core
- `Microsoft.EntityFrameworkCore.Design` - EF Core database tools
- `Microsoft.Extensions.Configuration*` - Configuration management
- `Microsoft.Extensions.DependencyInjection` - Dependency injection
- `Microsoft.Extensions.Logging*` - Logging framework

## Step 2: Configure User Secrets (Recommended - Secure)

User Secrets are the recommended way to store sensitive credentials during development.

### Step 2a: Initialize User Secrets (One-time setup)
```bash
dotnet user-secrets init
```

This creates a secrets.json file in your user profile folder (not in the project).

### Step 2b: Set MySQL Credentials
Replace the values with your actual MySQL cloud credentials:

```bash
# Connection string method (simplest)
dotnet user-secrets set "ConnectionStrings:ParkingSystemDb" "Server=your-cloud-host;Port=3306;Database=parking_system;Uid=your-username;Pwd=your-password;"

# OR: Individual credential method
dotnet user-secrets set "MySql:Host" "your-cloud-host"
dotnet user-secrets set "MySql:Port" "3306"
dotnet user-secrets set "MySql:Database" "parking_system"
dotnet user-secrets set "MySql:Username" "your-username"
dotnet user-secrets set "MySql:Password" "your-password"
```

### Step 2c: Verify Secrets
List all configured secrets:
```bash
dotnet user-secrets list
```

### Step 2d: Clear Secrets (if needed)
```bash
dotnet user-secrets clear
dotnet user-secrets init
```

## Step 3: Configure appsettings.json (Alternative - Less Secure)

If you prefer NOT to use User Secrets, configure appsettings.json directly:

Edit `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "ParkingSystemDb": "Server=your-cloud-host;Port=3306;Database=parking_system;Uid=your-username;Pwd=your-password;"
  }
}
```

**Note:** Never commit appsettings.json with real credentials to version control!

## Step 4: Verify Database Tables Exist

Before running the application, ensure the database tables are created:

1. Connect to your MySQL cloud instance:
```bash
mysql -h your-cloud-host -u your-username -p
```

2. Select your database:
```sql
USE parking_system;
```

3. Check tables:
```sql
SHOW TABLES;
```

Expected tables:
- `PRQ_Automobiles`
- `PRQ_Parking`
- `PRQ_CarEntry`

If tables don't exist, execute the SQL scripts:
```bash
mysql -h your-cloud-host -u your-username -p parking_system < design-bd.sql
mysql -h your-cloud-host -u your-username -p parking_system < insertar-registros.sql
```

## Step 5: Run the Application

From the ParkingSystemApp directory:

```bash
dotnet run
```

### Expected Output
```
========================================
Parking System - Database Connection Test
========================================

Testing database connection...

✓ Database connection successful!

Tables in the database:
- PRQ_Automobiles
- PRQ_Parking
- PRQ_CarEntry

Sample Data Query Results:

Total Automobiles: 8
  - Toyota Red (sedan) - Year 2020
  - Honda Black (sedan) - Year 2019
  - Ford Silver (4x4) - Year 2021

Total Parking Lots: 4
  - Parking Central Downtown (San José) - $2.50/hour
  - Alajuela Mall Parking (Alajuela) - $2.00/hour
  ...

Total Parking Sessions: 15
  - Currently Parked: 4
  - Completed Sessions: 11

Sample Completed Sessions with Calculations:
  Vehicle: Toyota Red
  Parking: Parking Central Downtown
  Stay: 150 minutes (2.50 hours)
  Amount to Pay: $7.50
  ...

========================================
Connection test completed.
========================================
```

## Computed Properties in CarEntry

The `CarEntry` entity includes the following computed properties:

### 1. StayDurationMinutes
- **Type:** `int?` (nullable)
- **Returns:** Total stay duration in minutes
- **Formula:** `(ExitDateTime - EntryDateTime).TotalMinutes`
- **Returns NULL if:** Vehicle is still parked (ExitDateTime is NULL)
- **Example:** 150 minutes

### 2. StayDurationHours
- **Type:** `decimal?` (nullable)
- **Returns:** Total stay duration in hours (with decimals)
- **Formula:** `(ExitDateTime - EntryDateTime).TotalHours`
- **Returns NULL if:** Vehicle is still parked (ExitDateTime is NULL)
- **Example:** 2.50 hours

### 3. TotalAmountToPay
- **Type:** `decimal?` (nullable)
- **Returns:** Total amount to charge for parking
- **Formula:** `Math.Ceiling(StayDurationHours) * Parking.PricePerHour`
- **Behavior:** Rounds UP to the next whole hour
  - 2.0 hours = 2 hours to charge
  - 2.5 hours = 3 hours to charge
- **Returns NULL if:** Vehicle is still parked (ExitDateTime is NULL)
- **Example:** $7.50 (3 hours × $2.50/hour)

### 4. IsCurrentlyParked (Helper)
- **Type:** `bool`
- **Returns:** `true` if ExitDateTime is NULL; `false` otherwise
- **Usage:** Check if vehicle is still in parking

### 5. IsSessionCompleted (Helper)
- **Type:** `bool`
- **Returns:** `true` if ExitDateTime is not NULL; `false` otherwise
- **Usage:** Verify if parking session has ended

### 6. SessionStatus (Helper)
- **Type:** `string`
- **Returns:** Formatted status description
- **Example:** "Currently parked" or "Completed: 2 hours 30 minutes"

## Example Usage in Code

```csharp
using (var context = serviceProvider.GetRequiredService<ParkingSystemDbContext>())
{
    // Get all completed sessions
    var completedSessions = context.CarEntries
        .Where(ce => ce.ExitDateTime != null)
        .Include(ce => ce.Parking)
        .Include(ce => ce.Automobile)
        .ToList();

    foreach (var session in completedSessions)
    {
        Console.WriteLine($"Vehicle: {session.Automobile.Manufacturer}");
        Console.WriteLine($"Parking: {session.Parking.ParkingName}");
        Console.WriteLine($"Duration: {session.StayDurationMinutes} minutes");
        Console.WriteLine($"Hours: {session.StayDurationHours:F2}");
        Console.WriteLine($"Amount to Pay: ${session.TotalAmountToPay:F2}");
        Console.WriteLine();
    }

    // Get currently parked vehicles
    var currentlyParked = context.GetCurrentlyParkedVehicles()
        .Include(ce => ce.Parking)
        .Include(ce => ce.Automobile)
        .ToList();

    // Calculate total revenue
    var totalRevenue = context.GetParkingRevenue(parkingId: 1);
    Console.WriteLine($"Revenue for Parking 1: ${totalRevenue:F2}");
}
```

## Troubleshooting

### Connection Error: "Unable to connect to MySQL"
- **Issue:** Incorrect host, port, or credentials
- **Solution:**
  1. Verify credentials in User Secrets: `dotnet user-secrets list`
  2. Test connection manually: `mysql -h your-host -u your-user -p`
  3. Check if MySQL port is open (default: 3306)
  4. Verify firewall allows connection from your IP

### Error: "Unknown database 'parking_system'"
- **Issue:** Database doesn't exist on the MySQL server
- **Solution:**
  1. Create the database: `CREATE DATABASE parking_system;`
  2. Run setup scripts (design-bd.sql, insertar-registros.sql)

### Error: "Table 'PRQ_Automobiles' doesn't exist"
- **Issue:** Tables not created yet
- **Solution:**
  1. Execute design-bd.sql to create schema
  2. Execute insertar-registros.sql to insert sample data

### Error: "Access denied for user"
- **Issue:** Wrong username or password
- **Solution:**
  1. Verify credentials with your cloud MySQL provider
  2. Reset User Secrets and reconfigure
  3. Check user permissions in MySQL: `SHOW GRANTS FOR 'username'@'%';`

### DbContext Not Working
- **Issue:** EntityFrameworkCore design tools not found
- **Solution:**
  ```bash
  dotnet add package Microsoft.EntityFrameworkCore.Design
  dotnet tool install --global dotnet-ef
  ```

## Additional Commands

### List Entity Framework Models
```bash
dotnet ef dbcontext info
```

### Generate a New Migration (if schema changes)
```bash
dotnet ef migrations add InitialCreate
```

### Apply Migrations to Database
```bash
dotnet ef database update
```

### Scaffold from Existing Database
If you want to regenerate models from the database:
```bash
dotnet ef dbcontext scaffold "Server=host;Port=3306;Database=parking_system;Uid=user;Pwd=password;" Pomelo.EntityFrameworkCore.MySql --tables PRQ_Automobiles,PRQ_Parking,PRQ_CarEntry
```

## Security Best Practices

1. **Never commit credentials** - Use User Secrets for development
2. **Use environment variables** in production - Set via cloud provider settings
3. **Restrict database user permissions** - Only grant necessary privileges
4. **Enable SSL/TLS** - Use encrypted connections to MySQL
5. **Rotate passwords regularly** - Update credentials periodically
6. **Audit access logs** - Monitor who accesses your database

## Additional Resources

- [Entity Framework Core Documentation](https://learn.microsoft.com/en-us/ef/core/)
- [Pomelo MySQL Provider](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql)
- [.NET User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)
- [MySQL 8.0 Documentation](https://dev.mysql.com/doc/refman/8.0/en/)

## Contact & Support

For issues or questions about this application, refer to the troubleshooting section or check the inline code comments in the entity classes and DbContext.
