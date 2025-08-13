#!/bin/bash
set -e

# Check if service name was provided
if [ -z "$1" ]; then
    echo "Usage: $0 <service-name>"
    echo ""
    echo "Examples:"
    echo "  $0 investment-service"
    echo "  $0 portfolio-service"
    echo "  $0 reporting-service"
    exit 1
fi

SERVICE_NAME="$1"
SERVICE_NAME_PASCAL=$(echo "$SERVICE_NAME" | sed 's/-\([a-z]\)/\U\1/g' | sed 's/^\([a-z]\)/\U\1/')
SERVICE_NAME_PASCAL="${SERVICE_NAME_PASCAL}Service"

echo "🚀 Creating new service: $SERVICE_NAME"
echo "📦 Service class name: $SERVICE_NAME_PASCAL"

# Navigate to repo root directory
cd "$(dirname "$0")/.."

# Check if service already exists
if [ -d "services/$SERVICE_NAME" ]; then
    echo "❌ Service already exists: services/$SERVICE_NAME"
    exit 1
fi

# Copy template to new service directory
echo "📁 Creating service directory from template..."
cp -r services/template-service "services/$SERVICE_NAME"

# Navigate to the new service directory
cd "services/$SERVICE_NAME"

echo "🔄 Updating service files..."

# Replace template placeholders with actual service names
find . -type f -name "*.md" -exec sed -i "s/TemplateService/$SERVICE_NAME_PASCAL/g; s/template-service/$SERVICE_NAME/g" {} \;
find . -type f -name "*.sh" -exec sed -i "s/TemplateService/$SERVICE_NAME_PASCAL/g; s/template-service/$SERVICE_NAME/g; s/templateservice/$SERVICE_NAME/g" {} \;

# Update the README with specific service name
sed -i "1s/.*/# $SERVICE_NAME_PASCAL/" README.md
sed -i "s/This is a template for creating new services in the Finman monorepo./This service handles $SERVICE_NAME functionality in the Finman application./" README.md

echo "🔧 Service template created successfully!"
echo ""
echo "Next steps:"
echo "  1. cd services/$SERVICE_NAME"
echo "  2. Create the actual project structure (solution file, projects, etc.)"
echo "  3. Implement your domain models, use cases, and controllers"
echo "  4. Update the root-level scripts to include your new service"
echo "  5. Write tests following the established patterns"
echo ""
echo "Service directory: services/$SERVICE_NAME"
echo "Service class name: $SERVICE_NAME_PASCAL"
