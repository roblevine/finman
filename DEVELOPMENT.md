# Development Guide

This document provides comprehensive development guidance for the Finman monorepo, including setup, workflow, and service development patterns.

## Monorepo Structure

Finman uses a **monorepo microservices architecture** with the following structure:

```
finman/
├── services/
│   ├── user-service/              # User authentication & management
│   │   ├── src/UserService/       # Main application code
│   │   ├── tests/                 # Service-specific tests
│   │   ├── scripts/               # Service build/test/run scripts
│   │   ├── UserService.sln        # Independent solution
│   │   └── README.md              # Service documentation
│   ├── shared/                    # Shared libraries
│   │   ├── Finman.Shared.Domain/           # Common domain models
│   │   ├── Finman.Shared.Infrastructure/   # Common infrastructure
│   │   ├── Finman.Shared.Testing/          # Test utilities
│   │   └── Finman.Shared.sln               # Shared solution
│   └── template-service/          # Template for creating new services
├── frontend/                      # Next.js SPA (future)
├── infrastructure/                # Docker, deployment configs
├── scripts/                       # Root-level orchestration scripts
├── docs/                          # Architecture documentation
└── plans/                         # Implementation plans & ADRs
```

### Service Independence Principles

- **Independent Development**: Each service can be built, tested, and run independently
- **Independent Solutions**: Each service has its own `.sln` file
- **Independent Versioning**: Services can be versioned and deployed independently
- **HTTP-Only Communication**: Services communicate via HTTP APIs only (no direct assembly references)
- **Shared Libraries**: Common code is consumed as project references (not NuGet packages initially)

## Development Workflow

### Methodology
We follow a **Test-First Analyse → Plan → Execute → Review** methodology:

1. **Analyse**: Understand requirements and break down problems
2. **Plan**: Document approach in PLAN-*.md files for major features
3. **Execute**: Implement in small, testable increments
4. **Review**: Verify functionality and update documentation

### Service Development

#### Working on Individual Services

Each service can be developed independently using its own scripts:

```bash
# Navigate to service directory
cd services/user-service

# Setup service (one-time)
./scripts/setup.sh

# Development cycle
./scripts/test.sh              # Run tests
./scripts/build.sh             # Build and test (creates Docker image)
./scripts/run.sh --local       # Run locally (e.g., http://localhost:5001)
./scripts/run.sh --docker      # Run in Docker (e.g., http://localhost:8080)

# Cleanup
./scripts/clean.sh
```

#### Working with Multiple Services

Use root-level orchestration scripts for monorepo-wide operations:

```bash
# From repository root
./scripts/setup.sh      # Setup entire monorepo
./scripts/test.sh       # Test all services and shared libraries
./scripts/build.sh      # Build all services and shared libraries
./scripts/run.sh        # Run services (with orchestration options)
./scripts/clean.sh      # Clean all build artifacts
```

### Creating New Services

1. **Use the Template**:
   ```bash
   # Copy template service
   cp -r services/template-service services/my-new-service
   cd services/my-new-service
   ```

2. **Customize the Service**:
   - Update `README.md` with service-specific information
   - Modify solution and project names
   - Implement service-specific functionality following Hexagonal Architecture

3. **Follow Established Patterns**:
   - Use the same directory structure as existing services
   - Follow the Hexagonal Architecture pattern (see ARCHITECTURE.md)
   - Maintain comprehensive test coverage
   - Use provided script templates

## Database Development

### PostgreSQL Setup

Finman uses PostgreSQL for persistence. The database setup supports both host and devcontainer development.

#### Quick Setup

```bash
# Start PostgreSQL container
docker-compose up postgres -d

# Verify container is running
docker-compose ps postgres

# Check health
docker-compose logs postgres
```

#### Connection Details

**From Host Machine**:
- Host: `localhost`
- Port: `5432`
- Database: `finman`
- Username: `finman`
- Password: `finman_dev_password`

**From Devcontainer**:
- Host: `postgres` (service name)
- Port: `5432`
- Database: `finman`
- Username: `finman`  
- Password: `finman_dev_password`

#### Connection Strings

```bash
# For devcontainer applications
Host=postgres;Database=finman;Username=finman;Password=finman_dev_password

# For host applications  
Host=localhost;Database=finman;Username=finman;Password=finman_dev_password
```

#### Database Management

```bash
# Connect to database
docker-compose exec postgres psql -U finman -d finman

# Stop database
docker-compose stop postgres

# Reset database (removes all data)
docker-compose down postgres -v
```

#### Current Implementation Status

- **Phase 1-3 Complete**: Dependencies, ports, and infrastructure persistence layer implemented
- **EF Core Integration**: FinmanDbContext with PostgreSQL support and value object mappings
- **Migrations**: Initial schema with citext support for case-insensitive uniqueness
- **Environment Awareness**: Uses PostgreSQL in production/development, InMemoryRepository in tests

### Entity Framework Core

Services use EF Core with PostgreSQL:

```bash
# Add migration (from service directory)
cd services/user-service/src/UserService
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update
```

## Testing Strategy

### Test Structure

Tests mirror the service architecture:

```
tests/
├── Domain.Tests/              # Domain logic tests (framework-free)
├── Application.Tests/         # Use case and application logic tests
└── Infrastructure.Tests/     # Infrastructure adapter tests
```

### Running Tests

```bash
# Service-specific tests
cd services/user-service
./scripts/test.sh

# All tests with coverage
./scripts/test.sh              # From root (generates coverage reports)
```

### Test Guidelines

