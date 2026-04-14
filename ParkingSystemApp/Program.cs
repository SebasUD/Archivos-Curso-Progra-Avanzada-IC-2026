using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using ParkingSystemApp.Configuration;
using ParkingSystemApp.Data;

// ========================================
// Parking System Application
// MySQL 8.0 Cloud Connection Setup
// ========================================

// Build configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets<Program>(optional: true) // Load User Secrets if available
    .AddEnvironmentVariables()
    .Build();

// Setup dependency injection
var services = new ServiceCollection();

// Add logging
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.AddConfiguration(configuration.GetSection("Logging"));
});

// Add DbContext with MySQL connection
var connectionStringHelper = new ConnectionStringHelper(configuration);
var connectionString = connectionStringHelper.GetMySqlConnectionString();

Console.WriteLine("========================================");
Console.WriteLine("Parking System - Database Connection Test");
Console.WriteLine("========================================\n");

services.AddDbContext<ParkingSystemDbContext>(options =>
{
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString),
        mysqlOptions =>
        {
            mysqlOptions.CharSetBehavior(CharSetBehavior.NeverAppend);
        });
});

// Add configuration and helpers to DI container
services.AddSingleton(configuration);
services.AddSingleton<ConnectionStringHelper>();

// Build service provider
var serviceProvider = services.BuildServiceProvider();

try
{
    // Get DbContext and test connection
    using (var context = serviceProvider.GetRequiredService<ParkingSystemDbContext>())
    {
        Console.WriteLine("Testing database connection...\n");
        
        // Test connection by checking if database is accessible
        bool canConnect = await context.Database.CanConnectAsync();
        
        if (canConnect)
        {
            Console.WriteLine("✓ Database connection successful!\n");

            // Display database tables
            Console.WriteLine("Tables in the database:");
            Console.WriteLine("- PRQ_Automobiles");
            Console.WriteLine("- PRQ_Parking");
            Console.WriteLine("- PRQ_CarEntry\n");

            // Query sample data
            await QuerySampleData(context);
        }
        else
        {
            Console.WriteLine("✗ Failed to connect to database.\n");
            Console.WriteLine("Please verify:");
            Console.WriteLine("1. Your cloud MySQL instance is running");
            Console.WriteLine("2. Connection credentials are correct");
            Console.WriteLine("3. Network allows connection to your cloud host");
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"✗ Error: {ex.Message}\n");
    Console.WriteLine("Troubleshooting steps:");
    Console.WriteLine("1. Check your User Secrets configuration: dotnet user-secrets list");
    Console.WriteLine("2. Verify appsettings.json has valid connection values");
    Console.WriteLine("3. Ensure database tables exist (run design-bd.sql first)");
    Console.WriteLine($"\nDetails: {ex}\n");
}
finally
{
    Console.WriteLine("========================================");
    Console.WriteLine("Connection test completed.");
    Console.WriteLine("========================================");
}

// ========================================
// Helper method for querying sample data
// ========================================
async Task QuerySampleData(ParkingSystemDbContext context)
{
    Console.WriteLine("Sample Data Query Results:\n");

    // Query automobiles
    var automobiles = await context.Automobiles.ToListAsync();
    Console.WriteLine($"Total Automobiles: {automobiles.Count}");
    foreach (var auto in automobiles.Take(3))
    {
        Console.WriteLine($"  - {auto.Manufacturer} {auto.Color} ({auto.Type}) - Year {auto.Year}");
    }
    if (automobiles.Count > 3)
        Console.WriteLine($"  ... and {automobiles.Count - 3} more\n");
    else
        Console.WriteLine();

    // Query parking lots
    var parkings = await context.ParkingLots.ToListAsync();
    Console.WriteLine($"Total Parking Lots: {parkings.Count}");
    foreach (var parking in parkings.Take(3))
    {
        Console.WriteLine($"  - {parking.ParkingName} ({parking.ProvinceName}) - ${parking.PricePerHour}/hour");
    }
    if (parkings.Count > 3)
        Console.WriteLine($"  ... and {parkings.Count - 3} more\n");
    else
        Console.WriteLine();

    // Query car entries
    var entries = await context.CarEntries.Include(e => e.Automobile).Include(e => e.Parking).ToListAsync();
    Console.WriteLine($"Total Parking Sessions: {entries.Count}");
    
    var currentlyParked = entries.Where(e => e.IsCurrentlyParked).ToList();
    var completed = entries.Where(e => e.IsSessionCompleted).ToList();
    
    Console.WriteLine($"  - Currently Parked: {currentlyParked.Count}");
    Console.WriteLine($"  - Completed Sessions: {completed.Count}\n");

    // Show some completed session details with computed properties
    Console.WriteLine("Sample Completed Sessions with Calculations:");
    foreach (var entry in completed.Take(3))
    {
        Console.WriteLine($"  Vehicle: {entry.Automobile.Manufacturer} {entry.Automobile.Color}");
        Console.WriteLine($"  Parking: {entry.Parking.ParkingName}");
        Console.WriteLine($"  Stay: {entry.StayDurationMinutes} minutes ({entry.StayDurationHours:F2} hours)");
        Console.WriteLine($"  Amount to Pay: ${entry.TotalAmountToPay:F2}\n");
    }
}
