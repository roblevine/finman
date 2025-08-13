#!/bin/bash
set -e

echo "🚀 Starting Finman User Service..."

# Navigate to repo root directory
cd "$(dirname "$0")/.."

# Check if we should run in Docker or locally
if [ "$1" = "--docker" ]; then
    echo "🐳 Starting service using Docker Compose..."
    cd src/UserService && docker-compose up --build
elif [ "$1" = "--local" ]; then
    echo "🏠 Starting service locally..."
    echo "📡 Service will be available at: http://localhost:5001"
    echo "📚 Swagger UI will be available at: http://localhost:5001/swagger"
    echo ""
    cd src/UserService && dotnet run
else
    echo "Usage: $0 [--docker|--local]"
    echo ""
    echo "Options:"
    echo "  --docker    Run using Docker Compose (recommended)"
    echo "  --local     Run locally using dotnet run"
    echo ""
    echo "Examples:"
    echo "  $0 --docker"
    echo "  $0 --local"
    exit 1
fi