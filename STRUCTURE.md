# Proje Yapısı Özeti

## İçerik Envanteri

### Solution Root
```
MicroservicesCase.sln          - Ana solution dosyası
README.md                      - Proje dokumentasyonu
STRUCTURE.md                   - Bu dosya (yapı özeti)
.git/                          - Git repository
src/                           - Tüm proje kodları
```

## Service Katmanları (Layered Architecture)

Her servis (AuthService, ProductService, LogService) şu 4 katmane ayrılmıştır:

### 1. Domain Layer (AuthService.Domain, ProductService.Domain, LogService.Domain)
**Amaç**: Temel iş kuralları ve varlık tanımları
**Klasörler**:
- `Entities/`      - Domain entity'leri (User.cs, Product.cs, ActivityLog.cs)
- `Interfaces/`    - Domain interface'leri (IUserRepository.cs)

**NuGet Paketleri**: MediatR

### 2. Application Layer (AuthService.Application, ProductService.Application, LogService.Application)
**Amaç**: Use case'leri ve CQRS işlemlerini yönet
**Klasörler**:
- `Handlers/`      - CQRS command/query handler'ları (GetUserByIdQueryHandler.cs)
- `DTOs/`          - Data Transfer Object'ler (UserDto.cs)
- `Interfaces/`    - Servis interface'leri (IAuthenticationService.cs)

**NuGet Paketleri**: MediatR
**Referanslar**: Domain layer

### 3. Infrastructure Layer (AuthService.Infrastructure, ProductService.Infrastructure, LogService.Infrastructure)
**Amaç**: Veritabanı, cache ve dış servisler
**Klasörler**:
- `Data/`          - Entity Framework DbContext (AuthDbContext.cs)
- `Repositories/`  - Veritabanı operasyonları (UserRepository.cs)

**NuGet Paketleri**: MediatR, EntityFrameworkCore, EntityFrameworkCore.SqlServer
**Referanslar**: Application layer

### 4. API Layer (AuthService.API, ProductService.API, LogService.API)
**Amaç**: REST endpoint'leri ve middleware
**Dosyalar & Klasörler**:
- `Controllers/`   - API controller'ları (AuthController.cs)
- `Middleware/`    - Custom middleware'ler
- `Program.cs`     - Application startup
- `appsettings.json` - Konfigürasyon

**NuGet Paketleri**: MediatR, EntityFrameworkCore, Serilog.AspNetCore
**Referanslar**: Infrastructure layer

## ApiGateway Servisi
**Amaç**: Merkezi routing ve gateway işlemleri
**Dosyalar & Klasörler**:
- `Controllers/`   - Gateway controller'ları
- `Middleware/`    - Gateway middleware'leri
- `Program.cs`     - Startup
- `appsettings.json` - Konfigürasyon

**NuGet Paketleri**: MediatR, Serilog.AspNetCore

## Dosya Başvurusu

### Örnek Files (AuthService için)
```
AuthService/
├── AuthService.Domain/
│   ├── AuthService.Domain.csproj
│   ├── Entities/
│   │   └── User.cs ✓ (Oluşturuldu)
│   └── Interfaces/
│       └── IUserRepository.cs ✓ (Oluşturuldu)
│
├── AuthService.Application/
│   ├── AuthService.Application.csproj
│   ├── Handlers/
│   │   └── GetUserByIdQueryHandler.cs ✓ (Oluşturuldu)
│   ├── DTOs/
│   │   └── UserDto.cs ✓ (Oluşturuldu)
│   └── Interfaces/
│       └── IAuthenticationService.cs ✓ (Oluşturuldu)
│
├── AuthService.Infrastructure/
│   ├── AuthService.Infrastructure.csproj
│   ├── Data/
│   │   └── AuthDbContext.cs ✓ (Oluşturuldu)
│   └── Repositories/
│       └── UserRepository.cs ✓ (Oluşturuldu)
│
└── AuthService.API/
    ├── AuthService.API.csproj
    ├── Controllers/
    │   └── AuthController.cs ✓ (Oluşturuldu)
    ├── Middleware/
    ├── Program.cs ✓ (Oluşturuldu)
    └── appsettings.json ✓ (Oluşturuldu)
```

### ProductService & LogService
Aynı yapı ile oluşturuldu (örnek Entity dosyaları eklenmiştir)

### ApiGateway
```
ApiGateway/
├── ApiGateway.csproj
├── Controllers/
│   └── GatewayController.cs ✓ (Oluşturuldu)
├── Middleware/
├── Program.cs ✓ (Oluşturuldu)
└── appsettings.json ✓ (Oluşturuldu)
```

## Proje Referanslı Ilişkiler (Dependency Flow)

```
AuthService.API
    ↓ references
AuthService.Infrastructure
    ↓ references
AuthService.Application
    ↓ references
AuthService.Domain

[Aynı pattern ProductService ve LogService için tekrarlanır]
```

## NuGet Paketleri Çıkışı

| Paket | Versiyon | Kullanıldığı Layer | Amaç |
|-------|----------|-------------------|------|
| MediatR | 12.1.1 | Tüm katmanlar | CQRS Pattern |
| EntityFrameworkCore | 8.0.0 | Infrastructure, API | ORM |
| EntityFrameworkCore.SqlServer | 8.0.0 | Infrastructure | SQL Server Provider |
| Serilog.AspNetCore | 7.0.0 | API | Structured Logging |

## Target Framework
- .NET 8.0 (LTS)

## Geliştirmeye Başlamak

1. **Solution'u aç**: `MicroservicesCase.sln` dosyasını Visual Studio'da açın
2. **Paketleri geri yükle**: NuGet paketleri otomatik yüklenecektir
3. **Örnek dosyaları inceyin**: AuthService'teki örnek dosyalar pattern'i gösterir
4. **DbContext'i yapılandırın**: Infrastructure katmanında DbContext'i tamamlayın
5. **Repository'leri uygulayın**: Infrastructure katmanında repository'leri implement edin
6. **Handler'ları yazın**: Application katmanında CQRS handler'larını yazın
7. **Controller'lar ekleyin**: API katmanında endpoint'leri expose edin

## Önemli Notlar

✓ Tüm katmanlar uygun şekilde referans alıyor (Clean Architecture)
✓ MediatR CQRS pattern'i tüm katmanlara entegre
✓ Serilog structured logging yapılandırılmış
✓ Entity Framework Core ayarlanmışür
✓ Her servis bağımsız ve ölçeklenebilir

---

Oluşturulma Tarihi: 2026-03-24
