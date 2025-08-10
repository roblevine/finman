**IMPORTANT - PLEASE READ the AI-AGENT.md file for more information on the rules and guidance on contributing to this project.

# Introduction

Welcome to the Finman project.

In time, this application will be a fully functional investment tracking tool.
Initially, we are building out the user module, and will then move on to a simple investment tracking module.

## Local development with Postgres (optional, upcoming phases)

- To start a local Postgres for development:
	- `./scripts/setup.sh --start-postgres` to boot a postgres:16-alpine container and export `POSTGRES_CONNECTION` for your shell.
	- Or `./scripts/run.sh --docker` to run the service and Postgres via docker-compose.

- A guarded flag `MIGRATE_AT_STARTUP` is supported in Development. It is a no-op today and will apply EF Core migrations in a later phase.