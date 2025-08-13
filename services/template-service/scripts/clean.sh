#!/bin/bash
set -e

echo "🧹 Cleaning Template Service build artifacts..."

# Navigate to service root directory
cd "$(dirname "$0")/.."

echo "🗑️  Removing build artifacts..."
dotnet clean

echo "🗑️  Removing bin and obj directories..."
find . -name "bin" -type d -exec rm -rf {} + 2>/dev/null || true
find . -name "obj" -type d -exec rm -rf {} + 2>/dev/null || true

echo "🗑️  Removing test results..."
rm -rf TestResults/ 2>/dev/null || true

echo "🐳 Cleaning Docker artifacts..."
docker system prune -f --filter "label=com.docker.compose.project=templateservice" 2>/dev/null || true

# Remove dangling images
docker image prune -f 2>/dev/null || true

echo "✅ Cleanup completed successfully!"
