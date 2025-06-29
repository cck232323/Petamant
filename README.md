# Pet Activity Social Platform

A comprehensive web application for pet owners to create, discover, and join pet-related activities and events.

## Overview

Pet Activity Social Platform is a .NET 8.0 web application that allows users to create accounts, organize pet activities, register for events, and build a community of pet enthusiasts. The platform provides an intuitive interface for managing activities and connecting with other pet owners.

## Features

- User Authentication: Secure JWT-based authentication system
- Activity Management: Create, view, update, and delete pet activities
- Registration System: Register for activities with pet information
- User Profiles: View user profiles and their created activities
- Responsive Design: Mobile-friendly interface using Bootstrap 4

## Technology Stack

**Backend:**

- ASP.NET Core 8.0
- Entity Framework Core 8.0
- PostgreSQL Database
- JWT Authentication

**Frontend:**

- Razor Views
- JavaScript/jQuery
- Bootstrap 4
- AJAX for asynchronous operations

**Tools & Libraries:**

- AutoMapper for object mapping
- FluentValidation for input validation
- Swagger for API documentation
- BCrypt for password hashing

## Project Structure

```
MyDotnetApp/
├── Controllers/           # API and MVC controllers
├── Models/                # Domain models
├── DTOs/                  # Data transfer objects
├── Views/                 # Razor views
│   ├── Home/              # Main application views
│   ├── Users/             # User-related views
│   └── Shared/            # Shared layouts and partials
├── wwwroot/               # Static files
│   ├── js/                # JavaScript files
│   │   ├── activities/    # Activity-related scripts
│   │   └── common/        # Shared utilities
│   ├── css/               # Stylesheets
│   └── images/            # Image assets
├── Dockerfile             # Docker configuration
├── docker-compose.yml     # Docker Compose configuration
└── Program.cs             # Application entry point and configuration
```

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- PostgreSQL database
- Visual Studio 2022 or Visual Studio Code
- Docker (optional, for containerized deployment)

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/cck232323/Petamant
   cd Petamant
   ```
2. Update the database connection string in appsettings.json:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Database=petactivities;Username=youruser;Password=yourpassword"
   }
   ```
3. Apply database migrations:
   ```bash
   dotnet ef database update
   ```
4. Run the application:
   ```bash
   dotnet run
   ```
5. Access the application at [https://localhost:5001] or [http://localhost:5000]

## API Documentation

The API documentation is available via Swagger UI at /swagger when running the application in development mode.

### Key API Endpoints

- GET /api/activities - Get all activities
- GET /api/activities/{id} - Get activity details
- POST /api/activities - Create a new activity
- DELETE /api/activities/{id} - Delete an activity
- POST /api/registrations - Register for an activity
- GET /api/activities/user/{id}/created - Get activities created by a user
- GET /api/activities/user/{id}/registered - Get activities a user has registered for

## Authentication

The application uses JWT (JSON Web Tokens) for authentication. To access protected endpoints:

1. Register a new user account or login with existing credentials
2. Use the returned token in the Authorization header for subsequent requests:
   ```
   Authorization: Bearer {your_token}
   ```

## Deployment

### Hosting on IIS

1. Publish the application:
   ```bash
   dotnet publish -c Release
   ```
2. Deploy the published files to your IIS server
3. Configure the application pool to use .NET Core hosting

### Docker Deployment

The project includes Docker configuration for easy containerized deployment:

1. Build and run using Docker Compose (recommended):
   ```bash
   docker-compose up -d
   ```
   This will start both the web application and PostgreSQL database.
2. Or build and run just the application container:
   ```bash
   docker build -t pet-activity-platform .
   docker run -p 8081:80 pet-activity-platform
   ```
3. Access the application at [http://localhost:8081](http://localhost:8081)

## Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/your-feature-name`
3. Commit your changes: `git commit -m 'Add some feature'`
4. Push to the branch: `git push origin feature/your-feature-name`
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

1. Bootstrap team for the responsive UI framework
2. ASP.NET Core team for the excellent web framework
2. All contributors who have helped improve this