- **Framework-Free Domain**: Domain tests should not depend on external frameworks
- **Test-First Development**: Write tests before implementing functionality
- **Comprehensive Coverage**: Aim for high test coverage across all layers
- **Fast Feedback**: Use InMemoryRepository for unit tests, real database for integration tests
- **No FluentAssertions**: Use standard xUnit assertions (see ARCHITECTURE.md)

## Build and Deployment

### Local Development

```bash
# Build and run locally
cd services/user-service
./scripts/build.sh
./scripts/run.sh --local
```

### Docker Development

```bash
# Build Docker image
./scripts/build.sh             # Creates finman-userservice:latest

# Run in Docker
./scripts/run.sh --docker      # Runs on http://localhost:8080
```

### CI/CD Pipeline

- Each service builds independently
- Docker images created during build process
- Comprehensive test suites run before deployment
- Services can be deployed independently

## Architecture Patterns

### Hexagonal Architecture

All services follow Hexagonal Architecture (Ports and Adapters):

- **Domain**: Business entities, value objects, domain services (framework-free)
- **Application**: Use cases, DTOs, ports (interfaces)
- **Infrastructure**: Web controllers, repositories, external service adapters

### Service Communication

- **HTTP APIs Only**: Services communicate exclusively via HTTP
- **Independent Deployment**: Services can be deployed and scaled independently
- **API Documentation**: Swagger documentation available in development mode

## Development Environment

### Prerequisites

- .NET 8.0 SDK
- Docker and Docker Compose
- Git

### VS Code Configuration

The repository includes a multi-folder workspace configuration (`finman.code-workspace`) optimized for monorepo development.

### Devcontainer Support

The repository supports development containers with:
- Pre-installed .NET SDK, Docker CLI, and development tools
- Network integration with PostgreSQL container
- Consistent development environment across team members

## Contributing

### Code Style

- Use meaningful variable and function names
- Follow SOLID principles
- Prefer clean object-oriented design
- Maintain framework-free domain logic

### Documentation

- Update README.md for user-facing changes
- Update ARCHITECTURE.md for architectural changes
- Create PLAN-*.md files for major features
- Update service README files for service-specific changes

### Dependencies

- Always discuss before adding new dependencies
- Prefer built-in solutions over external libraries
- Consider impact on other services when adding shared dependencies

## Troubleshooting

### Common Issues

1. **Port Conflicts**: Check if ports 5432 (PostgreSQL) or service ports are in use
2. **Docker Network**: Ensure containers are on the `finman-network`
3. **Missing Dependencies**: Run `./scripts/setup.sh` to install prerequisites
4. **Test Failures**: Check database connectivity and migrations

### Getting Help

1. Check service-specific README files
2. Review ARCHITECTURE.md for design patterns
3. Examine existing working services as examples
4. Check plans/ directory for feature implementation guidance

## Monorepo Migration Success

The monorepo restructure was completed successfully on August 13, 2025:

- **Zero Functional Regressions**: All 125 tests passing after migration
- **Service Independence**: UserService fully independent and functional  
- **Shared Libraries Ready**: Foundation created for future shared code
- **Template Service**: Standard template available for service expansion
- **Root Orchestration**: Multi-service build/test/run scripts operational
- **Clean Structure**: No redundant files or orphaned artifacts

This foundation enables future service expansion, infrastructure orchestration, and unified frontend development while maintaining service independence and development workflow familiarity.


## Architecture and layout
- Hexagonal (Ports & Adapters). Current service: `src/UserService`.
	- Domain: `Domain/{Entities,ValueObjects,Exceptions}`
	- Application: `Application/{Ports,UseCases,DTOs}`
	- Infrastructure: `Infrastructure/Web/Controllers`
	- Composition root: `Program.cs` (partial for tests)
- See `HelloController.cs` for controller style; `Program.cs` registers controllers, Swagger (Dev), and health checks.

*For detailed technical architecture and design patterns, see [ARCHITECTURE.md](ARCHITECTURE.md).*

## Endpoints (UserService)
- GET `/api/hello` → 200 JSON `{ message, timestamp, version }`
- GET `/api/hello/{name}` → 200 personalized; 400 for empty/whitespace
- GET `/health` → 200 "Healthy"
- Swagger (Dev): `/swagger`, `/swagger/v1/swagger.json`

## Conventions
- Tests: xUnit; do not use FluentAssertions (see ARCHITECTURE.md). Mirror target layer in `tests/*`.
- Controllers: `[ApiController]`, `[Route("api/...“)]`, `[Produces("application/json")]`; return `ActionResult<T>` with `[ProducesResponseType]`. Example returns `HelloResponse` record (init-only props).
- Health checks: `AddHealthChecks().AddCheck("self", ...)` + `app.MapHealthChecks("/health")`.
- Swagger enabled only in Development in `Program.cs`.
- Test host: `TestWebApplicationFactory` forces `Development` and sets JSON `camelCase`.

## Extending safely
1) Write tests (`tests/*`), including minimal integration via `WebApplicationFactory`.
2) Define ports in `Application/Ports`; add use cases in `Application/UseCases`.
3) Update domain in `Domain/*` (framework-free logic).
4) Implement adapters in `Infrastructure/*`; wire in `Program.cs`.
5) Run `build.sh`, then `run.sh`; validate with `/health`, `/swagger`, endpoints, and tests.

Key references: `src/UserService/Infrastructure/Web/Controllers/HelloController.cs`, `src/UserService/Program.cs`, `tests/Infrastructure.Tests/*`.