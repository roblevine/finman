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

echo "🐳 Building Docker image..."
docker build -t finman-userservice:latest src/UserService/

echo "✅ Build completed successfully!"
echo ""
echo "Docker image 'finman-userservice:latest' is ready to use."
echo "Run './scripts/run.sh' to start the containerized service."