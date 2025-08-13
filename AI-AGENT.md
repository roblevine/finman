# AI-AGENT.md

This file provides guidance to AI Agents (such as Claude Code, GitHub CoPilot, Curtsor, and Windsurf) when working with code in this repository.

## Important Files
- **ARCHITECTURE.md**: Outlines the monorepo microservices architecture, service structure, and design patterns.
- **README.md**: Contains the introduction and overview of the Finman monorepo, including how to build, run, and test services.
- **TODO.md**: Lists tasks and features to be implemented across all services.
- **plans/**: Implementation plans and architectural decision records for major features and changes.

This file (AI-AGENT.md) contains ways of working instructions for AI Agents, including how to interpret and generate code, as well as guidelines for contributing to the Finman monorepo.

## Monorepo Structure Guidelines
- This is a **monorepo** containing multiple microservices in `services/`
- Each service in `services/` is independent and has its own solution file, tests, and scripts
- Shared libraries are in `services/shared/` and consumed as project references
- For service-specific work, navigate to the service directory (e.g., `services/user-service/`)
- Use service-specific scripts for individual service development
- Root-level scripts (when implemented) orchestrate multiple services

## Current Services
- **UserService** (`services/user-service/`): User authentication and management
- **Shared Libraries** (`services/shared/`): Common domain, infrastructure, and testing utilities (currently empty, populated as needed)

## Guidelines for AI Agents
- Always start every interactions with a the message "Hi Rob!" - this allows me to check you are working in accordance with the guidelines.
- Always refer to the **ARCHITECTURE.md** for understanding the overall structure and design of the Finman application.
- Use the **README.md** for context on how to set up and run the application.
- Follow the tasks outlined in **TODO.md** for implementing new features or fixing bugs.
- For building, running, testing, etc - always use the scripts provided in the service's "scripts" folder. Each service has its own scripts for independent development.
- For monorepo-wide operations, use root-level scripts (when implemented).

## Development Workflow
- We are a coding pair. We approach all problems with a Analyse -> Plan -> Execute -> Review methodology.
- We deliver individual features in small, incremental slices. For instance, if adding a new service - start with basic structure, then add core functionality incrementally.
- **Service Independence**: When working on a service, navigate to its directory and use its scripts. Services should remain independently developable.
- **Shared Libraries**: Only add code to `services/shared/` when genuinely needed by multiple services. Keep libraries minimal and focused.
- When generating code, ensure you update the relevant documentation files and maintain consistency with the existing codebase.
- If you encounter any issues or have questions, or there are multiple choices ahead - always ask for clarification.
- When making changes, ensure you document them clearly in the commit messages and comments within the code.
- We are *strictly* test-first, and we always write fully automated tests for any new features or changes to ensure the stability of the application.
- Always discuss before bringing in new dependencies/packages, as I might have preferences or not want to use particular choices.
- Prefer clean object-oriented design and follow SOLID principles.
- Use meaningful variable and function names to enhance code readability.