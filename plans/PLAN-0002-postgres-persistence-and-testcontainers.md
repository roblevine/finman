# Plan: PostgreSQL persistence and Testcontainers

**Status**: PAUSED 🔄 (Phases 1-3 Complete, 5 Remaining)  
**Started**: August 10, 2025  
**Last Updated**: August 15, 2025  
**Paused**: August 16, 2025 (Documentation reorganization priority)  
**Phase 4 Completed:** 2025-08-15

## Overview
Introduce durable persistence using PostgreSQL while preserving the existing Hexagonal (Ports & Adapters) architecture. We will use EF Core with Npgsql for data access and code-first migrations, and DotNet.Testcontainers to run real PostgreSQL instances during integration tests. Scope includes schema/modeling for Users, repository ports and adapters, DI wiring, local dev experience (Docker), and a minimal migration workflow. Out of scope: full auth flows or advanced aggregates beyond Users.

In scope:
- EF Core + Npgsql integration (code-first, migrations committed to repo)
- Application-layer repository ports for User aggregate
- Infrastructure DbContext + fluent configurations and EF-based repository adapters
- Dockerized Postgres for local dev; environment-driven connection strings
- Testcontainers-based integration tests and test fixtures
- Health checks for Postgres and guarded startup migration in Development

Out of scope (for this phase):
- Non-User aggregates and complex relationships
- Advanced performance tuning, connection sharding, read replicas
- Full-blown auth/identity server

## Requirements
- Use PostgreSQL as the persistence store
- Keep Domain layer framework-free; map via Infrastructure
- Define repository ports in Application and EF adapters in Infrastructure
- Implement code-first migrations with deterministic, reviewed scripts
- Support local development via Docker Compose
- Add Testcontainers-backed integration tests
- Secure, env-based configuration for connection strings
- Add a DB health check; keep existing self health check
- Enforce case-insensitive uniqueness for email and username
- Minimal impact on existing endpoints; all tests remain green

## Architecture and Design
- Data access: EF Core (Microsoft.EntityFrameworkCore) with Npgsql provider
- Migrations: code-first; first migration creates users table and enables "citext" for case-insensitive email/username using a migration SQL step (CREATE EXTENSION IF NOT EXISTS citext)
- Domain: unchanged; no EF attributes; entities/value objects stay pure
- Application layer: define IUserRepository (ports) with operations required by registration and lookup use cases (ExistsByEmail, ExistsByUsername, Add, GetById, FindByEmail)
- Infrastructure:
  - Persistence: FinmanDbContext + Fluent API configurations per aggregate
  - Repositories: EF-based implementations of Application ports
  - Configuration: connection string via env var POSTGRES_CONNECTION (fallback to POSTGRES_HOST/DB/USER/PASSWORD if desired)
  - Health checks: AddNpgSql check wired to the configured connection string
- Transactions: start with DbContext scoped per request; promote to explicit transactions in application use cases where needed
- Concurrency: rely on DB uniqueness constraints; consider optimistic concurrency token later (xmin)
- Observability: EF Core logging at Information in Development; optional slow query logging threshold

## Progress Summary

### ✅ **Completed Phases (1-4)**
**Phase 1**: Dependencies and local PostgreSQL setup complete
- Added EF Core 9.0.8, Npgsql 9.0.4, health checks, and Testcontainers dependencies
- Extended docker-compose.yml with PostgreSQL 16-alpine service
- Updated setup scripts for local development

**Phase 2**: Application ports enhancement complete  
- Enhanced IUserRepository interface with 5 comprehensive methods
- Updated all implementations (InMemory, Mock, RegisterUserHandler)
- Maintained full backward compatibility with existing functionality

**Phase 3**: Infrastructure persistence layer complete
- Created FinmanDbContext with sophisticated value object mappings using OwnsOne pattern
- Generated initial EF Core migration (20250815163136_InitialUsers) with PostgreSQL citext support
- Implemented comprehensive EfUserRepository with proper error handling and PostgreSQL optimizations
- Added environment-aware DI configuration (PostgreSQL for prod/dev, InMemory for tests)
- Enhanced health checks with PostgreSQL monitoring
- **Test Results**: 114/115 tests passing (1 unrelated Swagger test failure)

**Phase 4**: Auto-migration feature and test infrastructure complete
- Implemented environment-aware auto-migration logic with MIGRATE_AT_STARTUP flag
- Enhanced appsettings.Development.json with PostgreSQL configuration and EF Core logging
- Created comprehensive .env.example with PostgreSQL setup documentation
- Resolved EF Core version conflicts across all test projects (9.0.8 consistency)
- Fixed TestWebApplicationFactory with proper health check overrides for test isolation
- **Test Results**: All 115 tests passing (57 Infrastructure + 10 Application + 48 Domain)

