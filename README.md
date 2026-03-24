# Microservice Commerce Backend

Bu, çok katmanlı mikroservis mimarisiyle oluşturulan bir e-ticaret backend çözümüdür.

## Proje Yapısı

### Solution: MicroservicesCase.sln

```
MicroservicesCase
├── src/
│   └── Services/
│       ├── AuthService/
│       │   ├── AuthService.Domain/ (Class Library)
│       │   │   ├── Entities/
│       │   │   └── Interfaces/
│       │   ├── AuthService.Application/ (Class Library)
│       │   │   ├── Handlers/
│       │   │   ├── DTOs/
│       │   │   └── Interfaces/
│       │   ├── AuthService.Infrastructure/ (Class Library)
│       │   │   ├── Repositories/
│       │   │   └── Data/
│       │   └── AuthService.API/ (Web API)
│       │       ├── Controllers/
│       │       └── Middleware/
│       │
│       ├── ProductService/
│       │   ├── ProductService.Domain/ (Class Library)
│       │   │   ├── Entities/
│       │   │   └── Interfaces/
│       │   ├── ProductService.Application/ (Class Library)
│       │   │   ├── Handlers/
│       │   │   ├── DTOs/
│       │   │   └── Interfaces/
│       │   ├── ProductService.Infrastructure/ (Class Library)
│       │   │   ├── Repositories/
│       │   │   └── Data/
│       │   └── ProductService.API/ (Web API)
│       │       ├── Controllers/
│       │       └── Middleware/
│       │
│       ├── LogService/
│       │   ├── LogService.Domain/ (Class Library)
│       │   │   ├── Entities/
│       │   │   └── Interfaces/
│       │   ├── LogService.Application/ (Class Library)
│       │   │   ├── Handlers/
│       │   │   ├── DTOs/
│       │   │   └── Interfaces/
│       │   ├── LogService.Infrastructure/ (Class Library)
│       │   │   ├── Repositories/
│       │   │   └── Data/
│       │   └── LogService.API/ (Web API)
│       │       ├── Controllers/
│       │       └── Middleware/
│       │
│       └── ApiGateway/ (Web API)
│           ├── Controllers/
│           └── Middleware/
```

## Katmanlar (Layers)

### 1. Domain Layer
- **Amaç**: Temel iş mantığı ve varlık tanımlarını içerir
- **İçerik**: Entity'ler, Interface'ler, Value Objects, Domain Rules

### 2. Application Layer
- **Amaç**: Use case'leri ve CQRS operasyonlarını handle eder
- **İçerik**: Command/Query Handlers (MediatR), DTO'lar, Application Services
- **Bağımlılık**: Domain layer'a referans verir

### 3. Infrastructure Layer
- **Amaç**: Veritabanı, cache ve dış servisleri implement eder
- **İçerik**: Entity Framework DbContext, Repository implementasyonları, Redis, HTTP clients
- **Bağımlılık**: Application layer'a referans verir

### 4. API Layer
- **Amaç**: REST endpoint'lerini ve middleware'leri expose eder
- **İçerik**: Controllers, Middleware, Program.cs, Configuration
- **Bağımlılık**: Infrastructure layer'a referans verir

## NuGet Paketleri

Tüm projelerde yüklü paketler:
- **MediatR (12.1.1)**: CQRS pattern implementasyonu
- **Microsoft.EntityFrameworkCore (8.0.0)**: ORM (Infrastructure ve API layer'larında)
- **Serilog.AspNetCore (7.0.0)**: Structured Logging (API layer'larında)

## Target Framework

- **.NET 8.0**: Modern, long-term support sürümü

## Başlama

1. Solution'ı açın: `MicroservicesCase.sln`
2. NuGet paketlerini geri yükleyin
3. Database migration'larını çalıştırın
4. API'leri başlatın

## Referans Yapısı

```
AuthService.Application → AuthService.Domain
AuthService.Infrastructure → AuthService.Application
AuthService.API → AuthService.Infrastructure

ProductService.Application → ProductService.Domain
ProductService.Infrastructure → ProductService.Application
ProductService.API → ProductService.Infrastructure

LogService.Application → LogService.Domain
LogService.Infrastructure → LogService.Application
LogService.API → LogService.Infrastructure
```

## Service'ler

### AuthService
- Kullanıcı kimlik doğrulaması ve JWT token yönetimi
- Rol ve yetki yönetimi

### ProductService
- Ürün database ve işlemleri
- Envanter yönetimi

### LogService
- Sistem çapında logging
- Audit trail

### ApiGateway
- Tüm servislerin merkezi giriş noktası
- Request routing ve load balancing
