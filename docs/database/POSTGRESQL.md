# PostgreSQL Database Guide

This document provides comprehensive guidance for PostgreSQL setup, development, and current implementation status in the Finman project.

## Overview

The Finman project uses PostgreSQL 16 with specialized configurations for:
- **Case-insensitive uniqueness** using `citext` extension
- **Value object mappings** via Entity Framework Core
- **Development/Production environment awareness**
- **Docker-based development workflow**

## Quick Start

### 1. Start PostgreSQL

```bash
# From repository root
docker-compose up postgres -d

# Verify running
docker-compose ps postgres
```

### 2. Connect to Database

```bash
# From devcontainer
timeout 5 bash -c 'cat < /dev/null > /dev/tcp/postgres/5432' && echo "PostgreSQL ready"

# From host (if postgresql-client installed)
psql -h localhost -p 5432 -U finman -d finman
```

### 3. Run Application

```bash
# Application will connect automatically using environment-aware configuration
cd services/user-service
./scripts/run.sh --local
```

## Database Configuration

### Container Setup

PostgreSQL runs in a Docker container with the following configuration:

```yaml
# docker-compose.yml
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
```

### Network Configuration

- **Network**: `finman-network` (shared between PostgreSQL and devcontainer)
- **Host Access**: `localhost:5432`
- **Container Access**: `postgres:5432` (service name resolution)

### Database Schema

```sql
-- Extensions
CREATE EXTENSION IF NOT EXISTS citext;

-- Users table with value object mappings
CREATE TABLE users (
    id UUID PRIMARY KEY,
    email CITEXT UNIQUE NOT NULL,              -- Case-insensitive email
    username CITEXT UNIQUE NOT NULL,           -- Case-insensitive username
    first_name TEXT NOT NULL,                  -- PersonName.FirstName
    last_name TEXT NOT NULL,                   -- PersonName.LastName
    password_hash TEXT NOT NULL,               -- BCrypt hash
    created_at TIMESTAMPTZ NOT NULL,
    updated_at TIMESTAMPTZ NOT NULL,
    is_active BOOLEAN NOT NULL DEFAULT true,
    is_deleted BOOLEAN NOT NULL DEFAULT false, -- Soft delete support
    deleted_at TIMESTAMPTZ NULL
);

-- Indexes (automatically created by EF Core)
CREATE UNIQUE INDEX IX_users_email ON users (email);
CREATE UNIQUE INDEX IX_users_username ON users (username);
```

## Connection Details

### Environment-Specific Configuration

**Development/Production (PostgreSQL)**:
```csharp
// Connection string format
Host={host};Database=finman;Username=finman;Password=finman_dev_password

// From devcontainer
Host=postgres;Database=finman;Username=finman;Password=finman_dev_password

// From host machine  
Host=localhost;Database=finman;Username=finman;Password=finman_dev_password
```

**Test Environment (In-Memory)**:
- Tests use `InMemoryUserRepository` for fast, isolated testing
- No database connection required for unit tests

### Security Considerations

⚠️ **Development Credentials**: The credentials (`finman`/`finman_dev_password`) are for development only. Production deployments must use secure, environment-specific credentials.

## Entity Framework Core Integration

### DbContext Configuration

```csharp
// FinmanDbContext with sophisticated value object mappings
public class FinmanDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Email mapped to citext with implicit conversions
        entity.OwnsOne(u => u.Email, email => {
            email.Property(e => e.Value)
                 .HasColumnName("email")
                 .HasColumnType("citext")
                 .IsRequired();
            email.HasIndex(e => e.Value).IsUnique();
        });
        
        // PersonName mapped to separate columns
        entity.OwnsOne(u => u.Name, name => {
            name.Property(n => n.FirstName).HasColumnName("first_name");
            name.Property(n => n.LastName).HasColumnName("last_name");
        });
    }
}
```

### Migration Management

```bash
# From service directory (services/user-service/src/UserService)
dotnet ef migrations add MigrationName
dotnet ef database update

# View migration status
dotnet ef migrations list
```

### Repository Implementation

The `EfUserRepository` provides:
- **Async operations** for all database interactions
- **PostgreSQL-specific error handling** (unique constraint violations)
- **Value object support** with seamless domain/database conversion
- **Case-insensitive queries** leveraging citext extension

```csharp
// Exception mapping for PostgreSQL unique constraints
catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex, "email"))
{
    throw new UserAlreadyExistsException($"User with email '{user.Email}' already exists.");
}
```

## Development Workflow

### Typical Development Session

1. **Start Database**:
   ```bash
   docker-compose up postgres -d
   ```

2. **Verify Connectivity**:
   ```bash
   # From devcontainer
   timeout 5 bash -c 'cat < /dev/null > /dev/tcp/postgres/5432' && echo "Connected"
   ```

3. **Run Application**:
   ```bash
   cd services/user-service
   ./scripts/run.sh --local
   ```

4. **Development and Testing**:
   - Application uses PostgreSQL automatically
   - Tests use in-memory repository for speed
   - Integration tests can use PostgreSQL container

5. **Cleanup**:
   ```bash
   docker-compose stop postgres
   ```

## Management Commands

### Container Management

