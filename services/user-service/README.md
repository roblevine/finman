# UserService

The UserService is a microservice within the Finman financial management platform, responsible for user authentication and management.

## Overview

This service follows Hexagonal Architecture (Ports and Adapters) pattern and provides:

- User registration with email/username uniqueness validation
- Password hashing using BCrypt
- PostgreSQL persistence with EF Core (Production/Development)
- In-memory storage for testing
- Environment-aware configuration
- RESTful API endpoints
- Comprehensive health checks (including PostgreSQL monitoring)

## Architecture

```
├── src/
│   └── UserService/               # Main application
│       ├── Domain/               # Business entities and rules
│       ├── Application/          # Use cases and DTOs
│       └── Infrastructure/       # Web controllers, repositories, security, persistence
│           ├── Persistence/      # EF Core DbContext, repositories, migrations
│           ├── Repositories/     # In-memory implementations for testing
│           ├── Security/         # BCrypt password hashing
│           └── Web/              # Controllers and API layer
├── tests/                        # Test projects
├── scripts/                      # Build, test, and run scripts
└── UserService.sln              # Solution file
```

## Endpoints

- `GET /api/hello` → 200 JSON `{ message, timestamp, version }`
- `GET /api/hello/{name}` → 200 personalized message; 400 for empty/whitespace
- `POST /api/auth/register` → 201 on success, 409 for duplicates, 400 for validation errors
- `GET /health` → 200 "Healthy"
- Swagger (Dev): `/swagger`, `/swagger/v1/swagger.json`

## Scripts

- **Setup**: `./scripts/setup.sh`
- **Build + Tests**: `./scripts/build.sh` (Release; builds Docker image if Docker available)
- **Tests + Coverage**: `./scripts/test.sh` (Cobertura in `TestResults/**/coverage.cobertura.xml`)
- **Run**: `./scripts/run.sh --local` (http://localhost:5001) or `--docker` (http://localhost:8080)
- **Clean**: `./scripts/clean.sh`

## Development

This service is part of a larger monorepo. For monorepo-level operations, use the root-level scripts.

For service-specific development:

```bash
cd services/user-service
./scripts/test.sh     # Run tests
./scripts/build.sh    # Build and test
./scripts/run.sh --local  # Run locally
```

## Testing

The service uses xUnit with comprehensive test coverage across:

- **Domain Tests**: Value objects and entities (48 tests)
- **Application Tests**: Use cases and business logic (10 tests)
- **Infrastructure Tests**: Controllers, repositories, security, integration tests (57 tests)

All tests must pass before deployment. Current status: **114/115 tests passing** (1 unrelated Swagger test failure).

**Test Strategy**: 
- Production/Development environments use PostgreSQL persistence
- Test environment uses in-memory repositories for fast, isolated testing
- Future: Testcontainers integration tests for end-to-end PostgreSQL validation

## Dependencies

- .NET 8.0
- ASP.NET Core
- **PostgreSQL** with EF Core 9.0.8 and Npgsql 9.0.4 (Production/Development)
- BCrypt for password hashing  
- Swagger for API documentation (Development only)
- xUnit for testing
- DotNet.Testcontainers for integration testing

## Persistence

The service uses **environment-aware persistence**:

- **Production/Development**: PostgreSQL with EF Core
  - Rich value object mappings using OwnsOne pattern
  - PostgreSQL `citext` extension for case-insensitive email/username uniqueness
  - Automatic health checks for database connectivity
  - Code-first migrations with version control
  
- **Test Environment**: In-memory repository
  - Fast, isolated testing without external dependencies
  - Full compatibility with existing test suite

Configuration via connection string:
```
DefaultConnection: "Host=localhost;Port=5432;Database=finman_user_service;Username=finman_user;Password=finman_password"
```
