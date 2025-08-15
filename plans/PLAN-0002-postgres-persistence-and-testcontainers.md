# Plan: PostgreSQL persistence and Testcontainers

**Status:** IN PROGRESS  
**Started:** 2025-08-10  
**Resumed:** 2025-08-15

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
- [ ] Define `Application/Ports/IUserRepository` with:
  - [ ] Task<bool> ExistsByEmail(Email email)
  - [ ] Task<bool> ExistsByUsername(Username username)
  - [ ] Task Add(User user)
  - [ ] Task<User?> GetById(Guid id)
  - [ ] Task<User?> FindByEmail(Email email)
- [ ] Verify `User` entity in Domain supports these needs (already exists with value objects)

### Phase 3 – Infrastructure persistence
- [ ] Create `Infrastructure/Persistence/FinmanDbContext` with a Users DbSet
- [ ] Add `Infrastructure/Persistence/Configurations/UserConfiguration` with Fluent API:
  - [ ] Primary key, property lengths
  - [ ] **Value object conversions using OwnsOne pattern**:
    - [ ] `Email`: Store as CITEXT, use `HasConversion<string>()` with Email's implicit operators
    - [ ] `Username`: Store as CITEXT, use `HasConversion<string>()` with Username's implicit operators  
    - [ ] `PersonName`: Use `OwnsOne()` to map FirstName/LastName as separate TEXT columns
  - [ ] Enable citext via migration SQL; map Email/Username columns to `citext` 
  - [ ] Unique indexes on email and username
  - [ ] Timestamps with UTC handling
- [ ] Create initial migration `20250815_0001_Users`:
  - [ ] Execute `CREATE EXTENSION IF NOT EXISTS citext;`
  - [ ] Create table `users` with:
    - [ ] id UUID (PK)
    - [ ] email CITEXT UNIQUE
    - [ ] username CITEXT UNIQUE
    - [ ] first_name TEXT, last_name TEXT
    - [ ] password_hash TEXT
    - [ ] created_at TIMESTAMPTZ, updated_at TIMESTAMPTZ
    - [ ] is_active BOOLEAN, is_deleted BOOLEAN, deleted_at TIMESTAMPTZ

### Phase 4 – DI wiring and health
- [ ] In `Program.cs`:
  - [ ] Register DbContext with UseNpgsql(POSTGRES_CONNECTION)
  - [ ] Add health checks: `.AddNpgSql(POSTGRES_CONNECTION)` alongside existing self check
  - [ ] On startup in Development and when `MIGRATE_AT_STARTUP=true`, run `db.Database.Migrate()`
- [ ] Register repository adapter: `IUserRepository` -> `EfUserRepository`

### Phase 5 – Repository adapter
- [ ] Implement `Infrastructure/Persistence/Repositories/EfUserRepository` with the port methods
- [ ] Handle value object conversions properly (EF will use the configured conversions)
- [ ] Map `DbUpdateException` (unique violations) to domain-specific errors where needed

### Phase 6 – Integration tests (Testcontainers)
- [ ] Add a shared xUnit fixture implementing `IAsyncLifetime` that:
  - [ ] Spins up a Postgres 16-alpine container with random host port
  - [ ] Exposes a connection string
  - [ ] Applies EF Core migrations before tests run
- [ ] For web integration tests using `TestWebApplicationFactory`, override configuration to inject the container connection string
- [ ] Tests to add:
  - [ ] Migration smoke test: Db schema exists, unique indexes present
  - [ ] Repository tests:
    - [ ] Add user (happy path) with value objects
    - [ ] Duplicate email rejected (unique constraint)
    - [ ] Duplicate username rejected (unique constraint)
    - [ ] FindByEmail returns expected user with proper value object reconstruction

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
