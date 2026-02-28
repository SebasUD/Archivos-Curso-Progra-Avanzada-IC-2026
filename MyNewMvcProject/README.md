# MyNewMvcProject

An ASP.NET Core MVC project built with .NET 10.0.

## Features

- **InitialController**: Base controller with three actions:
  - **Index**: Displays the current date and time in 24-hour format (yyyy-MM-dd HH:mm:ss)
  - **About**: Shows static information about the application
  - **HelloWorld**: Interactive form that captures a user's name and displays a personalized greeting

- **Responsive UI**: Built with Bootstrap 5 for a clean, responsive interface
- **Navigation**: Navbar for easy navigation between views

## Getting Started

### Prerequisites
- .NET 10.0 SDK or later

### Running the Application

1. Navigate to the project directory:
   ```bash
   cd MyNewMvcProject
   ```

2. Build the project:
   ```bash
   dotnet build
   ```

3. Run the application:
   ```bash
   dotnet run
   ```

4. Open your browser and navigate to:
   ```
   https://localhost:5001
   ```

## Project Structure

```
MyNewMvcProject/
├── Controllers/
│   └── InitialController.cs
├── Views/
│   ├── Initial/
│   │   ├── Index.cshtml
│   │   ├── About.cshtml
│   │   └── HelloWorld.cshtml
│   └── Shared/
│       ├── _Layout.cshtml
│       ├── _ViewImports.cshtml
│       └── _ViewStart.cshtml
├── Program.cs
└── MyNewMvcProject.csproj
```

## Views

### Index View
Displays the current system date and time in 24-hour format.

### About View
Shows static text describing the application and its features.

### HelloWorld View
- Contains a form that accepts a user's name
- On submission, displays a personalized "Hello World" greeting
- Uses Bootstrap for styling
