# PLAN-0001: User Registration Feature

**Status:** IN PROGRESS  
**Started:** August 8, 2025  
**Approach:** Test-First Development, Hexagonal Architecture

## Overview

Implement comprehensive user registration functionality for the Finman User Service. This plan follows a test-first approach using hexagonal architecture principles, building incrementally from domain layer through application layer to infrastructure.

## Scope & Boundaries

### ✅ In Scope
- User registration with email, username, password, and personal details
- Comprehensive validation (email format, username rules, password requirements)
- Uniqueness checking for email and username
- Secure password hashing
- REST API endpoint: `POST /api/auth/register`
- Complete test coverage (unit, integration)
- Error handling with appropriate HTTP status codes

### ❌ Out of Scope (Future Plans)
- Database persistence (using in-memory for now)
- JWT authentication/login flow
- Email verification
- Password reset functionality
- User profile management

## Architecture

### Domain Layer
- **Entities:** `User`
- **Value Objects:** `Email`, `Username`, `PersonName`
- **Exceptions:** `UserDomainException`

### Application Layer
- **Ports:** `IUserRepository`, `IPasswordHasher`
- **Use Cases:** `RegisterUserHandler`
- **DTOs:** `RegisterRequest`, `RegisterResponse`

### Infrastructure Layer
- **Repositories:** `InMemoryUserRepository` (temporary)
- **Controllers:** `AuthController`
- **Password Hashing:** Implementation of `IPasswordHasher`

## API Contract

### Request
```
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "username": "johndoe",
  "firstName": "John",
  "lastName": "Doe", 
  "password": "SecurePass123!"
}
```

### Responses
```
201 Created
Location: /api/users/{id}
{
  "id": "uuid",
  "email": "user@example.com",
  "username": "johndoe", 
  "fullName": "John Doe",
  "createdAt": "2025-08-08T20:30:00Z"
}

400 Bad Request - Validation errors
409 Conflict - Email or username already exists
500 Internal Server Error - Unexpected failures
```

## Implementation Task List

### Phase 1: Foundation & Domain ✅
- [x] **T001:** Domain entities and value objects
  - [x] User entity with factory method
  - [x] Email value object with validation
  - [x] Username value object with rules
  - [x] PersonName value object
  - [x] UserDomainException
- [x] **T002:** Domain unit tests (48 tests)
  - [x] User entity creation and validation
  - [x] Value object validation and behavior
  - [x] Edge cases and error conditions
- [x] **T003:** Application ports definition
  - [x] IUserRepository interface
  - [x] IPasswordHasher interface

### Phase 2: Application Layer Testing ✅
- [x] **T004:** Test infrastructure setup
  - [x] Application.Tests project creation
  - [x] xUnit and FluentAssertions configuration
  - [x] Project references and solution integration
- [x] **T005:** Mock implementations for testing
  - [x] MockUserRepository with in-memory storage
  - [x] MockPasswordHasher with deterministic behavior
  - [x] Configurable test scenarios (force conflicts, etc.)
- [x] **T006:** Comprehensive unit tests (10 tests)
  - [x] RegisterUser success scenario
  - [x] Email uniqueness validation (409 Conflict)
  - [x] Username uniqueness validation (409 Conflict)
  - [x] Input validation for all fields (400 Bad Request)
  - [x] Password hashing verification
  - [x] Mock repository edge cases

### Phase 3: Application Layer Implementation ✅
- [x] **T007:** Data Transfer Objects
  - [x] RegisterRequest DTO with validation attributes
  - [x] RegisterResponse DTO
- [x] **T008:** RegisterUserHandler implementation
  - [x] ExecuteAsync method with full business logic
  - [x] Email/username uniqueness checking
  - [x] Password hashing integration
  - [x] Domain entity creation and persistence
  - [x] Error handling and mapping
- [x] **T009:** Validation integration
  - [x] Make all unit tests pass
  - [x] Verify error scenarios work correctly

### Phase 4: Infrastructure Layer 🚧
- [ ] **T010:** In-memory repository implementation
  - [ ] InMemoryUserRepository class
  - [ ] Thread-safe operations
  - [ ] Uniqueness constraint enforcement
- [ ] **T011:** Password hashing service
  - [ ] BCrypt implementation
  - [ ] Secure hash generation and verification
- [ ] **T012:** Dependency injection setup
  - [ ] Service registration in Program.cs
  - [ ] Configuration for development/test environments

### Phase 5: API Layer 🚧
- [ ] **T013:** AuthController implementation
  - [ ] POST /api/auth/register endpoint
  - [ ] Request validation and model binding
  - [ ] Response formatting and status codes
  - [ ] Error handling middleware integration
- [ ] **T014:** OpenAPI documentation
  - [ ] Swagger endpoint documentation
  - [ ] Example requests and responses
  - [ ] Error response schemas

### Phase 6: Integration Testing 🚧
- [ ] **T015:** API integration tests
  - [ ] Successful registration (201)
  - [ ] Duplicate email handling (409)
  - [ ] Duplicate username handling (409)  
  - [ ] Invalid input handling (400)
  - [ ] Content-Type and response format validation
- [ ] **T016:** End-to-end scenarios
  - [ ] Full request/response cycle testing
  - [ ] Location header validation
  - [ ] Error response format consistency

## Current Status

