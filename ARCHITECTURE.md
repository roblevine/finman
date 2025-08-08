# Architecture

This document outlines the architecture of the Finman application, including its components, interactions, and deployment strategies.

## High-Level Architecture

The Finman application follows a microservices architecture with the following characteristics:
- **Backend**: Multiple .NET microservices using **Hexagonal Architecture** (Ports and Adapters)
- **Frontend**: Next.js application (to be implemented later as unified UI across all backend services)
- **Deployment**: Containerized services

### Development Approach
**Backend-First Development**: APIs are being developed first to establish the core business logic and data models. The frontend will be implemented later and will operate across multiple backend services, providing a unified user experience.

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