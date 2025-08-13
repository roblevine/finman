#!/bin/bash
set -e

echo "🧪 Running all tests for Finman User Service..."

# Navigate to repo root directory
cd "$(dirname "$0")/.."

echo "📋 Building and running tests..."
dotnet test --verbosity normal --logger "console;verbosity=detailed"

echo "📊 Generating test coverage report..."
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults

echo "✅ All tests completed successfully!"
echo "🎉 All tests passed!"