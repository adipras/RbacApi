# RBAC (Role-Based Access Control) API

A .NET Core Web API implementing Role-Based Access Control with JWT authentication.

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or later
- SQL Server (LocalDB, Express, or full version)
- Git

## Getting Started

Follow these steps to get the API running locally:

1. **Clone the Repository**
   ```bash
   git clone [repository-url]
   cd RbacApi
   ```

2. **Restore Dependencies**
   ```bash
   dotnet restore
   ```

3. **Install Entity Framework Core Tools**
   ```bash
   dotnet tool install --global dotnet-ef
   ```

4. **Configure Database Connection**
   
   Update the connection string in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=RbacDb;Trusted_Connection=True;TrustServerCertificate=true"
     }
   }
   ```

5. **Create and Apply Database Migrations**
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

6. **Build and Run the Application**
   ```bash
   dotnet build
   dotnet run
   ```

   The API will be available at:
   ```
   Swagger UI: https://localhost:7234/swagger
   API Base URL: https://localhost:7234/api
   ```

## API Testing

1. **Login to get JWT Token**
   ```http
   POST /api/auth/login
   Content-Type: application/json

   {
       "username": "admin",
       "password": "Admin123!"
   }
   ```

2. **Use the JWT Token for authenticated requests**
   ```http
   GET /api/users
   Authorization: Bearer <your-token>
   ```

## Troubleshooting

If you encounter database issues, try resetting the database:

```bash
# Drop existing database
dotnet ef database drop -f

# Remove migrations
dotnet ef migrations remove

# Create new migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update
```

## Available Endpoints

### Authentication
```http
POST /api/auth/register - Register new user
POST /api/auth/login - Login user
```

### Users
```http
GET /api/users - Get all users
GET /api/users/{id} - Get user by ID
GET /api/users/{id}/roles - Get user roles
POST /api/users/{userId}/roles/{roleId} - Assign role to user
DELETE /api/users/{userId}/roles/{roleId} - Remove role from user
```

### Roles
```http
GET /api/roles - Get all roles
GET /api/roles/{id} - Get role by ID
POST /api/roles - Create new role
PUT /api/roles/{id} - Update role
DELETE /api/roles/{id} - Delete role
```

### Permissions
```http
GET /api/permissions - Get all permissions
GET /api/permissions/{id} - Get permission by ID
POST /api/permissions - Create new permission
PUT /api/permissions/{id} - Update permission
DELETE /api/permissions/{id} - Delete permission
```

## Technology Stack

```
- .NET 9.0
- Entity Framework Core
- SQL Server
- JWT Authentication
- Swagger/OpenAPI
```

## Project Structure

```
RbacApi/
├── Controllers/         # API Controllers
├── Data/               # Database context and configurations
├── DTOs/               # Data Transfer Objects
├── Models/             # Domain models
├── Services/           # Business logic services
└── Middleware/         # Custom middleware
```
