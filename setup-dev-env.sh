#!/bin/bash
# Development Environment Setup Script for Linux/macOS
# This script sets up environment variables for local development

echo "========================================="
echo "Microservice Development Environment Setup"
echo "========================================="
echo ""

# Function to add to profile
add_to_profile() {
    echo "export $1" >> ~/.bashrc
    echo "export $1" >> ~/.zshrc 2>/dev/null || true
}

# JWT Configuration
echo "Setting JWT Configuration..."
export Jwt__Secret="dev-super-secret-jwt-key-with-at-least-32-chars"
export Jwt__Issuer="MicroservicesIssuer"
export Jwt__Audience="MicroserviceClients"
export Jwt__ExpirationMinutes="60"

add_to_profile 'Jwt__Secret="dev-super-secret-jwt-key-with-at-least-32-chars"'
add_to_profile 'Jwt__Issuer="MicroservicesIssuer"'
add_to_profile 'Jwt__Audience="MicroserviceClients"'
add_to_profile 'Jwt__ExpirationMinutes="60"'

# AuthService
echo "Setting AuthService Configuration..."
export AuthService__ConnectionString="Server=localhost,1433;Database=AuthServiceDb;User Id=sa;Password=YourPassword123;"
add_to_profile 'AuthService__ConnectionString="Server=localhost,1433;Database=AuthServiceDb;User Id=sa;Password=YourPassword123;"'

# ProductService
echo "Setting ProductService Configuration..."
export ProductService__ConnectionString="Server=localhost,1433;Database=ProductServiceDb;User Id=sa;Password=YourPassword123;"
add_to_profile 'ProductService__ConnectionString="Server=localhost,1433;Database=ProductServiceDb;User Id=sa;Password=YourPassword123;"'

# LogService
echo "Setting LogService Configuration..."
export LogService__ConnectionString="Server=localhost,1433;Database=LogDb;User Id=sa;Password=YourPassword123;"
add_to_profile 'LogService__ConnectionString="Server=localhost,1433;Database=LogDb;User Id=sa;Password=YourPassword123;"'

# API Gateway
echo "Setting ApiGateway Configuration..."
export ApiGateway__AuthServiceUrl="http://localhost:5001"
export ApiGateway__ProductServiceUrl="http://localhost:5002"
export ApiGateway__LogServiceUrl="http://localhost:5003"

add_to_profile 'ApiGateway__AuthServiceUrl="http://localhost:5001"'
add_to_profile 'ApiGateway__ProductServiceUrl="http://localhost:5002"'
add_to_profile 'ApiGateway__LogServiceUrl="http://localhost:5003"'

# Logging
echo "Setting Logging Configuration..."
export ASPNETCORE_ENVIRONMENT="Development"
export Serilog__MinimumLevel="Information"

add_to_profile 'ASPNETCORE_ENVIRONMENT="Development"'
add_to_profile 'Serilog__MinimumLevel="Information"'

echo ""
echo "========================================="
echo "Environment variables have been set!"
echo "Run: source ~/.bashrc or source ~/.zshrc"
echo "========================================="
