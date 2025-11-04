using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TestFunctionApp.Models;
using TestFunctionApp.Services;

namespace TestFunctionApp.Functions;

/// <summary>
/// Azure Function triggered by Service Bus queue messages
/// Receives user data and saves to Azure SQL Database
/// </summary>
public class UserDataProcessor
{
    private readonly ILogger<UserDataProcessor> _logger;
    private readonly DatabaseService _databaseService;

    public UserDataProcessor(ILogger<UserDataProcessor> logger)
    {
        _logger = logger;
        _databaseService = new DatabaseService(logger);
    }

    [Function("UserDataProcessor")]
    public async Task Run(
        [ServiceBusTrigger("userdata-queue", Connection = "ServiceBusConnection")]
        string messageBody,
        FunctionContext context)
    {
        try
        {
            _logger.LogInformation("Processing Service Bus message: {MessageBody}", messageBody);

            // Deserialize the message
            var userData = JsonSerializer.Deserialize<UserDataMessage>(messageBody);

            if (userData == null)
            {
                _logger.LogError("Failed to deserialize message");
                throw new InvalidOperationException("Invalid message format");
            }

            // Validate data
            if (string.IsNullOrWhiteSpace(userData.Name))
            {
                _logger.LogError("Name is empty or null");
                throw new ArgumentException("Name is required");
            }

            if (userData.Age < 1 || userData.Age > 150)
            {
                _logger.LogError("Age is out of valid range: {Age}", userData.Age);
                throw new ArgumentException("Age must be between 1 and 150");
            }

            // Save to database
            _logger.LogInformation("Saving user data to database: Name={Name}, Age={Age}", 
                userData.Name, userData.Age);

            await _databaseService.SaveUserDataAsync(userData.Name, userData.Age);

            _logger.LogInformation("Successfully processed and saved user data for: {Name}", 
                userData.Name);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing message");
            throw; // This will move message to dead-letter queue after retries
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing message");
            throw; // This will move message to dead-letter queue after retries
        }
    }
}
