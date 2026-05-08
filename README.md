# Student Management System API
### Zest India IT Pvt Ltd — Full Stack Developer Technical Assignment
**Framework:** ASP.NET Core 8.0 Web API | **ORM:** Entity Framework Core 8 | **DB:** SQL Server

---

## Project Structure

```
StudentManagementSystem/
├── Controllers/
│   ├── AuthController.cs        # POST /api/auth/register  |  POST /api/auth/login
│   └── StudentsController.cs    # CRUD /api/students
├── Data/
│   └── AppDbContext.cs          # EF Core DbContext
├── Database/
│   └── setup.sql                # Manual SQL setup script (alternative to EF Migrations)
├── DTOs/
│   ├── ApiResponse.cs           # Generic wrapper: { success, message, data, errors }
│   ├── AuthDTOs.cs              # RegisterDto, LoginDto, AuthResponseDto
│   └── StudentDTOs.cs           # CreateStudentDto, UpdateStudentDto, StudentResponseDto
├── Helpers/
│   └── JwtHelper.cs             # JWT token generation
├── Middleware/
│   └── ExceptionHandlingMiddleware.cs  # Global exception handler
├── Migrations/                  # EF Core auto-generated migrations
├── Models/
│   ├── Student.cs               # Student entity
│   └── User.cs                  # User entity
├── Repositories/
│   ├── Interfaces/
│   │   ├── IStudentRepository.cs
│   │   └── IUserRepository.cs
│   ├── StudentRepository.cs
│   └── UserRepository.cs
├── Services/
│   ├── Interfaces/
│   │   ├── IStudentService.cs
│   │   └── IAuthService.cs
│   ├── StudentService.cs
│   └── AuthService.cs
├── Logs/                        # Auto-created at runtime by Serilog
├── appsettings.json
├── appsettings.Development.json
└── Program.cs
```

---

## Quick Start

### Step 1 — Configure the Database Connection

Edit `appsettings.json` and update **your SQL Server name**:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=StudentManagementDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

Common server name examples:
| Setup | Server Name |
|---|---|
| SQL Server Express (local) | `.\SQLEXPRESS` or `localhost\SQLEXPRESS` |
| SQL Server Default (local) | `.` or `localhost` |
| SQL Server with instance | `DESKTOP-XXX\SQLEXPRESS` |

### Step 2 — Apply EF Core Migrations (creates DB automatically)

```bash
# Option A: Auto-applied on startup (already configured in Program.cs)
dotnet run

# Option B: Apply manually via CLI
dotnet ef database update
```

> **Alternative:** Run `Database/setup.sql` directly in SQL Server Management Studio (SSMS) to create the schema manually.

### Step 3 — Run the API

```bash
dotnet run
```

The API starts at: **`http://localhost:5000`**  
Swagger UI opens at: **`http://localhost:5000`** (root URL)

---

## API Endpoints

### 🔐 Authentication (No token required)

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/auth/register` | Register a new user |
| `POST` | `/api/auth/login` | Login → returns JWT token |

### 👨‍🎓 Students (JWT Bearer token required)

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/students` | Get all students |
| `GET` | `/api/students/{id}` | Get student by ID |
| `POST` | `/api/students` | Create new student |
| `PUT` | `/api/students/{id}` | Update student |
| `DELETE` | `/api/students/{id}` | Delete student |

---

## How to Test via Swagger UI

1. Open `http://localhost:5000` in your browser
2. **Register** → `POST /api/auth/register`
   ```json
   { "username": "admin", "password": "Admin@123", "role": "Admin" }
   ```
3. **Login** → `POST /api/auth/login`
   ```json
   { "username": "admin", "password": "Admin@123" }
   ```
4. Copy the `token` from the response
5. Click **Authorize 🔒** at the top of Swagger
6. Enter: `Bearer <your_token_here>` and click **Authorize**
7. Now all Student endpoints are accessible

---

## Sample Request / Response

### Register
```json
POST /api/auth/register
{
  "username": "admin",
  "password": "Admin@123",
  "role": "Admin"
}
```

### Login Response
```json
{
  "success": true,
  "message": "Login successful.",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "username": "admin",
    "role": "Admin",
    "expiration": "2026-05-10T00:00:00Z"
  }
}
```

### Create Student
```json
POST /api/students
Authorization: Bearer <token>

{
  "name": "Aarav Sharma",
  "email": "aarav.sharma@example.com",
  "age": 20,
  "course": "Computer Science"
}
```

### Get All Students Response
```json
{
  "success": true,
  "message": "5 student(s) retrieved.",
  "data": [
    {
      "id": 1,
      "name": "Aarav Sharma",
      "email": "aarav.sharma@example.com",
      "age": 20,
      "course": "Computer Science",
      "createdDate": "2026-05-09T00:00:00Z"
    }
  ]
}
```

---

## Technical Implementation

| Feature | Implementation |
|---------|---------------|
| **Authentication** | JWT Bearer tokens (HMAC-SHA256, configurable expiry) |
| **Password Hashing** | BCrypt (`BCrypt.Net-Next`) |
| **Global Error Handling** | Custom `ExceptionHandlingMiddleware` — maps exception types to HTTP status codes |
| **Logging** | Serilog → Console + rolling file (`Logs/app-YYYY-MM-DD.log`) |
| **Swagger** | Swashbuckle with JWT security definition |
| **Architecture** | Controller → Service → Repository → DbContext (strict layering) |
| **ORM** | Entity Framework Core 8 with Code-First migrations |
| **Response Format** | Uniform `ApiResponse<T>` wrapper on all endpoints |
| **Validation** | Data Annotations + ModelState checks |
| **DB Resilience** | EF retry-on-failure (5 retries, 10s max delay) |

---

## JWT Configuration (`appsettings.json`)

```json
"JwtSettings": {
  "SecretKey": "ZestIndia@SuperSecretKey#2024!StudentMgmt$JWT",
  "Issuer": "StudentManagementSystem",
  "Audience": "StudentManagementSystem",
  "ExpiryHours": "24"
}
```

> ⚠️ In production, store the `SecretKey` in environment variables or Azure Key Vault — never in source code.

---

## EF Core CLI Commands (Reference)

```bash
# Create a new migration
dotnet ef migrations add <MigrationName>

# Apply migrations to database
dotnet ef database update

# Remove last migration
dotnet ef migrations remove

# Generate SQL script from migrations
dotnet ef migrations script
```
