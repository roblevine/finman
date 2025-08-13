#!/bin/bash
set -e

echo "🧹 Cleaning Finman Shared Libraries..."

# Navigate to shared directory
cd "$(dirname "$0")/.."

echo "🗑️  Cleaning build artifacts..."
dotnet clean

echo "✅ Shared libraries cleaned successfully!"
