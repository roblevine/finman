# Plan: PostgreSQL persistence and Testcontainers

**Status:** PENDING  
**Started:** 2025-08-10

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
- Verify current solution builds and tests pass (scripts/test.sh)
- Add a feature flag for guarded auto-migration in Development (MIGRATE_AT_STARTUP=true)

### Phase 1 – Dependencies and local Postgres
- Add NuGet dependencies to UserService:
  - Npgsql.EntityFrameworkCore.PostgreSQL
  - Microsoft.EntityFrameworkCore.Design
  - (optional) Microsoft.Extensions.Diagnostics.HealthChecks.NpgSql for DB health check
- Add Test dependencies to tests projects:
  - DotNet.Testcontainers
- Extend `src/UserService/docker-compose.yml` with a `postgres` service (postgres:16-alpine), volume, healthcheck, and port mapping; define default env (POSTGRES_USER, POSTGRES_PASSWORD, POSTGRES_DB)
- Update `scripts/setup.sh` to optionally start Postgres via compose and export a `POSTGRES_CONNECTION` suitable for local runs

### Phase 2 – Application ports
- Define `Application/Ports/IUserRepository` with:
  - Task<bool> ExistsByEmail(string email)
  - Task<bool> ExistsByUsername(string username)
  - Task Add(User user)
  - Task<User?> GetById(Guid id)
  - Task<User?> FindByEmail(string email)
- If not already present, ensure a minimal `User` entity in Domain supports these needs (id/email/username/first/last/password-hash/timestamps)

### Phase 3 – Infrastructure persistence
- Create `Infrastructure/Persistence/FinmanDbContext` with a Users DbSet
- Add `Infrastructure/Persistence/Configurations/UserConfiguration` with Fluent API:
  - Primary key, property lengths
  - Enable citext via migration SQL; map columns to `citext` where applicable or enforce LOWER(...) unique index
  - Unique indexes on email and username
  - Timestamps with default now() and on update trigger if desired (or set via app)
- Create initial migration `20250810_0001_Users`:
  - Execute `CREATE EXTENSION IF NOT EXISTS citext;`
  - Create table `users` with:
    - id UUID (PK)
    - email CITEXT UNIQUE
    - username CITEXT UNIQUE
    - first_name TEXT, last_name TEXT
    - password_hash BYTEA (or TEXT if using encoded) and optional password_salt
    - created_at TIMESTAMPTZ, updated_at TIMESTAMPTZ

### Phase 4 – DI wiring and health
- In `Program.cs`:
  - Register DbContext with UseNpgsql(POSTGRES_CONNECTION)
  - Add health checks: `.AddNpgSql(POSTGRES_CONNECTION)` alongside existing self check
  - On startup in Development and when `MIGRATE_AT_STARTUP=true`, run `db.Database.Migrate()`
- Register repository adapter: `IUserRepository` -> `EfUserRepository`

### Phase 5 – Repository adapter
- Implement `Infrastructure/Persistence/Repositories/EfUserRepository` with the port methods
- Map `DbUpdateException` (unique violations) to domain-specific errors where needed

### Phase 6 – Integration tests (Testcontainers)
- Add a shared xUnit fixture implementing `IAsyncLifetime` that:
  - Spins up a Postgres 16-alpine container with random host port
  - Exposes a connection string
  - Applies EF Core migrations before tests run
- For web integration tests using `TestWebApplicationFactory`, override configuration to inject the container connection string
- Tests to add:
  - Migration smoke test: Db schema exists, unique indexes present
  - Repository tests:
    - Add user (happy path)
    - Duplicate email rejected (unique constraint)
    - Duplicate username rejected (unique constraint)
    - FindByEmail returns expected user

### Phase 7 – Use case persistence wiring
- Update the registration use case to call `IUserRepository` for exists-checks and persistence
- Map infrastructure exceptions to application DTO errors for duplicate email/username
- Add end-to-end test via WebApplicationFactory hitting the registration endpoint (if already present), using Testcontainers DB

### Phase 8 – Dev UX and docs
- Update README and scripts docs:
  - How to start local Postgres (compose)
  - How to run migrations and enable auto-migrate in Development
  - Environment variables (POSTGRES_CONNECTION, MIGRATE_AT_STARTUP)
- Provide `.env.example` with sensible defaults

### Phase 9 – Optional CI wiring (deferred if no CI yet)
- Ensure CI runner has Docker available
- Run tests; Testcontainers will pull and run postgres automatically

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
- Decide on `password_hash` storage format: BYTEA (preferred) vs TEXT base64; adapt EF mapping accordingly
- Consider mapping `email`/`username` as `citext` vs `text` + functional unique index on LOWER(column); prioritize `citext` for simplicity
- Consider `created_at`/`updated_at` source of truth: database default vs application-set timestamps
- Potential follow-up: optimistic concurrency via xmin, soft-deletes, and audit fields
- Potential follow-up: paginated queries on users with index on created_at
