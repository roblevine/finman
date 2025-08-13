#!/bin/bash
set -e

echo "🧹 Cleaning Finman Monorepo build artifacts..."

# Navigate to repo root directory
cd "$(dirname "$0")/.."

echo "🗑️  Cleaning shared libraries..."
if [ -f "services/shared/Finman.Shared.sln" ]; then
    dotnet clean services/shared/Finman.Shared.sln
else
    echo "ℹ️  No shared libraries to clean"
fi

echo "🗑️  Cleaning services..."
echo "📦 Cleaning UserService..."
cd services/user-service
dotnet clean
cd ../..

echo "🗑️  Removing bin and obj directories..."
find . -name "bin" -type d -exec rm -rf {} + 2>/dev/null || true
find . -name "obj" -type d -exec rm -rf {} + 2>/dev/null || true

echo "🗑️  Removing test results..."
rm -rf TestResults/ 2>/dev/null || true
rm -rf services/user-service/TestResults/ 2>/dev/null || true
rm -rf services/shared/TestResults/ 2>/dev/null || true

echo "🐳 Cleaning Docker artifacts..."
# Clean up specific project containers and networks
docker system prune -f --filter "label=com.docker.compose.project=userservice" 2>/dev/null || true
docker system prune -f --filter "label=com.docker.compose.project=finman" 2>/dev/null || true

# Remove dangling images
docker image prune -f 2>/dev/null || true

echo "✅ Monorepo cleanup completed successfully!"