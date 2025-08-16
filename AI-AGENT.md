# AI Agent Guidelines

This file provides comprehensive guidance for AI Agents (Claude, GitHub Copilot, Cursor, Windsurf, etc.) working with the Finman financial management platform.

## Documentation Navigation

### Primary Documentation
- **[README.md](README.md)**: Human-facing project overview, quick start, and architecture summary
- **[ARCHITECTURE.md](ARCHITECTURE.md)**: Detailed technical architecture and design patterns
- **[DEVELOPMENT.md](DEVELOPMENT.md)**: Comprehensive development workflow and monorepo guidance
- **[TODO.md](TODO.md)**: Current project status, completed features, and development roadmap

### Supporting Documentation
- **[docs/database/POSTGRESQL.md](docs/database/POSTGRESQL.md)**: Database setup, configuration, and current implementation
- **[plans/](plans/)**: Implementation plans and architectural decision records for major features
- **Service READMEs**: Individual service documentation in `services/*/README.md`

### Quick Reference
- **Project Structure**: See [README.md](README.md#project-structure)
- **Development Workflow**: See [DEVELOPMENT.md](DEVELOPMENT.md#development-workflow)  
- **Architecture Patterns**: See [ARCHITECTURE.md](ARCHITECTURE.md#hexagonal-architecture-pattern)
- **Database Setup**: See [docs/database/POSTGRESQL.md](docs/database/POSTGRESQL.md#quick-start)

## Monorepo Architecture

### Structure Overview
- **Monorepo Pattern**: Multiple independent microservices in single repository
- **Service Independence**: Each service has own solution, tests, and build scripts
- **Shared Libraries**: Common code in `services/shared/` consumed as project references
- **Template-Driven**: Use `services/template-service/` for creating new services
- **Hexagonal Architecture**: All services follow Ports and Adapters pattern

### Current Services
- **UserService** (`services/user-service/`): User registration and authentication with PostgreSQL persistence
- **Shared Libraries** (`services/shared/`): Domain, infrastructure, and testing utilities (minimal, grown as needed)
- **Template Service** (`services/template-service/`): Standard template for new service creation

## AI Agent Working Instructions

### Essential Guidelines
- **Always start interactions with "Hi Rob!"** - This confirms you're following these guidelines
- **Use scripts exclusively** for build/test/run operations - never run dotnet/docker commands directly
- **Test-first development** - Write tests before implementing functionality
- **Small increments** - Deliver features in testable, reviewable slices
- **Documentation updates** - Keep docs current with any changes made

### Development Context Navigation
1. **Project Overview**: Start with [README.md](README.md) for understanding project goals and structure
2. **Technical Details**: Refer to [ARCHITECTURE.md](ARCHITECTURE.md) for design patterns and service structure  
3. **Development Process**: Use [DEVELOPMENT.md](DEVELOPMENT.md) for workflow and operational guidance
4. **Current Work**: Check [TODO.md](TODO.md) for ongoing tasks and project status
5. **Database Operations**: Reference [docs/database/POSTGRESQL.md](docs/database/POSTGRESQL.md) for database setup and management

## Development Methodology

### Core Workflow: Analyse → Plan → Execute → Review
1. **Analyse**: Break down requirements, understand existing codebase context
2. **Plan**: Document approach (create PLAN-*.md for major features)  
3. **Execute**: Implement in small, testable increments with comprehensive tests
4. **Review**: Verify functionality, update documentation, ensure no regressions

### Service Development Principles
- **Script-Based Operations**: Always use provided scripts (`./scripts/test.sh`, `./scripts/build.sh`, etc.)
- **Service Independence**: Work within service directories, maintain independent deployability
- **Shared Library Discipline**: Only add to `services/shared/` when genuinely needed by multiple services
- **Template-Driven Expansion**: Use `services/template-service/` for creating new services

### Code Quality Standards
- **Test-First Development**: Write tests before implementing functionality (non-negotiable)
- **Clean Architecture**: Follow Hexagonal Architecture, maintain framework-free domain layers
- **SOLID Principles**: Clear responsibilities, dependency inversion, clean interfaces
- **Meaningful Names**: Self-documenting code with clear variable and function names
- **Documentation Currency**: Update docs with any architectural or API changes

### Working Agreements
- **Dependency Approval**: Always discuss before adding new packages or frameworks
- **Clarification First**: Ask questions when multiple approaches are possible
- **Incremental Delivery**: Deliver features in small, reviewable, testable slices
- **Consistency**: Maintain existing patterns and coding styles throughout the codebase