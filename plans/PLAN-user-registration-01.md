# Phase 2 — User Registration (Cycle 1)

Analyse → Plan → Execute → Review

Scope: Implement the first thin slice of user registration focused on application logic and a minimal API endpoint, test-first, without a real database yet. We'll use hand-written mocks for unit tests and a simple in-memory repository for integration tests. A proper PostgreSQL + EF Core adapter will follow in Cycle 2.

## Goals (Cycle 1)
- Application use case for registering a new user.
- Comprehensive unit tests with hand-written mocks (ports).
- Minimal API controller endpoint POST /api/auth/register.
- Happy-path integration test using an in-memory repository.
- Consistent validation and error handling.

Out of scope (defer to Cycle 2): EF Core + PostgreSQL, migrations, Docker Compose wiring for DB, JWT/login.

## Contracts

Input DTO (RegisterRequest):
- email: string (required, valid email)
- username: string (required, 3–20, [a-zA-Z0-9_])
- firstName: string (required, <= 50)
- lastName: string (required, <= 50)
- password: string (required, min 8 chars) — hashed via IPasswordHasher

Output DTO (RegisterResponse):
- id: Guid
- email: string
- username: string
- fullName: string
- createdAt: DateTime (UTC)

HTTP
- POST /api/auth/register
- 201 Created with body RegisterResponse and Location: /api/users/{id}
- 400 Bad Request on validation failures (model validation or VO exceptions mapped to problem details)
- 409 Conflict if email or username already exists
- 500 Internal Server Error for unexpected failures

## Use Case (Application)
RegisterUser
- Inputs: RegisterRequest
- Dependencies (ports):
  - IUserRepository
  - IPasswordHasher
- Steps:
  1) Validate input (basic checks; deeper validation by Value Objects: Email, Username, PersonName)
  2) Check uniqueness: IsEmailUniqueAsync, IsUsernameUniqueAsync
  3) Hash password via IPasswordHasher
  4) Create User domain entity
  5) Persist via IUserRepository.AddAsync
  6) Return RegisterResponse
- Errors:
  - Duplicate email → Conflict
  - Duplicate username → Conflict
  - Invalid values → ArgumentException (map to 400)

## Test Plan (Test-First)
Unit (tests/Application.Tests):
- RegisterUser_Succeeds_WithValidData
- RegisterUser_Fails_WhenEmailExists (409)
- RegisterUser_Fails_WhenUsernameExists (409)
- RegisterUser_Fails_WhenInvalidEmail/Username/Name/Password (400)
- Password_IsHashed_And_VerifiedWithHasher (interaction)

Integration (tests/Infrastructure.Tests):
- POST /api/auth/register returns 201 and RegisterResponse
- Duplicate email → 409
- Duplicate username → 409
- Invalid payload → 400 with validation details

Mocks (tests/Application.Tests/Mocks):
- MockUserRepository : IUserRepository (in-memory behavior, configurable)
- MockPasswordHasher : IPasswordHasher (deterministic hash for tests)

Infra (temporary, for integration tests):
- InMemoryUserRepository (Infrastructure/Persistence/Repositories) registered in DI for Development/Test; swap to EF in Cycle 2.

## Implementation Steps
1) Add DTOs
   - src/UserService/Application/DTOs/RegisterRequest.cs
   - src/UserService/Application/DTOs/RegisterResponse.cs
2) Add Use Case
   - src/UserService/Application/UseCases/RegisterUser/RegisterUserCommand.cs (or reuse RegisterRequest)
   - src/UserService/Application/UseCases/RegisterUser/RegisterUserHandler.cs
3) Tests (unit)
   - tests/Application.Tests/UseCases/RegisterUserTests.cs
   - tests/Application.Tests/Mocks/MockUserRepository.cs
   - tests/Application.Tests/Mocks/MockPasswordHasher.cs
4) Minimal Infra adapter for integration
   - src/UserService/Infrastructure/Persistence/Repositories/InMemoryUserRepository.cs
   - Wire DI in Program.cs for Development/Test only
5) Controller
   - src/UserService/Infrastructure/Web/Controllers/AuthController.cs with POST /api/auth/register
6) Integration tests
   - tests/Infrastructure.Tests/AuthRegistrationTests.cs

## Acceptance Criteria
- All new unit tests pass.
- New integration tests pass using in-memory repo.
- POST /api/auth/register returns 201 with expected payload and Location header.
- Duplicate email/username produce 409.
- Validation errors produce 400 with clear messages.
- No DB dependency added yet; EF Core postponed to Cycle 2.

## Risks & Assumptions
- Assumption: Temporary in-memory repo is acceptable for initial integration tests.
- Risk: Divergence between in-memory and EF implementations; mitigate with shared contract tests later.

## Follow-Up (Cycle 2: Persistence)
- EF Core DbContext, entity configuration, migrations.
- PostgreSQL Docker Compose service; connection string configuration.
- Replace in-memory with EF repository; keep contract tests.
- Add health check for DB.
