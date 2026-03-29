FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Services/LogService/LogService.API/LogService.API.csproj", "Services/LogService/LogService.API/"]
COPY ["Services/LogService/LogService.Application/LogService.Application.csproj", "Services/LogService/LogService.Application/"]
COPY ["Services/LogService/LogService.Domain/LogService.Domain.csproj", "Services/LogService/LogService.Domain/"]
COPY ["Services/LogService/LogService.Infrastructure/LogService.Infrastructure.csproj", "Services/LogService/LogService.Infrastructure/"]
COPY ["BuildingBlocks/Shared.Contracts/Shared.Contracts.csproj", "BuildingBlocks/Shared.Contracts/"]

RUN dotnet restore "Services/LogService/LogService.API/LogService.API.csproj"

COPY . .
RUN dotnet publish "Services/LogService/LogService.API/LogService.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

RUN adduser --disabled-password appuser && chown -R appuser /app
COPY --from=build --chown=appuser:appuser /app/publish .

USER appuser
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "LogService.API.dll"]
