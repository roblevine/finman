# Template Service

This is a template for creating new services in the Finman monorepo.

## Structure

```
template-service/
├── src/
│   └── TemplateService/
│       ├── Domain/
│       │   ├── Entities/
│       │   ├── ValueObjects/
│       │   └── Exceptions/
│       ├── Application/
│       │   ├── Ports/
│       │   ├── UseCases/
│       │   └── DTOs/
│       ├── Infrastructure/
│       │   ├── Web/
│       │   │   └── Controllers/
│       │   └── Persistence/
│       ├── Program.cs
│       └── TemplateService.csproj
├── tests/
│   ├── Domain.Tests/
│   ├── Application.Tests/
│   └── Infrastructure.Tests/
├── scripts/
│   ├── build.sh
│   ├── test.sh
│   ├── run.sh
│   ├── setup.sh
│   └── clean.sh
├── TemplateService.sln
└── README.md
```

## Creating a New Service

1. Copy this template directory to a new service name (e.g., `investment-service`)
2. Replace all instances of "TemplateService" and "template-service" with your service names
3. Update the solution file and project references
4. Implement your domain models, use cases, and controllers
5. Update the root-level scripts to include your new service
6. Add tests following the established patterns

## Design Principles

- **Hexagonal Architecture**: Domain at the center, infrastructure at the edges
- **Dependency Inversion**: Application layer depends on ports, infrastructure implements adapters
- **Test-First**: Write tests before implementing features
- **SOLID Principles**: Single responsibility, open/closed, Liskov substitution, interface segregation, dependency inversion
- **Clean Code**: Meaningful names, small functions, clear intent

## Scripts

Each service has its own scripts for independent development:

- `./scripts/setup.sh` - Set up the service
- `./scripts/build.sh` - Build the service and run tests
- `./scripts/test.sh` - Run all tests with coverage
- `./scripts/run.sh --local|--docker` - Run the service
- `./scripts/clean.sh` - Clean build artifacts

## Integration with Monorepo

- Services can reference shared libraries in `services/shared/`
- Services communicate via HTTP APIs (no direct assembly references)
- Each service has its own solution file and can be built independently
- Root-level scripts orchestrate multiple services for full-stack development
