-- ============================================
-- Azure SQL Database Setup Script
-- User Data Management System
-- ============================================

-- Create UserData table
CREATE TABLE UserData (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Age INT NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    Email NVARCHAR(255) NULL
);

-- Add index for better query performance
CREATE INDEX IX_UserData_CreatedAt ON UserData(CreatedAt DESC);

-- Verify table creation
SELECT 
    TABLE_NAME,
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'UserData'
ORDER BY ORDINAL_POSITION;

-- Test query (should return empty result initially)
SELECT * FROM UserData;

-- Sample query to count records
SELECT COUNT(*) AS TotalRecords FROM UserData;

-- Sample query to get recent entries
SELECT TOP 10 
    Id,
    Name,
    Age,
    CreatedAt
FROM UserData
ORDER BY CreatedAt DESC;
