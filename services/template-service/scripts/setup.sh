#!/bin/bash
set -e

echo "🚀 Setting up Template Service..."

# Navigate to service root directory
cd "$(dirname "$0")/.."

# Check if .NET 8 SDK is installed
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET 8 SDK is not installed. Please install it first."
    exit 1
fi

echo "📦 Restoring .NET packages..."
dotnet restore

echo "🔧 Building the solution..."
dotnet build --no-restore

echo "✅ Setup completed successfully!"
echo ""
echo "Next steps:"
echo "  • Run './scripts/test.sh' to execute all tests"
echo "  • Run './scripts/run.sh --local' to start the dev server"
echo "  • Run './scripts/run.sh --docker' to start with docker-compose"
