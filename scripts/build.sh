#!/bin/bash
set -e

echo "🏗️  Building Finman User Service..."

# Navigate to repo root directory
cd "$(dirname "$0")/.."

echo "📦 Restoring .NET packages..."
dotnet restore

echo "🔧 Building the solution..."
dotnet build --no-restore --configuration Release

echo "🧪 Running tests..."
dotnet test --no-build --configuration Release --verbosity normal

echo "✅ Build completed successfully!"
echo ""

# Check if Docker is available and build image if possible
if command -v docker &> /dev/null && docker info &> /dev/null 2>&1; then
    echo "🐳 Building Docker image..."
    docker build -t finman-userservice:latest src/UserService/
    echo "Docker image 'finman-userservice:latest' is ready to use."
    echo "Run './scripts/run.sh' to start the containerized service."
else
    echo "ℹ️  Note: Docker image building is skipped - Docker not available or not configured."
    echo "   For dev containers: Rebuild container with Docker-outside-of-Docker feature enabled."
    echo "   For local development: Install Docker Desktop or configure Docker daemon access."
    echo "Run './scripts/run.sh --local' to start the service locally."
fi