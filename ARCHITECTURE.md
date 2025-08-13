# Architecture

This document outlines the architecture of the Finman application, including its components, interactions, and deployment strategies.

## High-Level Architecture

The Finman application follows a **monorepo microservices architecture** with the following characteristics:
- **Monorepo Structure**: Single repository containing multiple independent services
- **Backend**: Multiple .NET microservices using **Hexagonal Architecture** (Ports and Adapters)
- **Frontend**: Next.js SPA (to be implemented as unified UI across all backend services)
- **Deployment**: Independently deployable containerized services
- **Shared Libraries**: Common domain models, infrastructure, and testing utilities

### Monorepo Structure

```
finman/
├── services/
│   ├── user-service/              # User authentication & management
│   │   ├── src/UserService/       # Main application code
│   │   ├── tests/                 # Service-specific tests
│   │   ├── scripts/               # Service build/test/run scripts
│   │   ├── UserService.sln        # Independent solution
│   │   └── README.md             # Service documentation
│   ├── shared/                    # Shared libraries
│   │   ├── Finman.Shared.Domain/           # Common domain models
│   │   ├── Finman.Shared.Infrastructure/   # Common infrastructure
│   │   ├── Finman.Shared.Testing/          # Test utilities
│   │   └── Finman.Shared.sln               # Shared solution
│   └── [future-service]/          # Additional services
├── frontend/
│   └── web-app/                   # Next.js SPA (future)
├── infrastructure/                # Docker, deployment configs
├── scripts/                       # Root-level orchestration scripts
├── docs/                         # Architecture documentation
└── plans/                        # Implementation plans & ADRs
```

### Development Approach
**Backend-First Development**: APIs are developed first to establish core business logic and data models. The frontend will be implemented later and will consume multiple backend services, providing a unified user experience.

### Service Independence
- Each service maintains its own solution file and can be built independently
- Services communicate via HTTP APIs only (no direct assembly references)
- Independent versioning and deployment capabilities
- Shared libraries are consumed as project references (not NuGet packages initially)

## Current Services

### UserService (`services/user-service/`)
Responsible for user authentication and management:

**Endpoints:**
- `GET /api/hello` - Health check with version info
- `GET /api/hello/{name}` - Personalized greeting
- `POST /api/auth/register` - User registration
- `GET /health` - Service health check
- Swagger documentation at `/swagger` (Development only)

**Features:**
- User registration with email/username uniqueness validation
- BCrypt password hashing
- Comprehensive validation and error handling
- Full test coverage (125+ tests across Domain/Application/Infrastructure layers)

## Planned Services

### InvestmentService
- Investment data management
- Real-time price feeds
- Portfolio calculations

### PortfolioService  
- User portfolio management
- Asset allocation tracking
- Performance analytics

### ReportingService
- Financial reports generation
- Historical analysis
- Export capabilities

### NotificationService
- User notifications
- Email/SMS integration
- Event-driven messaging

## Hexagonal Architecture Pattern

Each microservice follows the Hexagonal Architecture pattern to ensure:
- **Separation of Concerns**: Business logic is isolated from external dependencies
- **Testability**: Core domain logic can be tested independently
- **Flexibility**: Easy to swap out adapters (databases, external APIs, etc.)
- **Maintainability**: Clear boundaries between layers

### Architecture Layers

```
┌─────────────────────────────────────────────────────────┐
│                    Adapters (Infrastructure)            │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐    │
│  │   Web API   │  │  Database   │  │ External    │    │
│  │ Controllers │  │  Repository │  │   APIs      │    │
│  └─────────────┘  └─────────────┘  └─────────────┘    │
│           │               │               │            │
└───────────┼───────────────┼───────────────┼────────────┘
            │               │               │
┌───────────┼───────────────┼───────────────┼────────────┐
│           ▼               ▼               ▼            │
│                     Ports (Interfaces)                 │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐    │
│  │    Input    │  │  Repository │  │   Service   │    │
│  │    Ports    │  │    Ports    │  │   Ports     │    │
│  └─────────────┘  └─────────────┘  └─────────────┘    │
└─────────────────┬───────────────────────────────────────┘
                  │
┌─────────────────▼───────────────────────────────────────┐
│                Domain (Core)                            │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐    │
│  │   Entities  │  │   Use Cases │  │   Domain    │    │
│  │             │  │  (Services) │  │   Services  │    │
│  └─────────────┘  └─────────────┘  └─────────────┘    │
└─────────────────────────────────────────────────────────┘
```

### Layer Responsibilities

- **Domain (Core)**: Contains business entities, domain services, and business rules
- **Ports**: Define contracts/interfaces between layers
- **Adapters**: Implement ports to connect to external systems (databases, APIs, web controllers)

## Libraries and frameworks

### DO NOT USE
The follwing libraries should not be considered as part of this solution
- FLuent Assertions