# MyNewMvcProject Development Instructions

This is an ASP.NET Core MVC application built with .NET 10.0.

## Project Overview

- **Type**: ASP.NET Core MVC Web Application
- **Framework**: .NET 10.0
- **Language**: C#

## Project Structure

- `Controllers/InitialController.cs` - Main controller with Index, About, and HelloWorld actions
- `Views/Initial/` - Views for each controller action
- `Views/Shared/` - Shared layout and view imports
- `Program.cs` - Application startup configuration

## Running the Application

```bash
dotnet run
```

The application will start on `https://localhost:5001`

## Features

1. **Index View** - Displays current date/time in 24-hour format
2. **About View** - Shows static application information
3. **HelloWorld View** - Form that captures name and displays greeting

## Development Notes

- Uses Bootstrap 5 for styling
- Configured with default MVC routing
- All views use the shared layout template
