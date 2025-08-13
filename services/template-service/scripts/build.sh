#!/bin/bash
set -e

echo "🏗️  Building Template Service..."

# Navigate to service root directory
cd "$(dirname "$0")/.."

echo "📦 Restoring .NET packages..."
dotnet restore

echo "🔧 Building the solution..."
dotnet build --no-restore --configuration Release

echo "🧪 Running tests..."
dotnet test --no-build --configuration Release --verbosity normal

echo "✅ Build completed successfully!"
echo ""

# Check if Docker is available and build image if possible
if command -v docker &> /dev/null && docker info &> /dev/null 2>&1; then
    echo "🐳 Building Docker image..."
    
    # Check if Dockerfile exists
    if [ -f "src/TemplateService/Dockerfile" ]; then
        # Build Docker image from service root
        echo "📁 Building Docker image from service root..."
        docker build -f src/TemplateService/Dockerfile -t finman-templateservice:latest --load .
        echo "✅ Docker image 'finman-templateservice:latest' built successfully!"
        echo "🚀 Run './scripts/run.sh --docker' to start the containerized service."
    else
        echo "⚠️  Dockerfile not found in src/TemplateService/"
        echo "🏠 Run './scripts/run.sh --local' to start the service locally."
    fi
else
    echo "🚫 Docker not available in this environment."
    echo "🏠 Run './scripts/run.sh --local' to start the service locally."
fi
