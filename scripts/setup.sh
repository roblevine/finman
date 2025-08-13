#!/bin/bash
set -e

echo "🚀 Setting up Finman Monorepo..."

# Navigate to repo root directory
cd "$(dirname "$0")/.."

# Check if .NET 8 SDK is installed
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET 8 SDK is not installed. Please install it first."
    exit 1
fi

echo "📦 Restoring shared libraries..."
if [ -f "services/shared/Finman.Shared.sln" ]; then
    dotnet restore services/shared/Finman.Shared.sln
else
    echo "ℹ️  No shared libraries to restore yet"
fi

echo "📦 Restoring services..."
echo "🔧 Restoring UserService..."
cd services/user-service
dotnet restore
cd ../..

echo "🔧 Building the monorepo..."
./scripts/build.sh

# Optional: start Postgres via Docker Compose for local development
if [ "$1" = "--start-postgres" ]; then
    if ! command -v docker &> /dev/null; then
        echo "❌ Docker CLI not found; cannot start Postgres. Skipping."
    else
        if ! docker info &> /dev/null; then
            echo "❌ Docker is not running. Please start Docker and re-run with --start-postgres."
            exit 1
        fi
        echo "🐘 Starting PostgreSQL via docker-compose..."
        pushd services/user-service/src/UserService > /dev/null
        POSTGRES_USER=${POSTGRES_USER:-finman}
        POSTGRES_PASSWORD=${POSTGRES_PASSWORD:-finman}
        POSTGRES_DB=${POSTGRES_DB:-finman}
        docker-compose up -d postgres
        popd > /dev/null
        export POSTGRES_CONNECTION=${POSTGRES_CONNECTION:-"Host=localhost;Port=5432;Database=${POSTGRES_DB};Username=${POSTGRES_USER};Password=${POSTGRES_PASSWORD}"}
        echo "🧩 POSTGRES_CONNECTION set for this shell: $POSTGRES_CONNECTION"
    fi
fi

echo "✅ Monorepo setup completed successfully!"
echo ""
echo "Next steps:"
echo "  • Run './scripts/test.sh' to execute all tests"
echo "  • Run './scripts/run.sh --local' to start services locally (uses in-memory by default)"
echo "  • Run './scripts/run.sh --docker' to start with docker-compose (includes Postgres)"
echo "  • Re-run './scripts/setup.sh --start-postgres' to boot a local Postgres and set POSTGRES_CONNECTION"