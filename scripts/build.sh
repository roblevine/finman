#!/bin/bash
set -e

echo "🔨 Building Finman Monorepo..."

# Navigate to repo root directory
cd "$(dirname "$0")/.."

# Build shared libraries first
echo "📦 Building shared libraries..."
if [ -f "services/shared/Finman.Shared.sln" ]; then
    echo "🔧 Building shared solution..."
    dotnet build services/shared/Finman.Shared.sln --configuration Release
else
    echo "ℹ️  No shared libraries to build yet"
fi

# Build all services
echo "🏗️  Building services..."

# Build UserService
echo "📦 Building UserService..."
cd services/user-service
dotnet build --configuration Release
cd ../..

echo "✅ Monorepo build completed successfully!"
echo ""

# Check if Docker is available and build images
if command -v docker &> /dev/null && docker info &> /dev/null 2>&1; then
    echo "🐳 Building Docker images..."
    
    # Build UserService image if Dockerfile exists
    if [ -f "services/user-service/src/UserService/Dockerfile" ]; then
        echo "📁 Building UserService Docker image..."
        cd services/user-service
        docker build -f src/UserService/Dockerfile -t finman-userservice:latest --load .
        cd ../..
        echo "✅ Docker image 'finman-userservice:latest' built successfully!"
    else
        echo "⚠️  UserService Dockerfile not found"
    fi
else
    echo "🚫 Docker not available in this environment."
fi