# Avyyan Knitfab Backend API

A comprehensive ASP.NET Core 8.0 Web API for managing a textile/knitfab business with PostgreSQL database support.

## 🚀 Features

- **Entity Framework Core** with PostgreSQL
- **Repository Pattern** with Unit of Work
- **AutoMapper** for object mapping
- **JWT Authentication** ready
- **Swagger/OpenAPI** documentation
- **FluentValidation** for input validation
- **CORS** configuration
- **Serilog** structured logging with file and console sinks
- **Exception handling middleware**
- **Request/Response logging**
- **SignalR WebSockets** for real-time chat and notifications
- **Real-time messaging** with personal and group chat support
- **Live notifications** with unread counts and real-time delivery

## 📁 Project Structure

```
AvyyanBackend/
├── Controllers/           # API Controllers
├── Models/               # Entity Models
├── DTOs/                 # Data Transfer Objects
├── Data/                 # Database Context
├── Services/             # Business Logic Services
├── Repositories/         # Data Access Layer
├── Interfaces/           # Service Contracts
├── Extensions/           # Service Extensions & Mapping Profiles
├── Middleware/           # Custom Middleware (future)
└── Properties/           # Launch Settings
```

## 🗄️ Database Models

### Core Entities
- **MachineManager** - Machine management with specifications (dia, gg, needle, feeder, rpm, slit, constat, efficiency)
- **Category** - General categorization system
- **Customer** - Customer information
- **Address** - Customer addresses
- **Order** - Customer orders
- **OrderItem** - Order line items (with product name/SKU)
- **Supplier** - Supplier information
- **PurchaseOrder** - Purchase orders from suppliers
- **PurchaseOrderItem** - Purchase order line items (with product name/SKU)

### Chat & Communication Entities
- **User** - System users with authentication and profiles
- **ChatRoom** - Group and personal chat rooms
- **ChatMessage** - Messages with file attachments and replies
- **ChatRoomMember** - Room membership with roles and permissions
- **MessageReaction** - Emoji reactions to messages
- **Notification** - System notifications with real-time delivery
- **UserConnection** - WebSocket connection tracking

## 🔧 Setup Instructions

### Prerequisites
- .NET 8.0 SDK
- PostgreSQL 12+ 
- Visual Studio 2022 or VS Code

### Database Setup
1. Install PostgreSQL and create a database:
   ```sql
   CREATE DATABASE AvyyanKnitfab;
   CREATE DATABASE AvyyanKnitfab_Dev; -- for development
   ```

2. Update connection strings in `appsettings.json` and `appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=AvyyanKnitfab;Username=postgres;Password=your_password_here;Port=5432"
     }
   }
   ```

### Running the Application
1. Restore packages:
   ```bash
   dotnet restore
   ```

2. Create and run database migrations:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

3. Run the application:
   ```bash
   dotnet run
   ```

4. Access Swagger UI at: `https://localhost:7009` or `http://localhost:5133`

5. Test WebSocket functionality at: `https://localhost:7009/chat-test.html`

## 📦 Installed Packages

- **Microsoft.EntityFrameworkCore** (9.0.8) - ORM framework
- **Npgsql.EntityFrameworkCore.PostgreSQL** (9.0.4) - PostgreSQL provider
- **Microsoft.EntityFrameworkCore.Tools** (9.0.8) - EF Core CLI tools
- **Microsoft.AspNetCore.Authentication.JwtBearer** (8.0.8) - JWT authentication
- **AutoMapper.Extensions.Microsoft.DependencyInjection** (12.0.1) - Object mapping
- **FluentValidation.AspNetCore** (11.3.1) - Input validation
- **Serilog.AspNetCore** (9.0.0) - Structured logging framework
- **Serilog.Sinks.File** (7.0.0) - File logging sink
- **Serilog.Sinks.Console** (6.0.0) - Console logging sink
- **Serilog.Enrichers.Environment** (3.0.1) - Environment enrichers
- **Serilog.Enrichers.Process** (3.0.0) - Process enrichers
- **Serilog.Enrichers.Thread** (4.0.0) - Thread enrichers
- **Microsoft.AspNetCore.SignalR** (1.1.0) - Real-time WebSocket communication
- **Swashbuckle.AspNetCore** (6.6.2) - Swagger/OpenAPI

## 📊 Logging Configuration

### Serilog Features
- **Structured Logging** - JSON formatted logs for production
- **Multiple Sinks** - Console and file outputs
- **Log Rotation** - Daily rotation with size limits
- **Request Logging** - Automatic HTTP request/response logging
- **Exception Logging** - Comprehensive error tracking
- **Environment Enrichment** - Machine, process, and thread information

### Log Files
- **General Logs**: `logs/avyyan-knitfab-YYYYMMDD.log` (JSON format)
- **Error Logs**: `logs/avyyan-knitfab-errors-YYYYMMDD.log` (Text format)
- **Development Logs**: `logs/dev/avyyan-knitfab-dev-YYYYMMDD.log`

### Log Levels
- **Production**: Information and above
- **Development**: Debug and above
- **Microsoft**: Warning and above (filtered)
- **Entity Framework**: Information for database commands

## 🔐 Security Configuration

### JWT Settings
Update the JWT settings in `appsettings.json`:
```json
{
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-here-make-it-at-least-32-characters-long",
    "Issuer": "AvyyanKnitfab",
    "Audience": "AvyyanKnitfab-Users",
    "ExpirationInMinutes": 60
  }
}
```

