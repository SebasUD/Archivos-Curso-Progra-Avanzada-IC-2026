# Quick Setup Script for Parking System Application (Windows PowerShell)
# Automatically configures User Secrets and runs the application

Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "Parking System Application - Quick Setup" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

# Check if .NET SDK is installed
try
{
    $dotnetVersion = dotnet --version 2>$null
    Write-Host "✓ .NET SDK found" -ForegroundColor Green
    Write-Host "  Version: $dotnetVersion" -ForegroundColor Gray
}
catch
{
    Write-Host "❌ .NET SDK not found. Please install .NET 8.0 SDK or later." -ForegroundColor Red
    Write-Host "Download from: https://dotnet.microsoft.com/download" -ForegroundColor Yellow
    exit 1
}
Write-Host ""

# Step 1: Restore packages
Write-Host "Step 1: Restoring NuGet packages..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0)
{
    Write-Host "❌ Failed to restore packages" -ForegroundColor Red
    exit 1
}
Write-Host "✓ Packages restored" -ForegroundColor Green
Write-Host ""

# Step 2: Initialize User Secrets
Write-Host "Step 2: Initializing User Secrets..." -ForegroundColor Yellow
dotnet user-secrets init --force 2>$null
Write-Host "✓ User Secrets initialized" -ForegroundColor Green
Write-Host ""

# Step 3: Prompt for connection details
Write-Host "Step 3: Configure MySQL Connection" -ForegroundColor Yellow
Write-Host "Please provide your cloud MySQL instance details:" -ForegroundColor Gray
Write-Host ""

$mysqlHost = Read-Host "MySQL Host (e.g., db.example.com)"
$mysqlPort = Read-Host "MySQL Port [3306]"
if ([string]::IsNullOrWhiteSpace($mysqlPort)) { $mysqlPort = "3306" }

$mysqlDb = Read-Host "Database Name [parking_system]"
if ([string]::IsNullOrWhiteSpace($mysqlDb)) { $mysqlDb = "parking_system" }

$mysqlUser = Read-Host "MySQL Username"
$mysqlPassword = Read-Host "MySQL Password" -AsSecureString
$plainPassword = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto([System.Runtime.InteropServices.Marshal]::SecureStringToCoTaskMemUnicode($mysqlPassword))

Write-Host ""

# Step 4: Set User Secrets
Write-Host "Step 4: Storing credentials in User Secrets..." -ForegroundColor Yellow
$connString = "Server=$mysqlHost;Port=$mysqlPort;Database=$mysqlDb;Uid=$mysqlUser;Pwd=$plainPassword;"

dotnet user-secrets set "ConnectionStrings:ParkingSystemDb" $connString
if ($LASTEXITCODE -ne 0)
{
    Write-Host "❌ Failed to store connection string" -ForegroundColor Red
    exit 1
}
Write-Host "✓ Connection string stored securely" -ForegroundColor Green
Write-Host ""

# Step 5: Test connection
Write-Host "Step 5: Testing database connection..." -ForegroundColor Yellow
Write-Host "Starting application..." -ForegroundColor Gray
Write-Host ""

dotnet run

Write-Host ""
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "Setup completed!" -ForegroundColor Green
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "To run the application again:" -ForegroundColor Gray
Write-Host "  cd ParkingSystemApp && dotnet run" -ForegroundColor Cyan
Write-Host ""
Write-Host "To view stored secrets:" -ForegroundColor Gray
Write-Host "  dotnet user-secrets list" -ForegroundColor Cyan
Write-Host ""
