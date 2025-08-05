#!/bin/bash
set -e

echo "🚀 Setting up Finman User Service..."

# Check if .NET 8 SDK is installed
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET 8 SDK is not installed. Please install it first."
    exit 1
fi

# Check if Docker is running
if ! docker info &> /dev/null; then
    echo "❌ Docker is not running. Please start Docker first."
    exit 1
fi

# Navigate to repo root directory
cd "$(dirname "$0")/.."

echo "📦 Restoring .NET packages..."
dotnet restore

echo "🔧 Building the solution..."
dotnet build --no-restore

echo "✅ Setup completed successfully!"
echo ""
echo "Next steps:"
echo "  • Run './scripts/test.sh' to execute all tests"
echo "  • Run './scripts/run.sh' to start the development server"
echo "  • Run './scripts/build.sh' to build Docker containers"