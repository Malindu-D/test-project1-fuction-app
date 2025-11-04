using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace TestFunctionApp.Services;

/// <summary>
/// Service for interacting with Azure SQL Database
/// </summary>
public class DatabaseService
{
    private readonly ILogger _logger;
    private readonly string _connectionString;

    public DatabaseService(ILogger logger)
    {
        _logger = logger;
        
        // Get connection string from environment variable
        _connectionString = Environment.GetEnvironmentVariable("SqlConnectionString") 
            ?? throw new InvalidOperationException("SqlConnectionString environment variable is not set");
    }

    /// <summary>
    /// Save user data to the database
    /// </summary>
    public async Task SaveUserDataAsync(string name, int age)
    {
        const string insertQuery = @"
            INSERT INTO UserData (Name, Age, CreatedAt)
            VALUES (@Name, @Age, @CreatedAt)";

        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = new SqlCommand(insertQuery, connection);
            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@Age", age);
            command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

            var rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected > 0)
            {
                _logger.LogInformation("Successfully inserted user data into database");
            }
            else
            {
                _logger.LogWarning("No rows were inserted into database");
            }
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Database error while saving user data");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while saving user data");
            throw;
        }
    }

    /// <summary>
    /// Test database connection
    /// </summary>
    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            _logger.LogInformation("Database connection test successful");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database connection test failed");
            return false;
        }
    }
}
