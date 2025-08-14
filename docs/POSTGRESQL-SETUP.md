# PostgreSQL Container Setup

This document explains how to spin up and access the PostgreSQL container for development and testing.

## Overview

The PostgreSQL setup consists of:
- A PostgreSQL 16-alpine container accessible to both host and devcontainer
- Docker network configuration for container-to-container communication
- Devcontainer network integration for seamless development

## Quick Start

### 1. Start PostgreSQL Container

From the repository root:

```bash
docker-compose up postgres -d
```

This starts PostgreSQL in detached mode on the `finman-network`.

### 2. Verify Container is Running

```bash
docker ps | grep postgres
```

You should see something like:
```
97f250d9bc3f   postgres:16-alpine   "postgres"   Up X seconds   5432/tcp   finman-postgres-1
```

### 3. Check Container Health

```bash
docker-compose ps postgres
```

The status should show `healthy` when the container is ready.

## Network Access

### From Host Machine

PostgreSQL is accessible from the host at:
- **Host**: `localhost`
- **Port**: `5432`
- **Database**: `finman`
- **Username**: `finman`
- **Password**: `finman_dev_password`

Example connection:
```bash
psql -h localhost -p 5432 -U finman -d finman
```

### From Devcontainer

The devcontainer is configured to connect to the `finman-network`, allowing access via service name:
- **Host**: `postgres` (service name)
- **Port**: `5432`
- **Database**: `finman`
- **Username**: `finman`
- **Password**: `finman_dev_password`

Example from devcontainer:
```bash
# Test connectivity
timeout 5 bash -c 'cat < /dev/null > /dev/tcp/postgres/5432' && echo "Connected" || echo "Failed"

# If psql client was installed:
# psql -h postgres -p 5432 -U finman -d finman
```

### Connection String for .NET Applications

When running in the devcontainer:
```
Host=postgres;Database=finman;Username=finman;Password=finman_dev_password
```

When running on host:
```
Host=localhost;Database=finman;Username=finman;Password=finman_dev_password
```

## Container Configuration

### Docker Compose Configuration

The PostgreSQL service is defined in `docker-compose.yml`:

```yaml
services:
  postgres:
    image: postgres:16-alpine
    environment:
      POSTGRES_DB: finman
      POSTGRES_USER: finman
      POSTGRES_PASSWORD: finman_dev_password
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data
      - ./infrastructure/postgres/init.sql:/docker-entrypoint-initdb.d/init.sql:ro
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U finman -d finman"]
      interval: 5s
      timeout: 5s
      retries: 5

networks:
  default:
    name: finman-network
```

### Devcontainer Configuration

The devcontainer connects to the same network via `runArgs`:

```json
{
  "runArgs": ["--name", "finman_devcontainer", "--network", "finman-network"]
}
```

## Database Schema

PostgreSQL is configured with:
- **Extensions**: `citext` for case-insensitive text fields
- **Initial Schema**: Defined in `infrastructure/postgres/init.sql`

## Management Commands

### Start PostgreSQL
```bash
docker-compose up postgres -d
```

### Stop PostgreSQL
```bash
docker-compose stop postgres
```

### Remove PostgreSQL (with data)
```bash
docker-compose down postgres -v
```

### View Logs
```bash
docker-compose logs postgres -f
```

### Connect to PostgreSQL Container
```bash
docker-compose exec postgres psql -U finman -d finman
```

### Access PostgreSQL Shell from Host
```bash
# Install postgresql-client if not available
sudo apt-get update && sudo apt-get install -y postgresql-client

# Connect
psql -h localhost -p 5432 -U finman -d finman
```

## Troubleshooting

### Container Won't Start
1. Check if port 5432 is already in use:
   ```bash
   sudo netstat -tlnp | grep 5432
   ```

2. Check Docker logs:
   ```bash
   docker-compose logs postgres
   ```

### Can't Connect from Devcontainer
1. Verify network connectivity:
   ```bash
   timeout 5 bash -c 'cat < /dev/null > /dev/tcp/postgres/5432' && echo "Connected" || echo "Failed"
   ```

2. Check if containers are on the same network:
   ```bash
   docker network inspect finman-network
   ```

### Can't Connect from Host
1. Verify port mapping:
   ```bash
   docker port finman-postgres-1
   ```

2. Check if PostgreSQL is listening:
   ```bash
   docker-compose exec postgres pg_isready -U finman -d finman
   ```

## Development Workflow

### Typical Development Session

1. **Start PostgreSQL**:
   ```bash
   docker-compose up postgres -d
   ```

2. **Verify connectivity** from devcontainer:
   ```bash
   timeout 5 bash -c 'cat < /dev/null > /dev/tcp/postgres/5432' && echo "PostgreSQL ready"
   ```

3. **Run your .NET application** with connection string pointing to `postgres:5432`

4. **Stop when done**:
   ```bash
   docker-compose stop postgres
   ```

### For Integration Testing

The same PostgreSQL container can be used for integration tests by configuring the test connection string to use `Host=postgres`.

## Security Notes

⚠️ **Development Only**: The credentials (`finman`/`finman_dev_password`) are for development only. Never use these in production.

## Next Steps

- For production deployment, use secure credentials and proper networking
- Consider using environment variables for connection strings
- Implement proper database migrations for schema management
