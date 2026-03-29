FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Services/AuthService/AuthService.API/AuthService.API.csproj", "Services/AuthService/AuthService.API/"]
COPY ["Services/AuthService/AuthService.Application/AuthService.Application.csproj", "Services/AuthService/AuthService.Application/"]
COPY ["Services/AuthService/AuthService.Domain/AuthService.Domain.csproj", "Services/AuthService/AuthService.Domain/"]
COPY ["Services/AuthService/AuthService.Infrastructure/AuthService.Infrastructure.csproj", "Services/AuthService/AuthService.Infrastructure/"]

RUN dotnet restore "Services/AuthService/AuthService.API/AuthService.API.csproj"

COPY . .
RUN dotnet publish "Services/AuthService/AuthService.API/AuthService.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

RUN adduser --disabled-password appuser && chown -R appuser /app
COPY --from=build --chown=appuser:appuser /app/publish .

USER appuser
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "AuthService.API.dll"]
