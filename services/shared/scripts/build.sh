#!/bin/bash
set -e

echo "📦 Building Finman Shared Libraries..."

# Navigate to shared directory
cd "$(dirname "$0")/.."

echo "📦 Restoring .NET packages..."
dotnet restore

echo "🔧 Building the shared solution..."
dotnet build --no-restore --configuration Release

echo "✅ Shared libraries build completed successfully!"
