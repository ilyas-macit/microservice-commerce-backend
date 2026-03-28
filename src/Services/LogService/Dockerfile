FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["LogService.API/LogService.API.csproj", "LogService.API/"]
COPY ["LogService.Application/LogService.Application.csproj", "LogService.Application/"]
COPY ["LogService.Domain/LogService.Domain.csproj", "LogService.Domain/"]
COPY ["LogService.Infrastructure/LogService.Infrastructure.csproj", "LogService.Infrastructure/"]

RUN dotnet restore "LogService.API/LogService.API.csproj"

COPY . .
RUN dotnet publish "LogService.API/LogService.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

RUN adduser --disabled-password appuser && chown -R appuser /app
COPY --from=build --chown=appuser:appuser /app/publish .

USER appuser
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "LogService.API.dll"]
