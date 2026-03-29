FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Services/ProductService/ProductService.API/ProductService.API.csproj", "Services/ProductService/ProductService.API/"]
COPY ["Services/ProductService/ProductService.Application/ProductService.Application.csproj", "Services/ProductService/ProductService.Application/"]
COPY ["Services/ProductService/ProductService.Domain/ProductService.Domain.csproj", "Services/ProductService/ProductService.Domain/"]
COPY ["Services/ProductService/ProductService.Infrastructure/ProductService.Infrastructure.csproj", "Services/ProductService/ProductService.Infrastructure/"]
COPY ["BuildingBlocks/Shared.Contracts/Shared.Contracts.csproj", "BuildingBlocks/Shared.Contracts/"]

RUN dotnet restore "Services/ProductService/ProductService.API/ProductService.API.csproj"

COPY . .
RUN dotnet publish "Services/ProductService/ProductService.API/ProductService.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

RUN adduser --disabled-password appuser && chown -R appuser /app
COPY --from=build --chown=appuser:appuser /app/publish .

USER appuser
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "ProductService.API.dll"]