### ✅ Completed (Phase 1-3)
- **Domain layer complete:** All entities, value objects, and domain logic implemented
- **Domain tests passing:** 48/48 tests covering all domain scenarios  
- **Infrastructure tests passing:** 22/22 tests for existing API endpoints
- **Application test scaffolding:** 10/10 comprehensive unit tests for RegisterUser
- **Mock implementations:** Full test doubles for IUserRepository and IPasswordHasher
- **Test infrastructure:** Application.Tests project integrated into solution
- **Application layer complete:** RegisterUserHandler with full business logic
- **DTOs implemented:** RegisterRequest with validation, RegisterResponse for output
- **All unit tests passing:** Complete test coverage for registration use case

### 🚧 In Progress (Phase 4)
- **Next task:** T010-T012 - Infrastructure layer implementation
- **Focus:** InMemoryUserRepository, BCrypt password hashing, DI configuration

### 📊 Test Results
```
Total Tests: 80/80 passing
├── Domain.Tests: 48/48 ✅
├── Infrastructure.Tests: 22/22 ✅  
└── Application.Tests: 10/10 ✅
```

## Success Criteria

### Definition of Done
- [ ] All unit tests pass (currently 80/80 ✅)
- [ ] All integration tests pass
- [ ] API endpoint returns correct status codes and responses
- [ ] Error handling is comprehensive and user-friendly
- [ ] Code coverage meets quality standards
- [ ] Documentation is complete and accurate
- [ ] No security vulnerabilities in password handling

### Quality Gates
- **Test Coverage:** Maintain >90% code coverage
- **Security:** Passwords never stored in plaintext
- **Performance:** Registration completes in <100ms
- **Validation:** All input validation rules enforced
- **API Design:** RESTful conventions followed

## Risk Assessment

### High Priority Risks
- **Password Security:** Ensure BCrypt implementation is correct ⚠️
- **Uniqueness Race Conditions:** Handle concurrent registration attempts ⚠️
- **Input Validation:** Prevent injection attacks and malformed data ⚠️

### Medium Priority Risks
- **Error Message Leakage:** Don't expose internal system details 🟡
- **API Contract Changes:** Maintain backward compatibility 🟡

### Mitigations
- Comprehensive unit and integration tests
- Security-focused code reviews
- Input validation at multiple layers
- Proper error handling and logging

## Future Considerations

### Next Plans (Not in Scope)
- **PLAN-0002:** Database Integration (PostgreSQL + EF Core)
- **PLAN-0003:** JWT Authentication & Login
- **PLAN-0004:** Email Verification
- **PLAN-0005:** User Profile Management

### Technical Debt
- Replace in-memory repository with proper database
- Add proper logging infrastructure
- Implement rate limiting for registration endpoint
- Add monitoring and health checks

## Dependencies

### Internal
- Domain layer (User, Email, Username, PersonName) ✅
- Infrastructure layer (HelloController, health checks) ✅
- Application layer (RegisterUserHandler, DTOs) ✅

### External
- xUnit testing framework ✅
- ASP.NET Core Web API ✅
- BCrypt.Net-Next (for password hashing) - **Next Phase**
- Swashbuckle (OpenAPI/Swagger) ✅

## Implementation Summary (August 8, 2025)

### Phase 1-3 Completed Successfully
This implementation follows a comprehensive test-first approach using hexagonal architecture. Key achievements:

**Architecture Compliance:**
- Removed FluentAssertions per ARCHITECTURE.md constraints
- Converted all test assertions to xUnit native methods
- Maintained 80/80 passing tests throughout

**Domain Layer (Phase 1):**
- Complete User entity with factory method and validation
- Value objects: Email, Username, PersonName with comprehensive validation
- UserDomainException for domain constraint violations
- 48/48 domain tests covering all scenarios

**Application Layer Testing (Phase 2):**
- MockUserRepository with in-memory storage and configurable behavior
- MockPasswordHasher with deterministic test behavior  
- 10 comprehensive unit tests covering success, validation, uniqueness, hashing

**Application Layer Implementation (Phase 3):**
- RegisterRequest DTO with DataAnnotations validation
- RegisterResponse DTO for structured output
- RegisterUserHandler with complete business logic:
  - Input validation using domain value objects
  - Email/username uniqueness checking
  - Password hashing integration
  - Domain entity creation and persistence
  - Comprehensive error handling with appropriate exceptions

**Key Files Created:**
```
src/UserService/Application/
├── DTOs/
│   ├── RegisterRequest.cs     ✅ Input validation
│   └── RegisterResponse.cs    ✅ Output formatting
└── UseCases/
    └── RegisterUserHandler.cs ✅ Complete business logic
```

**Test Coverage:**
- Domain: 48/48 tests ✅
- Infrastructure: 22/22 tests ✅  
- Application: 10/10 tests ✅
- **Total: 80/80 tests passing**

### Next Session Priorities (Phase 4)
1. **T010:** InMemoryUserRepository implementation
   - Thread-safe operations using ConcurrentDictionary
   - Proper uniqueness constraint enforcement
   - Integration with existing domain entities

2. **T011:** BCrypt password hashing service
   - Add BCrypt.Net-Next NuGet package
   - Implement IPasswordHasher with secure hash generation
   - Replace mock hasher in integration scenarios

3. **T012:** Dependency injection configuration
   - Register services in Program.cs
   - Configure for Development/Test environments
   - Ensure proper service lifetimes

The foundation is solid with comprehensive test coverage ensuring we can confidently build the infrastructure layer.

## Notes

This plan represents a comprehensive approach to user registration, built incrementally with test-first development. The current implementation is at Phase 2 completion, with solid foundations in place and comprehensive test coverage ensuring quality as we move forward.

The architecture follows hexagonal principles, ensuring the core business logic is isolated from infrastructure concerns, making the system maintainable and testable.