### 🚧 **Next Phases (5-8)**
**Phase 5**: Advanced repository features and optimization  
**Phase 6**: Testcontainers-based integration tests
**Phase 7**: End-to-end persistence wiring with use cases
**Phase 8**: Development experience and documentation updates

## Implementation Steps

### Phase 0 – Baseline and guardrails
- [✅] **CLEANUP COMPLETED**: Reverted all partial EF Core work to achieve clean in-memory baseline
  - Removed all EF Core packages from UserService.csproj  
  - Removed Infrastructure/Persistence/ directory and EfUserRepository.cs
  - Cleaned Program.cs to use only InMemoryUserRepository
  - Fixed appsettings.Development.json with minimal config
  - **RESULT**: All 125 tests passing with clean in-memory implementation
- [ ] Add a feature flag for guarded auto-migration in Development (MIGRATE_AT_STARTUP=true)

### Phase 1 – Dependencies and local Postgres
- [✅] Add NuGet dependencies to UserService:
  - [✅] Npgsql.EntityFrameworkCore.PostgreSQL (9.0.4)
  - [✅] Microsoft.EntityFrameworkCore.Design (9.0.8) 
  - [✅] AspNetCore.HealthChecks.NpgSql (9.0.0) for DB health check
- [✅] Add Test dependencies to tests projects:
  - [✅] DotNet.Testcontainers (1.6.0)
- [✅] Extend `src/UserService/docker-compose.yml` with a `postgres` service (postgres:16-alpine), volume, healthcheck, and port mapping; define default env (POSTGRES_USER, POSTGRES_PASSWORD, POSTGRES_DB)
- [✅] Update `scripts/setup.sh` to optionally start Postgres via compose and export a `POSTGRES_CONNECTION` suitable for local runs

### Phase 2 – Application ports  
- [✅] Define `Application/Ports/IUserRepository` with:
  - [✅] Task<bool> ExistsByEmailAsync(Email email)
  - [✅] Task<bool> ExistsByUsernameAsync(Username username)
  - [✅] Task AddAsync(User user)
  - [✅] Task<User?> GetByIdAsync(Guid id)
  - [✅] Task<User?> FindByEmailAsync(Email email)
- [✅] Verify `User` entity in Domain supports these needs (already exists with value objects)
- [✅] Update `InMemoryUserRepository` implementation to match new interface
- [✅] Update `RegisterUserHandler` and all test code to use new method signatures

### Phase 3 – Infrastructure persistence
- [✅] Create `Infrastructure/Persistence/FinmanDbContext` with a Users DbSet
- [✅] Add `Infrastructure/Persistence/Configurations/UserConfiguration` with Fluent API:
  - [✅] Primary key, property lengths
  - [✅] **Value object conversions using OwnsOne pattern**:
    - [✅] `Email`: Store as CITEXT, use `HasConversion<string>()` with Email's implicit operators
    - [✅] `Username`: Store as CITEXT, use `HasConversion<string>()` with Username's implicit operators  
    - [✅] `PersonName`: Use `OwnsOne()` to map FirstName/LastName as separate TEXT columns
  - [✅] Enable citext via migration SQL; map Email/Username columns to `citext` 
  - [✅] Unique indexes on email and username
  - [✅] Timestamps with UTC handling
- [✅] Create initial migration `20250815163136_InitialUsers`:
  - [✅] Execute `CREATE EXTENSION IF NOT EXISTS citext;`
  - [✅] Create table `users` with:
    - [✅] id UUID (PK)
    - [✅] email CITEXT UNIQUE
    - [✅] username CITEXT UNIQUE
    - [✅] first_name TEXT, last_name TEXT
    - [✅] password_hash TEXT
    - [✅] created_at TIMESTAMPTZ, updated_at TIMESTAMPTZ
    - [✅] is_active BOOLEAN, is_deleted BOOLEAN, deleted_at TIMESTAMPTZ
- [✅] Create `Infrastructure/Persistence/Repositories/EfUserRepository` implementation:
  - [✅] Implement all IUserRepository methods with proper async patterns
  - [✅] Handle value object conversions with EF Core mappings
  - [✅] Map `DbUpdateException` (unique violations) to domain-specific exceptions
  - [✅] PostgreSQL-specific error handling and case-insensitive queries
