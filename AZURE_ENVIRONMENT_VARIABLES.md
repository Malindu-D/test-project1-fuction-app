# 🔧 Test Function App - Required Environment Variables

## 📋 Environment Variables You MUST Configure in Azure Function App

Your `test-function-app` requires **2 critical environment variables** to work. Without these, the function will fail.

---

## ⚙️ How to Configure

1. **Go to Azure Portal** → https://portal.azure.com
2. **Search for** "Function Apps"
3. **Click on** your deployed Function App (e.g., `test-function-app` or similar)
4. In the left menu, click **"Configuration"** or **"Environment variables"**
5. Under **"Application settings"**, click **"+ New application setting"**
6. Add each variable below
7. Click **"Save"** at the top
8. Click **"Continue"** to restart the app

---

## 🔑 Required Environment Variables

### **1. ServiceBusConnection** (CRITICAL!)

**Purpose:** Allows the function to listen to the Service Bus queue for incoming messages

**Name (exact):**

```
ServiceBusConnection
```

**Value:** Your Service Bus connection string

**How to get it:**

1. Go to Azure Portal → Search for **"Service Bus"**
2. Click on your Service Bus namespace (e.g., `userdata-servicebus-xyz`)
3. In the left menu, click **"Shared access policies"**
4. Click **"RootManageSharedAccessKey"**
5. Click the **Copy** button next to **"Primary Connection String"**

**Format looks like:**

```
Endpoint=sb://userdata-servicebus-xyz.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=abc123xyz456==
```

**⚠️ IMPORTANT:**

