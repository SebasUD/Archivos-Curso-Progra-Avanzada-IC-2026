#!/bin/bash
# Quick Setup Script for Parking System Application
# Automatically configures User Secrets and runs the application

echo "=========================================="
echo "Parking System Application - Quick Setup"
echo "=========================================="
echo ""

# Check if .NET SDK is installed
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET SDK not found. Please install .NET 8.0 SDK or later."
    echo "Download from: https://dotnet.microsoft.com/download"
    exit 1
fi

echo "✓ .NET SDK found"
DOTNET_VERSION=$(dotnet --version)
echo "  Version: $DOTNET_VERSION"
echo ""

# Step 1: Restore packages
echo "Step 1: Restoring NuGet packages..."
dotnet restore
if [ $? -ne 0 ]; then
    echo "❌ Failed to restore packages"
    exit 1
fi
echo "✓ Packages restored"
echo ""

# Step 2: Initialize User Secrets
echo "Step 2: Initializing User Secrets..."
dotnet user-secrets init --force 2>/dev/null
echo "✓ User Secrets initialized"
echo ""

# Step 3: Prompt for connection details
echo "Step 3: Configure MySQL Connection"
echo "Please provide your cloud MySQL instance details:"
echo ""

read -p "MySQL Host (e.g., db.example.com): " MYSQL_HOST
read -p "MySQL Port [3306]: " MYSQL_PORT
MYSQL_PORT=${MYSQL_PORT:-3306}

read -p "Database Name [parking_system]: " MYSQL_DB
MYSQL_DB=${MYSQL_DB:-parking_system}

read -p "MySQL Username: " MYSQL_USER
read -sp "MySQL Password: " MYSQL_PASSWORD
echo ""

# Step 4: Set User Secrets
echo ""
echo "Step 4: Storing credentials in User Secrets..."
CONN_STRING="Server=$MYSQL_HOST;Port=$MYSQL_PORT;Database=$MYSQL_DB;Uid=$MYSQL_USER;Pwd=$MYSQL_PASSWORD;"

dotnet user-secrets set "ConnectionStrings:ParkingSystemDb" "$CONN_STRING"
if [ $? -ne 0 ]; then
    echo "❌ Failed to store connection string"
    exit 1
fi
echo "✓ Connection string stored securely"
echo ""

# Step 5: Test connection
echo "Step 5: Testing database connection..."
echo "Starting application..."
echo ""

dotnet run

echo ""
echo "=========================================="
echo "Setup completed!"
echo "=========================================="
echo ""
echo "To run the application again:"
echo "  cd ParkingSystemApp && dotnet run"
echo ""
echo "To view stored secrets:"
echo "  dotnet user-secrets list"
echo ""
