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
    
    # Check if Dockerfile exists
    if [ -f "src/UserService/Dockerfile" ]; then
        # Build from repo root with UserService Dockerfile and load to local Docker
        echo "📁 Building Docker image from repository root..."
        docker build -f src/UserService/Dockerfile -t finman-userservice:latest --load .
        echo "✅ Docker image 'finman-userservice:latest' built successfully!"
        echo "🚀 Run './scripts/run.sh --docker' to start the containerized service."
    else
        echo "⚠️  Dockerfile not found in src/UserService/"
        echo "🏠 Run './scripts/run.sh --local' to start the service locally."
    fi
else
    echo "🚫 Docker not available in this environment."
    echo "🏠 Run './scripts/run.sh --local' to start the service locally."
fi