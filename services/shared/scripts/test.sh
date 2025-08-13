#!/bin/bash
set -e

echo "🧪 Testing Finman Shared Libraries..."

# Navigate to shared directory
cd "$(dirname "$0")/.."

echo "📋 Building and running tests..."
dotnet test --verbosity normal --logger "console;verbosity=detailed"

echo "✅ All shared library tests completed successfully!"
