#!/bin/bash
set -e

echo "🚀 Starting Finman Monorepo Services..."

# Navigate to repo root directory
cd "$(dirname "$0")/.."

# Parse service selection (default to all services)
SERVICE="all"
RUN_MODE="local"

while [[ $# -gt 0 ]]; do
    case $1 in
        --docker)
            RUN_MODE="docker"
            shift
            ;;
        --local)
            RUN_MODE="local"
            shift
            ;;
        --service)
            SERVICE="$2"
            shift 2
            ;;
        -h|--help)
            echo "Usage: $0 [--docker|--local] [--service SERVICE_NAME]"
            echo ""
            echo "Options:"
            echo "  --docker         Run using Docker"
            echo "  --local          Run locally using dotnet run (default)"
            echo "  --service NAME   Run specific service (user-service, or 'all' for all services)"
            echo ""
            echo "Examples:"
            echo "  $0 --local                        # Run all services locally"
            echo "  $0 --docker                       # Run all services with Docker"
            echo "  $0 --local --service user-service # Run only UserService locally"
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            echo "Use --help for usage information"
            exit 1
            ;;
    esac
done

run_user_service() {
    local mode=$1
    echo "📦 Starting UserService..."
    
    if [ "$mode" = "docker" ]; then
        echo "🐳 Running UserService with Docker..."
        cd services/user-service
        ./scripts/run.sh --docker
        cd ../..
    else
        echo "🏠 Running UserService locally..."
        echo "📡 UserService will be available at: http://localhost:5001"
        echo "📚 Swagger UI will be available at: http://localhost:5001/swagger"
        cd services/user-service
        ./scripts/run.sh --local
        cd ../..
    fi
}

# Run selected services
case $SERVICE in
    "all")
        echo "🏗️  Starting all services..."
        run_user_service "$RUN_MODE"
        ;;
    "user-service")
        run_user_service "$RUN_MODE"
        ;;
    *)
        echo "❌ Unknown service: $SERVICE"
        echo "Available services: user-service"
        exit 1
        ;;
esac