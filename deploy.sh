#!/bin/bash

# Deployment script for DigitalOcean Droplet
# This script should be run on your DigitalOcean server

set -e

echo "🚀 Starting deployment..."

# Pull latest code
echo "📥 Pulling latest code from git..."
git pull origin master

# Copy environment file if it doesn't exist
if [ ! -f .env ]; then
    echo "⚙️ Creating .env file from .env.example..."
    cp .env.example .env
    echo "⚠️ IMPORTANT: Edit .env file with your secure passwords!"
    exit 1
fi

# Stop existing containers
echo "🛑 Stopping existing containers..."
docker-compose down

# Build and start containers
echo "🔨 Building and starting containers..."
docker-compose up -d --build

# Wait for database to be ready
echo "⏳ Waiting for database to be ready..."
sleep 10

# Run database migrations
echo "📊 Running database migrations..."
docker-compose exec -T backend dotnet ef database update

echo "✅ Deployment complete!"
echo "🌐 Your application should be running at http://your-droplet-ip"
echo ""
echo "Useful commands:"
echo "  - View logs: docker-compose logs -f"
echo "  - Restart: docker-compose restart"
echo "  - Stop: docker-compose down"
