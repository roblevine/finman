#!/bin/bash
set -e

echo "🧪 Running all tests for Finman User Service..."

# Navigate to repo root directory
cd "$(dirname "$0")/.."

echo "📋 Running unit tests..."
dotnet test --no-build --verbosity normal --logger "console;verbosity=detailed"

echo "📊 Generating test coverage report..."
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults

echo "✅ All tests completed successfully!"

# Check if all tests passed
if [ $? -eq 0 ]; then
    echo "🎉 All tests passed!"
else
    echo "❌ Some tests failed. Please check the output above."
    exit 1
fi