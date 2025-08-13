# UserService

The UserService is a microservice within the Finman financial management platform, responsible for user authentication and management.

## Overview

This service follows Hexagonal Architecture (Ports and Adapters) pattern and provides:

- User registration with email/username uniqueness validation
- Password hashing using BCrypt
- RESTful API endpoints
- Comprehensive health checks

## Architecture

```
├── src/
│   └── UserService/               # Main application
│       ├── Domain/               # Business entities and rules
│       ├── Application/          # Use cases and DTOs
│       └── Infrastructure/       # Web controllers, repositories, security
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

- **Domain Tests**: Value objects and entities
- **Application Tests**: Use cases and business logic  
- **Infrastructure Tests**: Controllers, repositories, security, integration tests

All tests must pass before deployment. Current coverage includes 125 tests across all layers.

## Dependencies

- .NET 8.0
- ASP.NET Core
- BCrypt for password hashing
- Swagger for API documentation (Development only)
- xUnit for testing
