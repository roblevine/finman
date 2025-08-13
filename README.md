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
│   ├── shared/                    # Shared libraries (domain, infrastructure, testing)
│   └── template-service/          # Template for creating new services
├── frontend/                      # Next.js SPA (future)
├── infrastructure/                # Docker compose, deployment configs
├── scripts/                       # Root-level build/test/run scripts (working)
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

### Monorepo Scripts

Root-level scripts orchestrate multiple services:

```bash
./scripts/test.sh      # Test all services and shared libraries
./scripts/build.sh     # Build all services and shared libraries
./scripts/run.sh       # Run services (with orchestration options)
./scripts/setup.sh     # Setup entire monorepo
./scripts/clean.sh     # Clean all build artifacts
```

### Service Templates

Create new services using the template:

```bash
# Use services/template-service/ as a starting point
# Follow the README.md in template-service for guidance
cp -r services/template-service services/my-new-service
# Edit and customize the new service
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