- [✅] Wire up environment-aware DI configuration in `Program.cs`:
  - [✅] Use EfUserRepository for Production/Development environments
  - [✅] Maintain InMemoryUserRepository for Test environment
  - [✅] Register FinmanDbContext with PostgreSQL connection
  - [✅] Add PostgreSQL health checks alongside existing self-check

### Phase 4 – DI wiring and health
- [✅] In `Program.cs`:
  - [✅] Register DbContext with UseNpgsql(POSTGRES_CONNECTION)
  - [✅] Add health checks: `.AddNpgSql(POSTGRES_CONNECTION)` alongside existing self check
  - [✅] Environment-aware configuration: PostgreSQL for Production/Development, InMemory for Test
  - [✅] On startup in Development and when `MIGRATE_AT_STARTUP=true`, run `db.Database.Migrate()`
- [✅] Register repository adapter: `IUserRepository` -> `EfUserRepository`
- [✅] Enhanced configuration system:
  - [✅] Added MIGRATE_AT_STARTUP flag with safe defaults (false)
  - [✅] Created comprehensive .env.example with PostgreSQL setup instructions
  - [✅] Enhanced appsettings.Development.json with connection strings and EF Core logging
- [✅] Test infrastructure improvements:
  - [✅] Fixed EF Core version conflicts across all test projects (9.0.8 consistency)
  - [✅] Enhanced TestWebApplicationFactory with proper service overrides for PostgreSQL health checks
  - [✅] **Result**: All 115 tests passing

### Phase 5 – Repository adapter
- [✅] Implement `Infrastructure/Persistence/Repositories/EfUserRepository` with the port methods
- [✅] Handle value object conversions properly (EF will use the configured conversions)
- [✅] Map `DbUpdateException` (unique violations) to domain-specific errors where needed

### Phase 6 – Integration tests (Testcontainers) - SWITCHABLE APPROACH
**Design Decision**: Implement switchable test infrastructure using environment variable `USE_TESTCONTAINERS=true/false` to allow running same tests with InMemory (fast feedback) or PostgreSQL (realistic integration).

**Increment 1: PostgreSQL Testcontainer Fixture - HYBRID APPROACH**
- [✅] Create `PostgreSqlFixture : IAsyncLifetime` structure (completed)
- [⚠️] **Docker-in-Docker Issue Discovered**: Testcontainers ResourceReaper fails in devcontainer environment
  - Error: `no container with ID [id] found in database: no such container`
  - Common issue with Docker-in-Docker setups in devcontainers
  - **Current Solution**: Hybrid approach using existing PostgreSQL container for immediate progress
  - **Future Optimization**: Document proper Testcontainers devcontainer setup for Phase 6B

**Increment 1A: Hybrid PostgreSQL Fixture (Current) - COMPLETE ✅**
- [✅] Modify `PostgreSqlFixture` to connect to existing PostgreSQL container (`finman-postgres-1`)
  - **Solution**: Used `host.docker.internal:5432` to reach exposed PostgreSQL port from devcontainer
  - **Credentials**: `finman/finman_dev_password` from docker-compose.yml configuration
- [✅] Use dynamic database names for test isolation (`finman_test_{guid}`)
  - **Implementation**: Each test run creates unique database with GUID suffix
  - **Isolation**: Complete database-level isolation between test runs
- [✅] Implement database cleanup after test completion
  - **Cleanup Process**: Terminates active connections, then drops test database
  - **Resource Management**: Proper disposal through `IAsyncLifetime.DisposeAsync()`
- [✅] Add smoke tests to verify functionality
  - **Tests Added**: `PostgreSqlFixtureTests.cs` with 3 comprehensive tests
  - **Coverage**: Connection string validation, migration application, schema verification
  - **Result**: All tests passing (97ms execution time)

**Increment 1B: True Testcontainers Integration (Future)**
- [ ] **Environment Setup**: Configure devcontainer for proper Docker-in-Docker support
  - [ ] Add `DOCKER_HOST` configuration for Testcontainers
  - [ ] Configure ResourceReaper settings for devcontainer compatibility
  - [ ] Test with `--privileged` mode if needed
- [ ] **Alternative**: Use Docker Compose services instead of Testcontainers
- [ ] **Reference**: Research Testcontainers devcontainer best practices

