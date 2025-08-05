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
echo "ℹ️  Note: Docker image building is skipped in dev container environment."
echo "   To build Docker images, run this script on the host or configure DooD."
echo "Run './scripts/run.sh --local' to start the service locally."