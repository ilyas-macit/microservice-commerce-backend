@echo off
REM Development Environment Setup Script for Windows
REM This script sets up environment variables for local development

echo =========================================
echo Microservice Development Environment Setup
echo =========================================
echo.

REM JWT Configuration
echo Setting JWT Configuration...
setx Jwt__Secret "dev-super-secret-jwt-key-with-at-least-32-chars"
setx Jwt__Issuer "MicroservicesIssuer"
setx Jwt__Audience "MicroserviceClients"
setx Jwt__ExpirationMinutes "60"

REM AuthService
echo Setting AuthService Configuration...
setx AuthService__ConnectionString "Server=localhost,1433;Database=AuthServiceDb;User Id=sa;Password=YourPassword123;"

REM ProductService
echo Setting ProductService Configuration...
setx ProductService__ConnectionString "Server=localhost,1433;Database=ProductServiceDb;User Id=sa;Password=YourPassword123;"

REM LogService
echo Setting LogService Configuration...
setx LogService__ConnectionString "Server=localhost,1433;Database=LogServiceDb;User Id=sa;Password=YourPassword123;"

REM API Gateway
echo Setting ApiGateway Configuration...
setx ApiGateway__AuthServiceUrl "http://localhost:5001"
setx ApiGateway__ProductServiceUrl "http://localhost:5002"
setx ApiGateway__LogServiceUrl "http://localhost:5003"

REM Logging
echo Setting Logging Configuration...
setx ASPNETCORE_ENVIRONMENT "Development"
setx Serilog__MinimumLevel "Information"

echo.
echo =========================================
echo Environment variables have been set!
echo Please restart your terminal or IDE for changes to take effect.
echo =========================================
pause
