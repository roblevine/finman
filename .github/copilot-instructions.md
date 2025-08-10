# Finman – AI coding assistant instructions

Read AI-AGENT.md first. Begin replies with "Hi Rob!". Use `scripts/` for build/run/test. Work in small, test-first increments; discuss before adding dependencies.

## Architecture and layout
- Hexagonal (Ports & Adapters). Current service: `src/UserService`.
	- Domain: `Domain/{Entities,ValueObjects,Exceptions}`
	- Application: `Application/{Ports,UseCases,DTOs}`
	- Infrastructure: `Infrastructure/Web/Controllers`
	- Composition root: `Program.cs` (partial for tests)
- See `HelloController.cs` for controller style; `Program.cs` registers controllers, Swagger (Dev), and health checks.

## Endpoints (UserService)
- GET `/api/hello` → 200 JSON `{ message, timestamp, version }`
- GET `/api/hello/{name}` → 200 personalized; 400 for empty/whitespace
- GET `/health` → 200 "Healthy"
- Swagger (Dev): `/swagger`, `/swagger/v1/swagger.json`

## Build, test, run
- Setup: `scripts/setup.sh`
- Build + tests: `scripts/build.sh` (Release; builds Docker image `finman-userservice:latest` if Docker is available)
- Tests + coverage: `scripts/test.sh` (Cobertura in `TestResults/**/coverage.cobertura.xml`)
- Run: `scripts/run.sh --local` (http://localhost:5001) or `--docker` (http://localhost:8080)
- Clean: `scripts/clean.sh`

## Conventions
- Tests: xUnit; do not use FluentAssertions (see ARCHITECTURE.md). Mirror target layer in `tests/*`.
- Controllers: `[ApiController]`, `[Route("api/...“)]`, `[Produces("application/json")]`; return `ActionResult<T>` with `[ProducesResponseType]`. Example returns `HelloResponse` record (init-only props).
- Health checks: `AddHealthChecks().AddCheck("self", ...)` + `app.MapHealthChecks("/health")`.
- Swagger enabled only in Development in `Program.cs`.
- Test host: `TestWebApplicationFactory` forces `Development` and sets JSON `camelCase`.

## Extending safely
1) Write tests (`tests/*`), including minimal integration via `WebApplicationFactory`.
2) Define ports in `Application/Ports`; add use cases in `Application/UseCases`.
3) Update domain in `Domain/*` (framework-free logic).
4) Implement adapters in `Infrastructure/*`; wire in `Program.cs`.
5) Run `build.sh`, then `run.sh`; validate with `/health`, `/swagger`, endpoints, and tests.

Key references: `src/UserService/Infrastructure/Web/Controllers/HelloController.cs`, `src/UserService/Program.cs`, `tests/Infrastructure.Tests/*`.