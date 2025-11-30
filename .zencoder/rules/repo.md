---
description: Repository Information Overview
alwaysApply: true
---

# Avyyan Knitfab Backend Information

## Summary
A comprehensive ASP.NET Core 8.0 Web API for managing a textile/knitfab business with PostgreSQL database support. Features include Entity Framework Core, Repository Pattern, JWT Authentication, SignalR WebSockets for real-time chat and notifications.

## Structure
- **Controllers/**: API endpoints for machine management, chat, notifications, auth, etc.
- **Models/**: Entity models including MachineManager, User, ChatRoom, etc.
- **DTOs/**: Data transfer objects for API requests/responses
- **Data/**: Database context and configuration
- **Services/**: Business logic implementation
- **Repositories/**: Data access layer with Repository pattern
- **Interfaces/**: Service and repository contracts
- **Extensions/**: Service registration and configuration extensions
- **Middleware/**: Custom middleware for exception handling
- **Hubs/**: SignalR hubs for real-time communication

## Language & Runtime
**Language**: C#
**Version**: .NET 8.0
**Build System**: MSBuild
**Package Manager**: NuGet

## Dependencies
**Main Dependencies**:
- Microsoft.EntityFrameworkCore (9.0.8)
- Npgsql.EntityFrameworkCore.PostgreSQL (9.0.4)
- Microsoft.AspNetCore.Authentication.JwtBearer (8.0.8)
- Microsoft.AspNetCore.SignalR (1.2.0)
- AutoMapper.Extensions.Microsoft.DependencyInjection (12.0.1)
- FluentValidation.AspNetCore (11.3.1)
- BCrypt.Net-Next (4.0.3)

**Development Dependencies**:
- Microsoft.EntityFrameworkCore.Tools (9.0.8)
- Swashbuckle.AspNetCore (6.6.2)

## Build & Installation
```bash
dotnet restore
dotnet build
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```

## Database Configuration
**Provider**: PostgreSQL
**Connection String**: Host=localhost;Database=AvyyanKnitfab;Username=postgres;Password=system;Port=5432
**Migrations**: EF Core Code-First approach
**Seeding**: Development data seeding via DataSeedService

## Logging
**Framework**: Serilog
**Sinks**: Console and File
**Configuration**: JSON structured logging with daily rotation
**Log Files**:
- General: logs/avyyan-knitfab-YYYYMMDD.log
- Errors: logs/avyyan-knitfab-errors-YYYYMMDD.log
- Development: logs/dev/avyyan-knitfab-dev-YYYYMMDD.log

## Authentication
**Method**: JWT Bearer tokens
**Configuration**: JwtSettings in appsettings.json
**Password Hashing**: BCrypt
**Token Expiration**: 60 minutes

## Real-time Features
**Technology**: SignalR
**Hubs**:
- /hubs/chat - Database-backed chat
- /hubs/notifications - Database-backed notifications
- /hubs/simple-chat - In-memory chat
- /hubs/simple-notifications - In-memory notifications

## API Endpoints
**Machine Management**: CRUD operations for machine inventory
**Chat**: Room creation, messaging, user search
**Notifications**: User notifications with read status
**Authentication**: Login, register, token refresh
**Users**: User management and profiles
**Roles**: Role-based access control

## Development Notes
- Repository pattern with Unit of Work for data access
- Soft delete pattern (IsActive flag) for entities
- CORS configured for frontend integration
- Swagger UI available at root URL in development
- Environment-specific configuration via appsettings.json