using Microsoft.EntityFrameworkCore;
using ParkingSystemApp.Configuration;
using ParkingSystemApp.Data;
using ParkingSystemApp.Repositories.Implementations;
using ParkingSystemApp.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>(optional: true);

builder.Services.AddControllersWithViews();

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
});

var connectionStringHelper = new ConnectionStringHelper(builder.Configuration);
string connectionString;

try
{
    connectionString = connectionStringHelper.GetMySqlConnectionString();
    Console.WriteLine("MySQL connection string loaded successfully.");
}
catch (InvalidOperationException ex)
{
    Console.WriteLine("\n⚠️ WARNING: MySQL Connection String Not Configured!");
    Console.WriteLine(ex.Message);
    throw;
}

builder.Services.AddDbContext<ParkingSystemDbContext>(options =>
{
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 0)),
        mysqlOptions =>
        {
            mysqlOptions.EnableRetryOnFailure();
        });
});

builder.Services.AddScoped<IAutomobileRepository, AutomobileRepository>();
builder.Services.AddScoped<IParkingRepository, ParkingRepository>();
builder.Services.AddScoped<ICarEntryRepository, CarEntryRepository>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=AutomobilesMasterDetail}/{action=Index}/{id?}");
app.MapControllers();

logger.LogInformation("========================================");
logger.LogInformation("Parking System API - Starting Application");
logger.LogInformation("========================================");
logger.LogInformation($"Environment: {app.Environment.EnvironmentName}");
logger.LogInformation($"Database: {connectionStringHelper.GetDatabaseName()}");
logger.LogInformation("========================================");

app.Run("http://localhost:5000");