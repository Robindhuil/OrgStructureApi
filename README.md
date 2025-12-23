# OrgStructureApi

This repository implements a REST API for managing an organisational structure of companies and their employees.

It is a small but realistic ASP.NET Core Web API project demonstrating controllers, DTO validation, Entity Framework Core migrations, and an integration-style test suite (TeaPie).

---

**Appointment / Requirement**

"Document how to easily run/start the project (don't forget database creation — providing an SQL script is sufficient).

REST API — Organizational structure of companies

Create a REST API that allows managing the organizational structure of companies and recording their employees.

API requirements

- Allow managing (create/update/delete) a 4-level hierarchical organizational structure: companies → divisions → projects → departments.
- Each node of the structure must have a name and a code and must have a leader (company — director, division — division leader, project — project leader, department — department leader). The leader of a node must be one of the company's employees.
- Allow adding, modifying, and deleting employees.
- For each employee store at minimum: title, first name and last name, phone, and email.
- The API should include basic validations and return appropriate errors for missing or invalid input.
- Store the organizational structure and employee list in a database. Use C# (.NET) and Microsoft SQL Server for data storage (Express edition is acceptable).

Include ScalaR in the project so the endpoints can be explored and tried. Use TeaPie to test the endpoints."

---

## What this project contains

- ASP.NET Core Web API (controllers under `Controllers/`)
- DTOs and validation attributes under `Dtos/`
- EF Core `AppDbContext` and migrations under `Migrations/`
- Models under `Models/` (Company, Division, Project, Department, Employee)
- Middleware and extensions for error handling (`Middleware/`, `Extensions/`)
- Tests written as TeaPie requests and C# test scripts in `Tests/`

## Tech stack

- .NET 8 (project targets `net8.0`)
- C# Web API (controllers + DTOs)
- Entity Framework Core (migrations included)
- Microsoft SQL Server (compatible with Express edition)
- TeaPie for integration testing (collections in `Tests/`)
- ScalaR (API exploration UI available in Development)

## Quickstart — run locally (recommended)

Prerequisites

- Install .NET SDK 8.x: https://dotnet.microsoft.com/en-us/download
- Docker (recommended for running SQL Server locally) or a running Microsoft SQL Server instance
- (Optional) `dotnet-ef` tool to run migrations locally: `dotnet tool install --global dotnet-ef`
- TeaPie CLI installed to run tests, or run tests on a machine with TeaPie available (see TeaPie docs)

1. Start a SQL Server instance (Docker example)

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Your_strong!Passw0rd" \
	-p 1433:1433 --name orgstructure-sql -d mcr.microsoft.com/mssql/server:2019-latest
```

2. Update connection string

- Edit `appsettings.Development.json` or set the environment variable `ConnectionStrings__DefaultConnection` to point to your SQL Server. Example connection string:

```json
"ConnectionStrings": {
	"DefaultConnection": "Server=localhost,1433;Database=OrgStructureDb;User Id=sa;Password=Your_strong!Passw0rd;TrustServerCertificate=True;"
}
```

3. Apply EF Core migrations (create the DB)

From the repo root (where `OrgStructureApi.csproj` lives):

```bash
# (optional) install dotnet-ef if not already installed
dotnet tool install --global dotnet-ef

# apply migrations and create the database
dotnet ef database update --project ./OrgStructureApi.csproj

# alternatively generate a SQL script
dotnet ef migrations script -o ./init.sql --project ./OrgStructureApi.csproj
```

If you produced `init.sql`, you can run that script against SQL Server (for example using `sqlcmd` or SSMS).

4. Run the API

```bash
dotnet run --project ./OrgStructureApi.csproj
```

By default Kestrel will output the URLs it listens on (e.g., `http://localhost:5000` and `https://localhost:5001`).

Visit:

