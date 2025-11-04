# Test Function App - Deployment Guide

## 📋 Prerequisites

- Azure Account
- GitHub Account
- Git installed on your computer
- **Azure Service Bus must be created** (with `userdata-queue`)
- **Azure SQL Database must be created**

## 🚀 Step-by-Step Deployment

### Step 1: Create Azure SQL Database

#### 1.1 Create SQL Server

1. Go to [Azure Portal](https://portal.azure.com)
2. Click **"Create a resource"**
3. Search for **"SQL Database"** and click **Create**
4. Fill in the details:

   **Basics Tab:**

   - **Subscription**: Select your subscription
   - **Resource Group**: Use same as other apps (e.g., `user-data-system-rg`)
   - **Database name**: `userdata-db`
   - **Server**: Click **"Create new"**

   **Create SQL Server:**

   - **Server name**: Enter unique name (e.g., `userdata-sql-server-xyz`)
   - **Location**: Same region as other resources
   - **Authentication method**: Select **"Use SQL authentication"**
   - **Server admin login**: `sqladmin` (or your choice)
   - **Password**: Create a strong password (save it somewhere safe!)
   - Click **OK**

   **Compute + storage:**

   - Click **"Configure database"**
   - Select **"Basic"** (5 DTUs, 2GB storage) - Cheapest option
   - Click **Apply**

5. Click **Review + Create**, then **Create**
6. Wait for deployment to complete (3-5 minutes)

#### 1.2 Configure Firewall Rules

1. Go to your SQL Server resource (not the database, the server)
2. Click **"Networking"** in left menu
3. Under **Firewall rules**:
   - Check **"Allow Azure services and resources to access this server"** ✅
   - Click **"Add your client IP address"** (so you can access from your computer)
4. Click **Save** at the top

#### 1.3 Create Database Table

1. Go to your SQL Database resource (`userdata-db`)
2. Click **"Query editor"** in left menu
3. Login with:
   - **Login**: `sqladmin` (or what you set)
   - **Password**: Your password
4. In the query window, paste this SQL:

```sql
CREATE TABLE UserData (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Age INT NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    Email NVARCHAR(255) NULL
);
```

5. Click **Run** button
6. You should see: "Query succeeded: Affected rows: 0"
7. Verify by running: `SELECT * FROM UserData;`

#### 1.4 Get Database Connection String

1. Still in your SQL Database resource
2. Click **"Connection strings"** in left menu
3. Copy the **ADO.NET** connection string
4. It looks like:

```
Server=tcp:userdata-sql-server-xyz.database.windows.net,1433;Initial Catalog=userdata-db;Persist Security Info=False;User ID=sqladmin;Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

5. Replace `{your_password}` with your actual password
6. **Save this connection string somewhere safe!**

### Step 2: Create Azure Function App

1. In Azure Portal, click **"Create a resource"**
2. Search for **"Function App"** and click **Create**
3. Fill in the details:

   **Basics Tab:**

   - **Subscription**: Select your subscription
   - **Resource Group**: Use same (e.g., `user-data-system-rg`)
   - **Function App name**: Enter unique name (e.g., `test-function-app-xyz123`)
   - **Publish**: Select **Code**
   - **Runtime stack**: Select **.NET**
   - **Version**: Select **8 (LTS), isolated worker model**
   - **Region**: Same region as other resources
   - **Operating System**: **Windows** (recommended)

   **Storage:**

   - **Storage account**: Create new or use existing

   **Plan:**

   - **Plan type**: Select **Consumption (Serverless)** - Pay only when function runs!

4. Click **Review + Create**, then **Create**
5. Wait for deployment to complete (2-3 minutes)

### Step 3: Push Code to GitHub

1. Open Terminal/PowerShell in your `test-function-app` folder
2. Run these commands:

```powershell
git init
git add .
git commit -m "Initial commit - Test Function App"
```

3. Go to [GitHub](https://github.com) and create a **new repository**
   - Name: `test-function-app`
   - Make it **Private** or **Public**
   - **Don't** initialize with README
   - Click **Create repository**
4. Copy the commands shown and run them:

```powershell
git remote add origin https://github.com/YOUR-USERNAME/test-function-app.git
git branch -M main
git push -u origin main
```

### Step 4: Configure GitHub Secrets

#### 4.1 Get Function App Publish Profile

1. Go to Azure Portal → Your Function App
2. Click **"Get publish profile"** at the top (Download button)
3. A file will download - open it with Notepad
4. **Copy all the content**

#### 4.2 Add Secrets to GitHub

1. Go to your GitHub repository
2. Click **Settings** → **Secrets and variables** → **Actions**
3. Click **"New repository secret"**

**First Secret:**

- **Name**: `AZURE_FUNCTIONAPP_PUBLISH_PROFILE`
- **Value**: Paste the publish profile content
- Click **Add secret**

**Second Secret:**

- **Name**: `AZURE_FUNCTIONAPP_NAME`
- **Value**: Your function app name (e.g., `test-function-app-xyz123`)
- Click **Add secret**

### Step 5: Configure Function App Settings

1. Go to Azure Portal → Your Function App
2. Click **"Environment variables"** in left menu (or **Configuration**)
3. Under **Application settings**, click **"+ New application setting"**

**First Setting - Service Bus:**

- **Name**: `ServiceBusConnection`
- **Value**: Your Service Bus connection string (from API Service deployment)
  - Example: `Endpoint=sb://userdata-servicebus-xyz.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=your-key`
- Click **OK**

**Second Setting - SQL Database:**

- **Name**: `SqlConnectionString`
- **Value**: Your SQL connection string (from Step 1.4)
  - Make sure password is included!
- Click **OK**

4. Click **Save** at the top
5. Click **Continue** when prompted
6. Wait for app to restart

### Step 6: Deploy Using GitHub Actions

1. Update the workflow file in your repository
2. Open `.github/workflows/azure-functions-deploy.yml`
3. You can keep it as is - the secrets will work

4. Push to trigger deployment:

```powershell
git add .
git commit -m "Ready for deployment"
git push
```

5. Go to GitHub repository → **Actions** tab
6. Watch the workflow run
7. Wait for green checkmark ✅ (takes 3-5 minutes)

### Step 7: Verify Function is Running

1. Go to Azure Portal → Your Function App
2. Click **"Functions"** in left menu
3. You should see: `UserDataProcessor`
4. Click on it
5. Click **"Monitor"** tab
6. You'll see invocations when messages arrive

### Step 8: Test the Complete Flow

**Now test the entire system:**

1. Open your **Name-Age App** (deployed Static Web App)
2. Make sure API endpoint is configured
3. Test API connection (should be green)
4. Fill in name and age
5. Click **Submit Data**
6. Wait for success message

**Check if it worked:**

**Option A: Check Function Logs**

1. Go to Function App → Functions → UserDataProcessor
2. Click **Monitor** tab
3. You should see a recent execution
4. Click on it to see logs
5. Logs should show: "Successfully processed and saved user data"

**Option B: Check Database**

1. Go to SQL Database → Query editor
2. Login with your credentials
3. Run: `SELECT * FROM UserData;`
4. You should see your data!

**Option C: Check Service Bus Queue**

1. Go to Service Bus → Queues → userdata-queue
2. Check **"Active messages"** count
3. Should be 0 (function processed them)
4. If messages are stuck, check function logs for errors

## ✅ Success!

Your Function App is now running and processing messages!

## 📝 Important Information to Save

- **Function App Name**: `test-function-app-xyz123`
- **SQL Server**: `userdata-sql-server-xyz.database.windows.net`
- **SQL Database**: `userdata-db`
- **SQL Admin**: `sqladmin`
- **SQL Password**: (your password)
- **Service Bus Queue**: `userdata-queue`

## 🔄 How It Works

```
1. User submits data in Name-Age App
2. Name-Age App → API Service App
3. API Service App → Service Bus Queue
4. Service Bus triggers Function App automatically
5. Function App reads message
6. Function App saves to SQL Database
7. Done! ✅
```

## 🔄 Future Updates

To update the function after code changes:

```powershell
git add .
git commit -m "Your change description"
git push
```

GitHub Actions will automatically deploy!

## ⚠️ Troubleshooting

**Problem**: Function not appearing in Azure

- **Solution**: Check GitHub Actions - deployment might have failed
- **Solution**: Wait 5 minutes after deployment

**Problem**: Function runs but data not in database

- **Solution**: Check SQL connection string in Function App settings
- **Solution**: Verify firewall rules allow Azure services
- **Solution**: Check function logs for SQL errors

**Problem**: Messages stuck in Service Bus queue

- **Solution**: Check if `ServiceBusConnection` setting is correct
- **Solution**: Verify queue name is exactly `userdata-queue`
- **Solution**: Check function logs for errors

**Problem**: Database connection error

- **Solution**: Verify firewall rule allows Azure services
- **Solution**: Check connection string has correct password
- **Solution**: Test connection from Query editor first

**Problem**: Function shows errors in logs

- **Solution**: Check both connection strings are set correctly
- **Solution**: Verify table `UserData` exists in database
- **Solution**: Check Service Bus queue exists

## 💡 Pro Tips

**Tip 1: Monitor Function Executions**

- Function App → Monitor → See all executions
- Check if function is being triggered
- View logs for each execution

**Tip 2: Test Messages**

- Service Bus → Queues → userdata-queue
- Click "Service Bus Explorer"
- Send test message manually
- See if function processes it

**Tip 3: Query Database Anytime**

- SQL Database → Query editor
- No need to install SQL Server Management Studio
- Run queries directly in browser

## 🔗 System Status

After this deployment:

1. ✅ API Service App - Deployed
2. ✅ Name-Age App - Deployed
3. ✅ Test Function App - Deployed (This one!)
4. ⏳ Email Notification App - Deploy later
5. ⏳ Spring Email Service - Deploy last

**The data collection flow is now complete! 🎉**

## 📞 Testing Everything Together

1. Submit data in Name-Age App
2. Check Function Monitor → Should show execution
3. Check Database → Data should be there
4. Check Service Bus → Queue should be empty (processed)

If all 4 checks pass = Everything works! ✅
