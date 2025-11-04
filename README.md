# Test Function App

## 📖 Overview

Azure Function App triggered by Service Bus messages. Receives user data and saves it to Azure SQL Database.

## 🎯 Purpose

- Listens to Azure Service Bus queue (`userdata-queue`)
- Automatically triggered when new message arrives
- Deserializes user data from message
- Validates data before saving
- Saves to Azure SQL Database
- Provides logging for monitoring

## 🔗 Connections

- **Triggered by**: Azure Service Bus (queue: `userdata-queue`)
- **Writes to**: Azure SQL Database (table: `UserData`)
- **Called by**: Service Bus trigger (automatic)
- **Azure Services**: Azure Functions, Azure Service Bus, Azure SQL Database

## 🏗️ Technology Stack

- .NET 8.0 Isolated Worker Model
- Azure Functions v4
- Azure Service Bus SDK
- Microsoft.Data.SqlClient
- Application Insights for monitoring

## 🎯 Features

- ✅ Service Bus trigger (automatic processing)
- ✅ JSON message deserialization
- ✅ Data validation (name, age)
- ✅ SQL database insertion
- ✅ Error handling and logging
- ✅ Dead-letter queue for failed messages
- ✅ Retry logic built-in

## 📚 Complete System Documentation

See `SYSTEM_ARCHITECTURE.md` for complete system overview and how this app fits into the bigger picture.

## 🚀 Deployment

See `DEPLOYMENT.md` for step-by-step deployment instructions using Azure Portal UI.

## 🔧 How It Works

### Service Bus Trigger Flow:

1. Message arrives in `userdata-queue`
2. Function automatically triggers
3. Message body is deserialized to `UserDataMessage` object
4. Data is validated (name not empty, age 1-150)
5. Data is saved to SQL database
6. If error occurs, message retries automatically
7. After max retries, message moves to dead-letter queue

### Database Operation:

```sql
INSERT INTO UserData (Name, Age, CreatedAt)
VALUES (@Name, @Age, @CreatedAt)
```

## 🛠️ Local Development

### Prerequisites:

- .NET 8.0 SDK
- Azure Functions Core Tools
- Azurite (Azure Storage Emulator)
- Access to Azure Service Bus and SQL Database

### Steps:

1. Install dependencies:

```bash
dotnet restore
```

2. Update `local.settings.json`:

```json
{
  "Values": {
    "ServiceBusConnection": "your-connection-string",
    "SqlConnectionString": "your-sql-connection-string"
  }
}
```

3. Run locally:

```bash
func start
```

4. Send test message to Service Bus queue
5. Function will trigger and process message

## 📝 Environment Variables

Required in Azure Function App Configuration:

- `ServiceBusConnection` - Service Bus connection string
- `SqlConnectionString` - SQL Database connection string

## 📂 Project Structure

```
test-function-app/
├── Functions/
│   └── UserDataProcessor.cs    # Main function (Service Bus trigger)
├── Services/
│   └── DatabaseService.cs      # SQL database operations
├── Models/
│   └── UserDataMessage.cs      # Message data model
├── Program.cs                  # Host configuration
├── host.json                   # Function host settings
├── local.settings.json         # Local settings (not in git)
├── .github/workflows/          # GitHub Actions
├── DEPLOYMENT.md               # Deployment guide
├── SYSTEM_ARCHITECTURE.md      # Complete system docs
└── README.md                   # This file
```

## 🔍 Monitoring

**In Azure Portal:**

1. Function App → Functions → UserDataProcessor
2. Click **Monitor** tab
3. View execution history
4. Click on execution to see detailed logs

**What to look for:**

- ✅ Successful executions
- ⚠️ Failed executions
- 📊 Execution duration
- 📝 Log messages

## ⚠️ Error Handling

### Automatic Retries:

- Service Bus retries failed messages automatically
- Default: 10 retries with exponential backoff
- After max retries → Dead-letter queue

### Dead-Letter Queue:

- Messages that failed all retries
- Access: Service Bus → Queue → Dead-letter messages
- Investigate errors and reprocess if needed

## 💡 Database Table Schema

```sql
CREATE TABLE UserData (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Age INT NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    Email NVARCHAR(255) NULL
);
```

## 🔐 Security Notes

- Connection strings stored in Azure Function App settings (encrypted)
- SQL Database uses SQL authentication
- Firewall rules restrict database access
- Service Bus uses connection string authentication
- Never commit `local.settings.json` to git

## 📄 License

Internal use only
