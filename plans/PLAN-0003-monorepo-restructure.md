# PLAN-0003: Monorepo Restructure for Multi-Service Architecture

**Status:** PAUSED  
**Started:** August 13, 2025  
**Progress:** Phase 1 & 2 completed - basic structure in place, UserService moved and working

## Overview

Restructure the current single-service repository into a monorepo that can support multiple microservices with a unified frontend. This will establish the foundation for expanding beyond the current UserService to include additional services like InvestmentService, PortfolioService, etc., while maintaining a single Next.js SPA frontend that consumes all backend APIs.

### Scope
- **In Scope:**
  - Restructure existing UserService into new monorepo layout
  - Create framework for future microservices
  - Establish shared libraries structure
  - Update build/test/run scripts for multi-service support
  - Prepare foundation for Next.js frontend integration
  - Maintain independent service versioning/deployment capabilities
  - Support both individual service development and full-stack orchestration

- **Out of Scope:**
  - Implementation of additional microservices (will be separate plans)
  - Next.js frontend implementation (will be separate plan)
  - Kubernetes/production deployment configurations (future plan)

## Requirements

1. **Service Independence**: Each service must remain independently buildable, testable, and deployable
2. **Shared Code Support**: Common concerns (auth, logging, domain models) should be shareable via shared assemblies
3. **Unified Frontend Foundation**: Structure must support a single Next.js SPA consuming multiple APIs
4. **Development Workflow**: Support both individual service development and full-stack local development
5. **CI/CD Ready**: Structure must support independent CI/CD pipelines per service with orchestration capability
6. **Script Consistency**: Maintain existing script-based build/test/run approach
7. **Documentation**: All architectural decisions and usage patterns must be documented
8. **Backward Compatibility**: Existing UserService functionality must remain intact

## Architecture and Design

### New Directory Structure
```
finman/
├── services/
│   ├── shared/
│   │   ├── Finman.Shared.Domain/           # Common domain models
│   │   ├── Finman.Shared.Infrastructure/   # Common infrastructure
│   │   ├── Finman.Shared.Testing/         # Test utilities
│   │   └── Finman.Shared.sln              # Shared libraries solution
│   ├── user-service/
│   │   ├── src/UserService/               # Moved from current src/UserService
│   │   ├── tests/                         # Moved from current tests/
│   │   ├── scripts/                       # Service-specific scripts
│   │   ├── UserService.sln               # Service solution
│   │   └── README.md                     # Service documentation
│   └── template-service/                  # Template for new services
├── frontend/
│   └── web-app/                          # Future Next.js SPA
├── infrastructure/
│   ├── docker-compose/                   # Local development orchestration
│   ├── scripts/                          # Infrastructure scripts
│   └── monitoring/                       # Future observability setup
├── scripts/                              # Root-level orchestration scripts
├── docs/                                 # Architecture and design docs
├── tests/                                # Integration tests across services
├── .devcontainer/                        # Dev container configuration
├── finman.code-workspace                 # Updated workspace configuration
└── README.md                             # Updated monorepo documentation
```

### Service Organization Principles
- Each service maintains its own solution file and can be built independently
- Shared libraries are referenced as project references (not NuGet initially)
- Each service follows the established Hexagonal Architecture pattern
- Services communicate via HTTP APIs (no direct assembly references between services)

### Build System Design
- Root-level scripts orchestrate multiple services
- Service-level scripts handle individual service operations
- Shared libraries have their own build pipeline
- Docker composition for local multi-service development

## Implementation Steps

### Phase 1: Repository Restructure ✅ COMPLETED
- [x] Create new directory structure
- [x] Move existing UserService to `services/user-service/`
- [x] Update all file paths and references
- [x] Create shared libraries structure
- [x] Update solution files and project references
- [x] Verify existing functionality still works

### Phase 2: Shared Libraries Foundation ✅ COMPLETED
- [x] Create `Finman.Shared.Domain` project for common domain concepts
- [x] Create `Finman.Shared.Infrastructure` for common infrastructure concerns
- [x] Create `Finman.Shared.Testing` for test utilities and helpers
- [x] Keep shared libraries empty until needed (per user request)
- [x] Create shared solution file structure

### Phase 3: Script System Overhaul
- [ ] Create root-level orchestration scripts
- [ ] Update service-level scripts to work within new structure
- [ ] Implement multi-service build/test/run capabilities
- [ ] Create template service with standard script set
- [ ] Update all script documentation

### Phase 4: Infrastructure and Orchestration
- [ ] Create docker-compose setup for local multi-service development
- [ ] Set up service discovery/communication patterns
- [ ] Create integration test framework
- [ ] Implement health check aggregation
- [ ] Set up local development database orchestration

### Phase 5: Documentation and Templates
- [ ] Update all documentation for monorepo structure
- [ ] Create service creation template and guide
- [ ] Update AI-AGENT.md with new structure guidelines
- [ ] Create contribution guidelines for monorepo
- [ ] Document service communication patterns

### Phase 6: Frontend Integration Preparation
- [ ] Define API gateway patterns (if needed)
- [ ] Set up CORS policies for multi-service frontend
- [ ] Create frontend integration documentation
- [ ] Prepare Next.js project structure (skeleton only)

## Success Criteria and Tests

### Functional Tests
- [ ] All existing UserService tests pass without modification
- [ ] UserService can be built, tested, and run independently
- [ ] Shared libraries can be built and consumed by UserService
- [ ] Root-level scripts can orchestrate multiple services (when more exist)
- [ ] Docker composition successfully runs UserService with dependencies

### Integration Tests
- [ ] Cross-service communication patterns work correctly
- [ ] Health checks aggregate properly across services
- [ ] Database orchestration works for local development
- [ ] Build system handles incremental builds efficiently

### Documentation Tests
- [ ] New developer can follow documentation to run the system
- [ ] Service creation template produces working service
- [ ] All architectural decisions are documented and current

### Performance Tests
- [ ] Build times remain reasonable (baseline current times)
- [ ] Development workflow is not significantly impacted
- [ ] Repository clone/fetch times are acceptable

## Working Area Scratchpad

### Current Analysis Notes
- UserService is currently in `src/UserService/` with tests in `tests/`
- Solution file is `FinmanUserService.sln` at root
- Scripts are in `scripts/` at root level
- DevContainer configuration exists and should be preserved

### Migration Considerations
- Need to update all absolute paths in project files
- Docker references will need updating
- CI/CD configurations (when they exist) will need updating
- VS Code workspace file needs updating

### Future Service Considerations
- InvestmentService: Handle investment data and calculations
- PortfolioService: Manage user portfolios and aggregations
- ReportingService: Generate reports and analytics
- NotificationService: Handle user notifications
- AuthService: Centralized authentication (if we move away from per-service auth)

### Risk Mitigation
- Take incremental approach to minimize disruption
- Maintain backward compatibility during transition
- Test thoroughly at each phase
- Keep rollback plan available

### Progress Summary
We've successfully completed the basic monorepo restructure:
- UserService moved to `services/user-service/` and fully functional
- Shared libraries structure created (empty, ready for future use)
- All tests pass in new structure
- Service can run independently

### Next Steps (for future implementation)
The remaining phases (script orchestration, infrastructure, documentation) should be tackled incrementally as needed, not all at once.
