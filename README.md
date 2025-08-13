# Finman - Financial Management Platform

**IMPORTANT - PLEASE READ the AI-AGENT.md file for more information on the rules and guidance on contributing to this project.**

## Overview

Finman is a comprehensive financial management platform built as a monorepo containing multiple microservices with a unified Next.js frontend. The platform will eventually provide full investment tracking, portfolio management, and financial reporting capabilities.

## Architecture

This is a **monorepo** structured for microservices development:

```
finman/
├── services/
│   ├── user-service/              # User authentication & management
│   └── shared/                    # Shared libraries (domain, infrastructure, testing)
├── frontend/                      # Next.js SPA (future)
├── infrastructure/                # Docker compose, deployment configs
├── scripts/                       # Root-level build/test/run scripts
├── docs/                         # Architecture and design documentation
└── plans/                        # Implementation plans and ADRs
```

### Current Services

- **UserService** (`services/user-service/`): User registration, authentication, and profile management

### Planned Services

- **InvestmentService**: Investment data and calculations
- **PortfolioService**: Portfolio management and aggregations  
- **ReportingService**: Financial reports and analytics
- **NotificationService**: User notifications

## Development

### Quick Start

Each service can be developed independently:

```bash
# Work on UserService
cd services/user-service
./scripts/test.sh      # Run tests
./scripts/build.sh     # Build and test
./scripts/run.sh --local   # Run locally (http://localhost:5001)
```

### Monorepo Scripts (Future)

Root-level scripts will orchestrate multiple services:

```bash
./scripts/test.sh      # Test all services
./scripts/build.sh     # Build all services  
./scripts/run.sh       # Run full stack locally
```

## Service Architecture

Each service follows **Hexagonal Architecture** (Ports and Adapters):

- **Domain**: Business entities, value objects, domain services
- **Application**: Use cases, DTOs, ports (interfaces)
- **Infrastructure**: Web controllers, repositories, external service adapters

## Getting Started

1. **Prerequisites**: .NET 8.0, Docker (optional)

2. **Clone and explore**:
   ```bash
   git clone <repo-url>
   cd finman
   ```

3. **Start with UserService**:
   ```bash
   cd services/user-service
   ./scripts/setup.sh    # One-time setup
   ./scripts/test.sh     # Verify everything works
   ./scripts/run.sh --local  # Start the service
   ```

4. **API Documentation**: Visit `http://localhost:5001/swagger` (in Development)

## Database (Future)

Local Postgres development support will be added:
- `./scripts/setup.sh --start-postgres` for local development database
- Docker Compose orchestration for full-stack development
- EF Core migrations with `MIGRATE_AT_STARTUP` flag support

## Contributing

See individual service README files for service-specific guidance. Follow the established patterns:

- Test-first development
- Hexagonal Architecture
- Independent service deployability
- Comprehensive documentation