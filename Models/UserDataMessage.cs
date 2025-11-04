namespace TestFunctionApp.Models;

/// <summary>
/// Model representing user data from Service Bus message
/// </summary>
public class UserDataMessage
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
}