- Must start with `Endpoint=sb://`
- Must include `SharedAccessKeyName` and `SharedAccessKey`
- Copy the ENTIRE string (it's very long)
- Do NOT include quotes
- This is the SAME connection string used in your API Service App

---

### **2. SqlConnectionString** (CRITICAL!)

**Purpose:** Allows the function to save data to Azure SQL Database

**Name (exact):**

```
SqlConnectionString
```

**Value:** Your Azure SQL Database connection string with password

**How to get it:**

1. Go to Azure Portal → Search for **"SQL databases"**
2. Click on your database (e.g., `userdata-db`)
3. In the left menu, click **"Connection strings"**
4. Copy the **ADO.NET (SQL authentication)** connection string
5. **IMPORTANT:** Replace `{your_password}` with your actual SQL admin password!

**Format looks like:**

```
Server=tcp:userdata-sql-server-xyz.database.windows.net,1433;Initial Catalog=userdata-db;Persist Security Info=False;User ID=sqladmin;Password=YourActualPassword123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

**⚠️ IMPORTANT:**

- Must include the actual password (NOT `{your_password}`)
- Server name must be correct
- Database name must be `userdata-db` (or your actual database name)
- User ID must match your SQL admin username
- Do NOT include quotes

---

## ✅ Configuration Summary

After adding both settings, your Function App Configuration should look like:

| Name                   | Value                                                                                                        |
| ---------------------- | ------------------------------------------------------------------------------------------------------------ |
| `ServiceBusConnection` | `Endpoint=sb://your-servicebus.servicebus.windows.net/;SharedAccessKeyName=...`                              |
| `SqlConnectionString`  | `Server=tcp:your-server.database.windows.net,1433;Initial Catalog=userdata-db;User ID=sqladmin;Password=...` |

**Other built-in settings (already there, don't touch):**

- `AzureWebJobsStorage` - Auto-configured
- `FUNCTIONS_WORKER_RUNTIME` - Should be `dotnet-isolated`
- `FUNCTIONS_EXTENSION_VERSION` - Should be `~4`

---

## 🔍 How the Function Uses These Variables

### **ServiceBusConnection:**

The function has a Service Bus trigger:

```csharp
[ServiceBusTrigger("userdata-queue", Connection = "ServiceBusConnection")]
```

- Listens to queue: `userdata-queue`
- Uses connection string from environment variable: `ServiceBusConnection`
- When a message arrives, function automatically runs

### **SqlConnectionString:**

The DatabaseService reads this variable:

```csharp
_connectionString = Environment.GetEnvironmentVariable("SqlConnectionString")
```

- Opens connection to Azure SQL Database
- Inserts user data into `UserData` table
- If variable is missing, function will crash with error

---

## 🧪 Testing After Configuration

### **1. Verify Settings Are Saved**

1. Go to Function App → Configuration
2. Check both variables exist
3. Click on each to verify values (don't show passwords to others!)

### **2. Restart the Function App**

1. Go to Function App → Overview
2. Click **"Restart"** at the top
3. Wait 1-2 minutes

### **3. Check Function is Running**

1. Go to Function App → Functions (left menu)
2. You should see: `UserDataProcessor`
3. Status should be **"Enabled"**

### **4. Test with Real Data**

1. Open your Name-Age App
2. Fill in name and age
3. Submit data
4. Wait 10-30 seconds

### **5. Verify Function Executed**

1. Go to Function App → Functions → UserDataProcessor
2. Click **"Monitor"** tab
3. You should see a recent execution
4. Click on it to see logs
5. Look for: **"Successfully processed and saved user data"**

### **6. Verify Data in Database**

1. Go to Azure SQL Database → Query editor
2. Login with your SQL credentials
3. Run query:
   ```sql
   SELECT * FROM UserData ORDER BY CreatedAt DESC;
   ```
4. You should see your test data!

---

## 🆘 Troubleshooting

### **Problem: "ServiceBusConnection environment variable is not set"**

**Cause:** Variable name is wrong or not saved  
**Solution:**

- Variable name MUST be exactly: `ServiceBusConnection` (case-sensitive)
- Click Save after adding
- Restart Function App

### **Problem: "SqlConnectionString environment variable is not set"**

**Cause:** Variable name is wrong or not saved  
**Solution:**

- Variable name MUST be exactly: `SqlConnectionString` (case-sensitive)
- Click Save after adding
- Restart Function App

### **Problem: Function runs but gets SQL connection error**

**Cause:** Connection string is wrong or password is incorrect  
**Solution:**

- Verify password in connection string is correct
- Check firewall allows Azure services
- Test connection in SQL Query editor first

### **Problem: Function not triggered**

**Cause:** Service Bus connection is wrong or queue name mismatch  
**Solution:**

- Verify connection string is complete
- Check queue exists and is named `userdata-queue`
- Check Service Bus has messages (Active messages count)

### **Problem: "Cannot open server" or "Login failed"**

**Cause:** SQL Server firewall blocking Azure Functions  
**Solution:**

1. Go to SQL Server (not database) → Networking
2. Check: **"Allow Azure services and resources to access this server"** ✅
3. Save

---

## 📊 Quick Checklist

- [ ] Function App deployed successfully
- [ ] `ServiceBusConnection` configured in Function App
- [ ] `SqlConnectionString` configured in Function App
- [ ] Both settings saved and Function App restarted
- [ ] SQL Database table `UserData` exists
- [ ] SQL Server firewall allows Azure services
- [ ] Service Bus queue `userdata-queue` exists
- [ ] Test data submitted from Name-Age App
- [ ] Function executed successfully (check Monitor)
- [ ] Data appears in SQL Database

---

## 🎯 Complete System Flow

```
┌─────────────────┐
│  Name-Age App   │ User submits name & age
│ (Static Web App)│
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ API Service App │ Receives data, sends to Service Bus
│  (App Service)  │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  Service Bus    │ Queue: userdata-queue
│   (Messaging)   │
└────────┬────────┘
         │ Triggers automatically
         ▼
┌─────────────────┐
│ Function App    │ ← NEEDS ServiceBusConnection ✅
│  (Serverless)   │ ← NEEDS SqlConnectionString ✅
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  SQL Database   │ Stores user data
│   (userdata-db) │
└─────────────────┘
```

---

## 🔗 Related Resources

- **Service Bus Queue Name:** `userdata-queue` (configured in trigger)
- **SQL Table Name:** `UserData` (created via database-setup.sql)
- **Function Name:** `UserDataProcessor`
- **Runtime:** .NET 8 Isolated

---

**Last Updated:** November 4, 2025  
**Status:** Configuration guide for deployed Function App  
**Next Step:** Configure the 2 environment variables in Azure Portal!