- ScalaR UI (in development): check the development UI endpoint configured in `Program.cs`/startup logs (commonly served at the application's base URL)
- Health endpoint: `GET /health` (used by tests)

5. Run tests (TeaPie)

Make sure the API is running and accessible (same host/port the tests expect). Then run:

```bash
# run all TeaPie tests
teapie test ./Tests/

# run a single test collection
teapie test ./Tests/Company
```

Note: TeaPie CLI must be installed and on PATH. If you don't have the `teapie` binary, follow TeaPie's installation instructions.

## How the API works — overview

- Resources: `Company` → `Division` → `Project` → `Department` and `Employee`.
- Each node (company/division/project/department) has `Name`, `Code` and a `Leader` which is an `Employee` within that company.
- Controllers expose standard REST endpoints for Create (POST), Read (GET), Update (PUT/PATCH) and Delete (DELETE).
- DTO validation uses data annotations (`[Required]`, `[StringLength]`, `[EmailAddress]`, `[Phone]`). Controllers check referenced entities (leaders/directors) and return appropriate errors.
- `AppDbContext` configures delete behaviors and unique constraints (e.g., unique `Employee.Email`, company-scoped codes for divisions/projects/departments).
- Error handling: unhandled exceptions are surfaced through `ApiExceptionMiddleware`. Many controller errors return a structured `ApiErrorResponse` (see `Dtos/ApiErrorResponse.cs`). Some controller paths may still return plain strings — standardising error responses is recommended.

## Validation and common responses

- 201 Created: resource successfully created (POST). API often returns `201` with created resource or `204` for successful updates/deletes where appropriate.
- 204 No Content: successful update (PUT/PATCH) or delete when there is no response body.
- 400 Bad Request: validation failures (missing required fields, invalid model state) or invalid referenced entity data.
- 404 Not Found: resource does not exist.
- 409 Conflict: unique constraint violation (e.g., duplicate email or duplicate code within scope).

Error payload (preferred structure):

```json
{
  "status": 400,
  "title": "Bad Request",
  "detail": "Validation failed",
  "errors": {
    "email": ["The Email field is required."]
  }
}
```

Controllers attempt to return structured errors (`ApiErrorResponse`), but some paths currently return plain strings. Tests in `Tests/` have been made tolerant to both shapes — standardizing responses is on the TODO list.

## Tests and CI notes

- Tests are implemented with TeaPie request collections in `Tests/` and C# assertions in `.csx` scripts. They are integration-style tests that expect a running server.
- To get deterministic test runs in CI, ensure a clean database is created prior to running tests (either run migrations against a fresh DB or run migrations + seed then run tests). The test collection tolerates possible `409` responses from duplicate constraints when the DB isn't reset.
- Recommended CI flow:
  1.  Start a SQL Server container
  2.  Set `ConnectionStrings__DefaultConnection` to the container DB
  3.  Run `dotnet ef database update`
  4.  Start the Web API (in background)
  5.  Run `teapie test ./Tests/`

## Troubleshooting

- If you see `409 Conflict` on create tests, it usually means the DB already contains a record with the same unique value (email, phone, or code). Reset DB or use a clean DB for CI.
- If TeaPie cannot connect, ensure the API is running and the base URL in the test requests matches (often `http://localhost:5000`).
- If EF migrations fail, confirm `dotnet-ef` is installed and the connection string points to a reachable SQL Server with valid credentials.

## Recommended next improvements (short list)

- Standardize all controller errors to `ApiErrorResponse`.
- Validate referenced entities before persisting (or wrap in transactions) to avoid partial writes.
- Extract validation logic (leader/director membership) into a shared service.
- Make tests deterministic by creating unique data per-test or running against a fresh DB per-run.

## Project layout (quick)

- `Controllers/` — API controllers per resource
- `Dtos/` — request/response DTOs and `ApiErrorResponse`
- `Models/` — EF entity models
- `Data/AppDbContext.cs` — EF Core DB context and constraints
- `Migrations/` — EF Core migrations (use `dotnet ef database update` to apply)
- `Tests/` — TeaPie collections and C# tests used to validate endpoints
