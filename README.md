# Finman - Financial Management Platform

A comprehensive financial management platform built with microservices architecture, designed for investment tracking, portfolio management, and financial analytics.

**For AI Agents**: Please read [AI-AGENT.md](AI-AGENT.md) for development guidelines and working instructions.

## What is Finman?

Finman is a modern financial management platform designed to provide comprehensive investment tracking, portfolio management, and financial reporting capabilities. Built using a microservices architecture within a monorepo structure, it delivers scalable, maintainable financial services with a unified user experience.

### Key Features
- **User Management**: Secure user registration and authentication
- **Investment Tracking**: Real-time investment data and calculations *(planned)*
- **Portfolio Management**: Advanced portfolio analytics and management *(planned)*
- **Financial Reporting**: Comprehensive reports and analytics *(planned)*
- **API-First Design**: RESTful APIs with comprehensive documentation

## Quick Start

Get up and running in minutes:

### 1. Prerequisites
- **.NET 8.0 SDK**
- **Docker & Docker Compose**

### 2. Clone and Setup
```bash
git clone https://github.com/roblevine/finman.git
cd finman

# Setup the entire project
./scripts/setup.sh
```

### 3. Start the UserService
```bash
# Start database
docker-compose up postgres -d

# Run the UserService
cd services/user-service
./scripts/run.sh --local
```

