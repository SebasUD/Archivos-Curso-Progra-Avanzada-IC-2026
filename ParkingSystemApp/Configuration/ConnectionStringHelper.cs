using Microsoft.Extensions.Configuration;

namespace ParkingSystemApp.Configuration;

/// <summary>
/// Helper class for managing MySQL connection strings.
/// Supports building connection strings from user secrets and configuration files.
/// </summary>
public class ConnectionStringHelper
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Constructor to inject configuration.
    /// </summary>
    /// <param name="configuration">The configuration object from dependency injection.</param>
    public ConnectionStringHelper(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the MySQL connection string from User Secrets or configuration.
    /// Priority:
    /// 1. User Secrets (secure, development)
    /// 2. Environment variables
    /// 3. appsettings.json
    /// </summary>
    /// <returns>The MySQL connection string.</returns>
    /// <exception cref="InvalidOperationException">Thrown if connection string not found.</exception>
    public string GetMySqlConnectionString()
    {
        // Try to get connection string directly
        var connectionString = _configuration.GetConnectionString("ParkingSystemDb");
        
        if (!string.IsNullOrEmpty(connectionString))
            return connectionString;

        // Try to build from individual secret components
        var host = _configuration["MySql:Host"];
        var port = _configuration["MySql:Port"] ?? "3306";
        var database = _configuration["MySql:Database"];
        var username = _configuration["MySql:Username"];
        var password = _configuration["MySql:Password"];

        if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(database) || 
            string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            throw new InvalidOperationException(
                "MySQL connection string not found. Please configure User Secrets or appsettings.json. " +
                "See SETUP_INSTRUCTIONS.md for details.");
        }

        // Build connection string: Server=host;Port=port;Database=database;Uid=username;Pwd=password;
        return $"Server={host};Port={port};Database={database};Uid={username};Pwd={password};";
    }

    /// <summary>
    /// Builds a Pomelo MySQL connection string with the specified parameters.
    /// This method is useful for creating custom connection strings.
    /// </summary>
    /// <param name="host">The MySQL server hostname or IP address.</param>
    /// <param name="port">The MySQL server port (default: 3306).</param>
    /// <param name="database">The database name.</param>
    /// <param name="username">The username for authentication.</param>
    /// <param name="password">The password for authentication.</param>
    /// <returns>A properly formatted MySQL connection string.</returns>
    public static string BuildConnectionString(
        string host, 
        int port = 3306, 
        string? database = "parking_system",
        string? username = null, 
        string? password = null)
    {
        if (string.IsNullOrWhiteSpace(host))
            throw new ArgumentException("Host cannot be null or empty.", nameof(host));

        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be null or empty.", nameof(username));

        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be null or empty.", nameof(password));

        return $"Server={host};Port={port};Database={database};Uid={username};Pwd={password};";
    }

    /// <summary>
    /// Validates the connection string format.
    /// </summary>
    /// <param name="connectionString">The connection string to validate.</param>
    /// <returns>True if valid; false otherwise.</returns>
    public static bool IsValidConnectionString(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            return false;

        var requiredParts = new[] { "Server=", "Database=", "Uid=", "Pwd=" };
        return requiredParts.All(part => connectionString.Contains(part, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets the database name from the current MySQL connection string.
    /// </summary>
    /// <returns>The database name, or "Unknown" if not found.</returns>
    public string GetDatabaseName()
    {
        try
        {
            var connectionString = GetMySqlConnectionString();
            var parts = connectionString.Split(';');
            var databasePart = parts.FirstOrDefault(p => p.StartsWith("Database=", StringComparison.OrdinalIgnoreCase));
            
            if (databasePart != null)
            {
                return databasePart.Substring("Database=".Length);
            }

            return "Unknown";
        }
        catch
        {
            return "Unknown";
        }
    }
}
