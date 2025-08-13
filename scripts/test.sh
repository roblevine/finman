#!/bin/bash
set -e

echo "🧪 Running all tests for Finman Monorepo..."

# Navigate to repo root directory
cd "$(dirname "$0")/.."

# Test shared libraries first
echo "� Testing shared libraries..."
if [ -f "services/shared/Finman.Shared.sln" ]; then
    echo "🔬 Running shared library tests..."
    dotnet test services/shared/Finman.Shared.sln --verbosity normal
else
    echo "ℹ️  No shared library tests to run yet"
fi

# Test all services
echo "🏗️  Testing services..."

# Test UserService
echo "📦 Testing UserService..."
cd services/user-service
./scripts/test.sh
cd ../..

echo "✅ All monorepo tests completed successfully!"
echo "🎉 All tests passed!"