```bash
# Start PostgreSQL
docker-compose up postgres -d

# Stop PostgreSQL (preserves data)
docker-compose stop postgres

# Remove PostgreSQL container and data
docker-compose down postgres -v

# View logs
docker-compose logs postgres -f

# Check container health
docker-compose ps postgres
```

### Database Access

```bash
# Connect to PostgreSQL container
docker-compose exec postgres psql -U finman -d finman

# Connect from host (requires postgresql-client)
psql -h localhost -p 5432 -U finman -d finman

# Execute SQL file
docker-compose exec -T postgres psql -U finman -d finman < script.sql
```

### Backup and Restore

```bash
# Create backup
docker-compose exec postgres pg_dump -U finman -d finman > backup.sql

# Restore from backup
docker-compose exec -T postgres psql -U finman -d finman < backup.sql
```

## Implementation Status

### Completed Phases (1-3)

#### ✅ Phase 1: Dependencies and Local PostgreSQL Setup
- Docker Compose configuration with PostgreSQL 16-alpine
- Network setup for devcontainer integration
- Initial database schema with citext extension
- Health checks and container management

#### ✅ Phase 2: Enhanced Application Ports
- Comprehensive `IUserRepository` interface with async operations
- Error handling contracts for domain exceptions
- Compatibility layer for existing in-memory implementation

#### ✅ Phase 3: Infrastructure Persistence Layer
- **FinmanDbContext**: Sophisticated EF Core DbContext with value object mappings
- **EF Core Migration**: Initial schema (20250815163136_InitialUsers) with PostgreSQL optimizations
- **EfUserRepository**: Complete async implementation with PostgreSQL-specific error handling
- **Environment-Aware DI**: Production uses PostgreSQL, tests use in-memory repository
- **Value Object Mappings**: Email/Username as citext, PersonName as separate columns

### Current Status

- **Test Results**: 114/115 tests passing (1 unrelated Swagger failure)
  - Domain Tests: ✅ 48/48
  - Application Tests: ✅ 10/10
  - Infrastructure Tests: ✅ 56/57
- **Backward Compatibility**: All existing functionality preserved
- **Architecture Compliance**: Domain layer remains framework-free

### Remaining Work (Phases 4-8)

#### Phase 4: Connection Strings and Auto-Migration
- Environment-specific connection string configuration
- Automatic migration on application startup
- Production-ready database configuration

#### Phase 5: Repository Optimizations
- Advanced query optimizations
- Bulk operations support
- Performance monitoring and logging

#### Phase 6: Testcontainers Integration
- Integration tests with real PostgreSQL using Testcontainers
- Isolated test database instances
- Enhanced test reliability

#### Phase 7: End-to-End Use Case Wiring
- Complete use case persistence integration
- Transaction management
- Error handling validation

#### Phase 8: Development Experience and Documentation
- Enhanced developer tooling
- Performance monitoring dashboards
- Production deployment guides

## Troubleshooting

### Common Issues

#### Container Won't Start

1. **Port conflict** (5432 already in use):
   ```bash
   sudo netstat -tlnp | grep 5432
   # Stop conflicting service or change port mapping
   ```

2. **Docker daemon not running**:
   ```bash
   sudo systemctl start docker
   ```

#### Connection Issues

1. **From devcontainer**:
   ```bash
   # Test network connectivity
   timeout 5 bash -c 'cat < /dev/null > /dev/tcp/postgres/5432'
   
   # Check network membership
   docker network inspect finman-network
   ```

2. **From host**:
   ```bash
   # Verify port mapping
   docker port $(docker-compose ps -q postgres)
   
   # Test connection
   telnet localhost 5432
   ```

#### Database Issues

1. **Permission errors**:
   ```bash
   # Reset container with fresh data
   docker-compose down postgres -v
   docker-compose up postgres -d
   ```

2. **Migration failures**:
   ```bash
   # Check migration status
   cd services/user-service/src/UserService
   dotnet ef migrations list
   
   # Reset database to specific migration
   dotnet ef database update PreviousMigrationName
   ```

### Database Monitoring

```bash
# Monitor PostgreSQL logs
docker-compose logs postgres -f

# Check database size
docker-compose exec postgres psql -U finman -d finman -c "
    SELECT pg_size_pretty(pg_database_size('finman')) as database_size;
"

# Monitor connections
docker-compose exec postgres psql -U finman -d finman -c "
    SELECT count(*) as active_connections 
    FROM pg_stat_activity 
    WHERE state = 'active';
"
```

## Best Practices

### Development

1. **Always use the Docker container** for consistent environment
2. **Test both environment configurations** (PostgreSQL and in-memory)
3. **Run migrations in development** before committing schema changes
4. **Use transactions** for complex operations
5. **Leverage citext** for case-insensitive text operations

### Production Considerations

1. **Use environment variables** for sensitive configuration
2. **Implement connection pooling** for high-load scenarios
3. **Monitor database performance** and connection counts
4. **Plan backup and disaster recovery** strategies
5. **Use secure credentials** and network configurations

### Testing Strategy

1. **Unit tests**: Use in-memory repository for fast feedback
2. **Integration tests**: Use PostgreSQL container for realistic scenarios
3. **Database tests**: Verify migrations and schema changes
4. **Performance tests**: Monitor query performance and optimization