### 4. Explore the API
Visit [http://localhost:5001/swagger](http://localhost:5001/swagger) to explore the API documentation and test endpoints.

### 5. Run Tests
```bash
# Test all services
./scripts/test.sh

# Or test specific service
cd services/user-service
./scripts/test.sh
```

## Architecture Overview

Finman uses a **monorepo microservices architecture** that balances service independence with development efficiency:

```
finman/
├── services/
│   ├── user-service/              # ✅ User authentication & management
│   ├── shared/                    # 📚 Shared libraries and utilities
│   └── template-service/          # 🛠️ Template for new services
├── frontend/                      # 🚀 Next.js SPA (planned)
├── infrastructure/                # 🐳 Docker & deployment configs
├── docs/                          # 📖 Technical documentation
└── scripts/                       # ⚡ Build, test, and run automation
```

### Current Status

#### ✅ Completed
- **User Service**: Full user registration and authentication with BCrypt security
- **Monorepo Structure**: Multi-service development environment with orchestration
- **PostgreSQL Integration**: Persistent storage with Entity Framework Core
- **Comprehensive Testing**: 114+ tests across all architectural layers
- **Docker Development**: Containerized development and deployment

#### 🚧 In Progress  
- **Documentation Reorganization**: Streamlining and consolidating project documentation

#### 📋 Planned
- **Investment Service**: Real-time investment data and calculations
- **Portfolio Service**: Portfolio management and analytics
- **Reporting Service**: Financial reports and dashboards
- **Frontend Application**: React-based web interface
- **Notification Service**: User notifications and alerts

### Architectural Principles

Each service follows **Hexagonal Architecture** (Ports and Adapters) ensuring:
- **Clean Separation**: Business logic isolated from external dependencies
- **High Testability**: Domain logic can be tested independently  
- **Flexibility**: Easy to swap databases, APIs, or external services
- **Maintainability**: Clear boundaries and responsibilities

*For detailed technical architecture and design patterns, see [ARCHITECTURE.md](ARCHITECTURE.md).*

## Development

### Working with Individual Services

Each service is independently developable:

```bash
cd services/user-service

# Development cycle
./scripts/test.sh              # Run comprehensive test suite
./scripts/build.sh             # Build service and Docker image
./scripts/run.sh --local       # Run locally with hot reload
./scripts/run.sh --docker      # Run in Docker container

# API endpoints available at:
# http://localhost:5001         (local)
# http://localhost:8080         (Docker)
```

### Working with the Full Monorepo

Orchestrate multiple services from the root:

```bash
# From project root
./scripts/test.sh      # Test all services and shared libraries  
./scripts/build.sh     # Build all services and create Docker images
./scripts/run.sh       # Run services with orchestration
./scripts/clean.sh     # Clean all build artifacts
```

### Creating New Services

Use the provided template for consistent service structure:

```bash
# Create new service from template
cp -r services/template-service services/my-new-service
cd services/my-new-service

# Follow template README for customization
```

### Database Development

Finman uses PostgreSQL with Docker for development:

```bash
# Start database
docker-compose up postgres -d

# Connection available at:
# Host: localhost:5432 (from host)
# Host: postgres:5432 (from containers)
# Database: finman
# User: finman / finman_dev_password
```

## API Documentation

### UserService Endpoints

- **GET** `/api/hello` - Service health check with version info
- **GET** `/api/hello/{name}` - Personalized greeting  
- **POST** `/api/auth/register` - User registration
- **GET** `/health` - Health check endpoint

### Interactive API Documentation
- **Development**: [http://localhost:5001/swagger](http://localhost:5001/swagger)
- **OpenAPI Spec**: [http://localhost:5001/swagger/v1/swagger.json](http://localhost:5001/swagger/v1/swagger.json)

## Testing

Comprehensive testing strategy across all architectural layers:

```bash
# Run all tests with coverage
./scripts/test.sh

# Coverage reports generated in:
# services/*/TestResults/**/coverage.cobertura.xml
```

### Test Structure
- **Domain Tests**: Business logic validation (framework-free)
- **Application Tests**: Use case and workflow testing
- **Infrastructure Tests**: API endpoints, database integration, and external services

### Current Test Results
- **Total Tests**: 114+ passing tests
- **Domain Layer**: 48 tests - Core business logic
- **Application Layer**: 10 tests - Use cases and workflows  
- **Infrastructure Layer**: 56+ tests - APIs, persistence, and integrations

## Contributing

We welcome contributions! Here's how to get involved:

### Development Methodology

1. **Analyze**: Understand requirements and break down problems
2. **Plan**: Document approach (create PLAN-*.md for major features)
3. **Execute**: Implement in small, testable increments
4. **Review**: Verify functionality and update documentation

### Guidelines

- **Test-First Development**: Write tests before implementing features
- **Clean Architecture**: Follow Hexagonal Architecture patterns
- **Service Independence**: Ensure services can be developed and deployed independently
- **Comprehensive Documentation**: Update relevant documentation with changes

### Getting Help

- **Technical Architecture**: See [ARCHITECTURE.md](ARCHITECTURE.md)
- **Development Workflow**: See [DEVELOPMENT.md](DEVELOPMENT.md)
- **Database Setup**: See [docs/database/POSTGRESQL.md](docs/database/POSTGRESQL.md)
- **Current Work**: See [TODO.md](TODO.md)
- **Implementation Plans**: Browse [plans/](plans/) directory

## Project Structure

### Key Documentation
- **[README.md](README.md)** *(this file)* - Project overview and quick start
- **[AI-AGENT.md](AI-AGENT.md)** - AI agent guidelines and instructions
- **[ARCHITECTURE.md](ARCHITECTURE.md)** - Detailed technical architecture
- **[DEVELOPMENT.md](DEVELOPMENT.md)** - Comprehensive development guide
- **[TODO.md](TODO.md)** - Current status and development roadmap

### Directory Structure
- **[services/](services/)** - Microservices and shared libraries
- **[docs/](docs/)** - Technical documentation and guides
- **[plans/](plans/)** - Implementation plans and architectural decision records
- **[scripts/](scripts/)** - Build, test, and deployment automation
- **[infrastructure/](infrastructure/)** - Docker configs and deployment manifests

## License

*License information to be added*

---

**Ready to contribute?** Start with the [Development Guide](DEVELOPMENT.md) or explore the [UserService](services/user-service/) to see the architecture in action.
