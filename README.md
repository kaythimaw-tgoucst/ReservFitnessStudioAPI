# FitnessStudio API

Backend API for class booking, waitlist, and package management.

## Tech Stack

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core (SQL Server)
- JWT Bearer authentication
- Redis (distributed booking lock)

## Solution Structure

- `FitnessStudioAPI` - API host and controllers
- `FitnessStudio.Application` - use cases, DTOs, interfaces
- `FitnessStudio.Domain` - domain entities
- `FitnessStudio.Infrastructure` - repositories, EF contexts, gateways, seeding
- `FitnessStudio.Tests` - unit tests

## Database Contexts

Current context names:

- `FitnessStudioDbContext`
- `FitnessStudioWriteDbContext` (inherits `FitnessStudioDbContext`)

Both are configured in `FitnessStudioAPI/Program.cs` using `ConnectionStrings:DefaultConnection`.

## Configuration

Set these values in `FitnessStudioAPI/appsettings.json`:

- `ConnectionStrings:DefaultConnection`
- `ConnectionStrings:Redis`
- `Jwt:Issuer`
- `Jwt:Audience`
- `Jwt:Key`

## Authentication

There is no username/password login API.

Authentication bootstrap endpoint:

- `GET /api/auth/account?email={email}`

This endpoint returns a JWT token for the user email if found.

All business endpoints require:

- `Authorization: Bearer <token>`

## API Endpoints (Current Design)

### Auth

- `GET /api/auth/account?email={email}`

### Business Studio (Authorized)

- `GET /api/businessstudio`

### Timetable (Authorized)

- `GET /api/timetable?businessStudioId={guid}&pageNumber=1&pageSize=10&startDate={optional}&endDate={optional}`

### Packages (Authorized)

- `GET /api/packages?businessStudioId={guid}&pageNumber=1&pageSize=10`
- `POST /api/packages/purchase`

Request body (`POST /api/packages/purchase`):

```json
{
  "businessStudioId": "00000000-0000-0000-0000-000000000000",
  "totalCredits": 10,
  "expiryDate": "2026-12-31T00:00:00Z"
}
```

### Bookings (Authorized)

- `GET /api/bookings?businessStudioId={guid}&pageNumber=1&pageSize=10`
- `POST /api/bookings`
- `POST /api/bookings/cancel`

Request body (`POST /api/bookings`):

```json
{
  "timetableScheduleId": "00000000-0000-0000-0000-000000000000",
  "businessStudioId": "00000000-0000-0000-0000-000000000000"
}
```

Request body (`POST /api/bookings/cancel`):

```json
{
  "bookingId": "00000000-0000-0000-0000-000000000000"
}
```

### Waitlist (Authorized)

- `POST /api/waitlist`

Request body:

```json
{
  "timetableScheduleId": "00000000-0000-0000-0000-000000000000",
  "businessStudioId": "00000000-0000-0000-0000-000000000000"
}
```

## Run Locally

1. Update `appsettings.json` connection strings and JWT settings.
2. Start dependencies (SQL Server and Redis).
3. Run:
   - `dotnet run --project FitnessStudioAPI`

In Development:

- OpenAPI JSON: `/openapi/v1.json`
- Swagger UI: `/swagger`

## Test

- `dotnet test FitnessStudio.Tests/FitnessStudio.Tests.csproj`
