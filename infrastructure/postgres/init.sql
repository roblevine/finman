-- Finman Database Initialization
-- This script will be run when the PostgreSQL container starts

-- Ensure the database exists (docker-compose already creates it)
\c finman;

-- Create basic schema structure for future use
-- Note: Actual table creation will be handled by each service's migrations

-- Grant permissions
GRANT ALL PRIVILEGES ON DATABASE finman TO finman;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO finman;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO finman;

-- Set default privileges for future tables
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON TABLES TO finman;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON SEQUENCES TO finman;

-- Ready for service-specific schemas and tables
SELECT 'Database initialized successfully' AS status;