**Increment 2: Testcontainer-enabled WebApplicationFactory - COMPLETE ✅**  
- [✅] Create `TestcontainersWebApplicationFactory` that:
  - [✅] Accepts PostgreSqlFixture in constructor
  - [✅] Overrides connection string configuration to use container
  - [✅] Maintains same service configuration patterns as existing TestWebApplicationFactory
  - [✅] Properly registers EF Core DbContext with PostgreSQL connection
  - [✅] Replaces InMemoryUserRepository with EfUserRepository for realistic testing
- [✅] Update test base class to switch between factories based on `USE_TESTCONTAINERS` env var
  - [✅] Created `SwitchableWebApplicationFactoryTests` base class  
  - [✅] Environment variable switching: `USE_TESTCONTAINERS=true/false`
  - [✅] Automatic factory selection with proper disposal
  - [✅] Transparent mode detection for test clarity

**Testing Results:**
- [✅] **PostgreSQL Mode**: All 3 integration tests passing (405ms execution)
- [✅] **InMemory Mode**: Basic functionality working (health check + registration)
- [⚠️] **Known Issue**: InMemory mode test isolation with duplicate email constraint (existing codebase behavior)
- [✅] **Mode Detection**: Proper environment variable detection and reporting

**Increment 3: Database Integration Tests**
- [ ] Migration smoke test: Db schema exists, unique indexes present
- [ ] Repository integration tests (running in both InMemory and PostgreSQL modes):
  - [ ] Add user (happy path) with value objects
  - [ ] Duplicate email rejected (unique constraint)  
  - [ ] Duplicate username rejected (unique constraint)
  - [ ] FindByEmail returns expected user with proper value object reconstruction
- [ ] Document how to run tests in both modes

### Phase 7 – Use case persistence wiring
- [ ] Update the registration use case to call `IUserRepository` for exists-checks and persistence
- [ ] Map infrastructure exceptions to application DTO errors for duplicate email/username
- [ ] Add end-to-end test via WebApplicationFactory hitting the registration endpoint, using Testcontainers DB

### Phase 8 – Dev UX and docs
- [ ] Update README and scripts docs:
  - [ ] How to start local Postgres (compose)
  - [ ] How to run migrations and enable auto-migrate in Development
  - [ ] Environment variables (POSTGRES_CONNECTION, MIGRATE_AT_STARTUP)
- [ ] Provide `.env.example` with sensible defaults

### Phase 9 – Optional CI wiring (deferred if no CI yet)
- [ ] Ensure CI runner has Docker available
- [ ] Run tests; Testcontainers will pull and run postgres automatically

## Success Criteria and Tests
Success criteria:
- Build and tests pass locally and in CI
- Local run uses Postgres when `POSTGRES_CONNECTION` is provided
- `/health` shows healthy when DB is reachable; remains healthy with self-check only when DB is not configured
- Registration (when wired) persists users and enforces unique email/username

Tests to implement:
- Infrastructure: migrations apply cleanly; schema matches expectations (users table, citext/unique indexes)
- Repository: add/find/exists; duplicate email/username raise expected domain/application errors
- Web integration: registration happy path and duplicate scenarios
- Health check: DB health check returns unhealthy when pointing to non-existent DB (non-flaky threshold)

## Working Area Scratchpad
- **Value Object Handling**: Critical EF Core mapping strategy for our rich domain objects:
  - **Email value object**: Use `HasConversion<string>()` to leverage Email's implicit string conversion operators
    - Database: Store as CITEXT for case-insensitive uniqueness
    - EF mapping: `Email email` property ↔ `string` column using implicit operators
  - **Username value object**: Use `HasConversion<string>()` to leverage Username's implicit string conversion operators
    - Database: Store as CITEXT for case-insensitive uniqueness  
    - EF mapping: `Username username` property ↔ `string` column using implicit operators
  - **PersonName value object**: Use `OwnsOne()` pattern for complex value object
    - Database: Store as separate `first_name` and `last_name` TEXT columns
    - EF mapping: `PersonName name` property ↔ `{ FirstName, LastName }` columns
    - Configuration: `entity.OwnsOne(u => u.Name, name => { name.Property(n => n.FirstName).HasColumnName("first_name"); name.Property(n => n.LastName).HasColumnName("last_name"); });`
- **Password Hash**: Store as TEXT (BCrypt produces string hashes)
- **Case-insensitive uniqueness**: Use `citext` PostgreSQL extension for Email/Username columns
- **Timestamps**: Use application-set timestamps (already in User entity with CreatedAt/UpdatedAt)
- **Entity construction**: User has private constructor + static factory - EF Core will handle correctly
- **Future considerations**: Optimistic concurrency via xmin, audit fields, paginated queries with created_at index
