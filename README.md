# SQL Server Stored Procedure Demo with C# and Docker (macOS)

## Prerequisites Installation

### 1. Install Docker Desktop
```bash
# Download from:
https://www.docker.com/products/docker-desktop

# Verify installation:
docker --version
```

### 2. Install Azure Data Studio
```bash
# Download from:
https://learn.microsoft.com/en-us/sql/azure-data-studio/download-azure-data-studio
```

## SQL Server Setup

### 1. Start SQL Server Container
```bash
docker run -d \
  --platform linux/amd64 \
  --name sql_server \
  -e "ACCEPT_EULA=Y" \
  -e "SA_PASSWORD=Your@StrongPass123" \
  -p 1433:1433 \
  mcr.microsoft.com/mssql/server:2022-latest
```

### 2. Verify Container Status
```bash
docker ps | grep sql_server
```

## Database Setup

### 1. Connect in Azure Data Studio
- Server: `localhost,1433`
- Auth: `SQL Login`
- Username: `sa`
- Password: `Your@StrongPass123!`
- Database: `master`
- ✔️ Trust Server Certificate

### 2. Run SQL Script
```sql
CREATE DATABASE UserDB;
GO

USE UserDB;
GO

CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    UserName NVARCHAR(50),
    Email NVARCHAR(100)
);
GO

CREATE PROCEDURE InsertUser
    @UserName NVARCHAR(50),
    @Email NVARCHAR(100),
    @NewUserID INT OUTPUT
AS
BEGIN
    INSERT INTO Users (UserName, Email)
    VALUES (@UserName, @Email);
    
    SET @NewUserID = SCOPE_IDENTITY();
END
GO
```

## C# Application Setup

### 1. Create Project
```bash
dotnet new console -n StoredProcDemo
cd StoredProcDemo
dotnet add package Microsoft.Data.SqlClient
```

### 2. Program.cs
```csharp
using System;
using Microsoft.Data.SqlClient;

class Program
{
    static void Main()
    {
        const string connectionString = "Server=localhost,1433;Database=UserDB;User Id=sa;Password=Your@StrongPass123!;Integrated Security=False;TrustServerCertificate=True;";

        try
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            
            using var command = new SqlCommand("InsertUser", connection)
            {
                CommandType = System.Data.CommandType.StoredProcedure
            };

            command.Parameters.AddWithValue("@UserName", "JohnDoe");
            command.Parameters.AddWithValue("@Email", "john@example.com");
            
            var outputParam = new SqlParameter("@NewUserID", System.Data.SqlDbType.Int)
            {
                Direction = System.Data.ParameterDirection.Output
            };
            command.Parameters.Add(outputParam);
            
            command.ExecuteNonQuery();
            Console.WriteLine($"New User ID: {outputParam.Value}");
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"SQL Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
```

## Run the Application
```bash
dotnet run
# Expected output: "New User ID: 1"
```

## Verification
```sql
-- Run in Azure Data Studio
USE UserDB;
SELECT * FROM Users;
```

## Cleanup
```bash
docker stop sql_server
docker rm sql_server
```

## Troubleshooting
1. **Password Requirements**:
   - 8+ characters with uppercase, lowercase, number, and symbol
   - Example: `Your@StrongPass123!`

2. **Connection Issues**:
   ```bash
   telnet localhost 1433  # Verify port access
   docker logs sql_server # Check container logs
   ```

3. **Apple Silicon Notes**:
   - Always include `--platform linux/amd64`
   - Allocate ≥4GB RAM in Docker Desktop