## 🌐 API Endpoints

### Machine Manager API
- `GET /api/machinemanager` - Get all machines
- `GET /api/machinemanager/summary` - Get machines summary
- `GET /api/machinemanager/{id}` - Get machine by ID
- `GET /api/machinemanager/name/{name}` - Get machine by name
- `GET /api/machinemanager/search?searchTerm={term}` - Search machines
- `GET /api/machinemanager/status/{status}` - Get machines by status
- `GET /api/machinemanager/manufacturer/{manufacturer}` - Get machines by manufacturer
- `GET /api/machinemanager/efficiency?min={min}&max={max}` - Get machines by efficiency range
- `GET /api/machinemanager/maintenance/due` - Get machines due for maintenance
- `GET /api/machinemanager/efficiency/high?threshold={threshold}` - Get high efficiency machines
- `GET /api/machinemanager/efficiency/low?threshold={threshold}` - Get low efficiency machines
- `GET /api/machinemanager/stats` - Get machine statistics
- `POST /api/machinemanager` - Create new machine
- `PUT /api/machinemanager/{id}` - Update machine
- `DELETE /api/machinemanager/{id}` - Delete machine (soft delete)
- `PATCH /api/machinemanager/{id}/status` - Update machine status
- `PATCH /api/machinemanager/{id}/efficiency` - Update machine efficiency
- `PATCH /api/machinemanager/{id}/maintenance/schedule` - Schedule maintenance
- `POST /api/machinemanager/bulk` - Create multiple machines
- `PATCH /api/machinemanager/bulk/status` - Bulk update status
- `DELETE /api/machinemanager/bulk` - Bulk delete machines

### Chat API
- `GET /api/chat/rooms` - Get user's chat rooms
- `GET /api/chat/rooms/{id}` - Get specific chat room
- `POST /api/chat/rooms` - Create new chat room
- `GET /api/chat/rooms/{id}/messages` - Get chat room messages
- `GET /api/chat/messages/{id}` - Get specific message
- `GET /api/chat/users/search` - Search users for chat
- `GET /api/chat/unread-counts` - Get unread message counts

### Notifications API
- `GET /api/notifications` - Get user notifications
- `GET /api/notifications/unread` - Get unread notifications
- `GET /api/notifications/unread/count` - Get unread count
- `GET /api/notifications/recent` - Get recent notifications
- `PATCH /api/notifications/{id}/read` - Mark notification as read
- `PATCH /api/notifications/read-all` - Mark all as read
- `DELETE /api/notifications/{id}` - Delete notification
- `POST /api/notifications` - Create notification (Admin)
- `POST /api/notifications/bulk` - Create bulk notifications (Admin)
- `POST /api/notifications/system` - Create system notification (Admin)

### WebSocket Hubs
- `/hubs/chat` - Real-time chat functionality
- `/hubs/notifications` - Real-time notifications

## 🏗️ Architecture Patterns

### Repository Pattern
- Generic repository for common CRUD operations
- Specific repositories for complex queries
- Unit of Work for transaction management

### Service Layer
- Business logic separation
- DTO mapping
- Validation handling

### Dependency Injection
- All services registered in `ServiceExtensions.cs`
- Scoped lifetime for database-related services

## 🔄 WebSocket Features

### Real-time Chat
- **Personal Chat** - Direct messaging between users
- **Group Chat** - Multi-user chat rooms with admin controls
- **Message Types** - Text, images, files with metadata
- **Message Reactions** - Emoji reactions to messages
- **Typing Indicators** - Real-time typing status
- **Message Threading** - Reply to specific messages
- **Online Status** - User presence tracking
- **Message History** - Persistent message storage
- **File Attachments** - Support for file sharing

### Live Notifications
- **Real-time Delivery** - Instant notification push
- **Notification Types** - Info, Warning, Error, Success, Chat, Order, System
- **Unread Tracking** - Automatic unread count management
- **Notification Categories** - Organized by business context
- **Action URLs** - Deep links to relevant app sections
- **Bulk Notifications** - Send to multiple users simultaneously
- **System Notifications** - Broadcast to all users
- **Scheduled Notifications** - Future delivery support

### Connection Management
- **Auto-reconnection** - Automatic reconnection on disconnect
- **Connection Tracking** - Monitor user connections and devices
- **Graceful Degradation** - Fallback for connection issues
- **Authentication** - JWT-based WebSocket authentication
- **Authorization** - Role-based access to chat rooms and notifications

## 🔄 Next Steps

1. **Authentication & Authorization**
   - Implement user registration/login
   - Add role-based authorization
   - Create user management endpoints

2. **Additional Controllers**
   - Categories controller
   - Customers controller
   - Orders controller
   - Suppliers controller

3. **Advanced Features**
   - File upload for product images
   - Email notifications
   - Reporting endpoints
   - Inventory management
   - Payment integration

4. **Testing**
   - Unit tests for services
   - Integration tests for controllers
   - Database testing with in-memory provider

## 🐛 Development Notes

- The application uses soft deletes (IsActive flag)
- All entities inherit from BaseEntity for common properties
- Database relationships are properly configured with appropriate delete behaviors
- CORS is configured for frontend integration
- Swagger UI is available at the root URL in development mode

## 📝 Environment Variables

For production, consider using environment variables for sensitive data:
- `ConnectionStrings__DefaultConnection`
- `JwtSettings__SecretKey`
- `JwtSettings__Issuer`
- `JwtSettings__Audience`
