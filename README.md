# Microservice Commerce Backend

Bu, çok katmanlı mikroservis mimarisiyle oluşturulan bir e-ticaret backend çözümüdür.

## 🚀 Hızlı Başlangıç

Hazırlıklı mı? [QUICKSTART.md](QUICKSTART.md) dosyasını takip et - 5 dakika içinde çalışır!

## 📖 Dokumentasyon

- **[QUICKSTART.md](QUICKSTART.md)** - 5 dakikalık başlangıç kılavuzu
- **[STRUCTURE.md](STRUCTURE.md)** - Detaylı proje yapısı ve dosya envanteri
- **[CONFIGURATION_SUMMARY.md](CONFIGURATION_SUMMARY.md)** - Konfigürasyon özeti ve kontrol listesi
- **[ENVIRONMENT_VARIABLES.md](ENVIRONMENT_VARIABLES.md)** - Ortam değişkenleri yönetimi
- **.env.example** - Ortam değişkenleri şablonu

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
│       │   └── AuthService.API/ (Web API) - Port: 5001
│       │       ├── Controllers/
│       │       └── Middleware/
│       │
│       ├── ProductService/
│       │   ├── ProductService.Domain/ (Class Library)
│       │   ├── ProductService.Application/ (Class Library)
│       │   ├── ProductService.Infrastructure/ (Class Library)
│       │   └── ProductService.API/ (Web API) - Port: 5002
│       │
│       ├── LogService/
│       │   ├── LogService.Domain/ (Class Library)
│       │   ├── LogService.Application/ (Class Library)
│       │   ├── LogService.Infrastructure/ (Class Library)
│       │   └── LogService.API/ (Web API) - Port: 5003
│       │
│       └── ApiGateway/ (Web API) - Port: 5000
```

## 🔧 Teknoloji Stack'i

| Kategori | Teknoloji | Versiyon |
|----------|-----------|---------|
| Framework | .NET | 8.0 (LTS) |
| Web Framework | ASP.NET Core | 8.0 |
| ORM | Entity Framework Core | 8.0 |
| CQRS | MediatR | 12.1.1 |
| Logging | Serilog | 7.0.0 |
| Database | SQL Server | 2022 |
| Caching | Redis | 7.0 |
| Logging Service | Seq | Latest |

## 🏗️ Mimarisek

### Katmanlar (Clean Architecture)

1. **Domain Layer** (Entity Framework'ten bağımsız)
   - Varlık tanımları (Entities)
   - Domain interface'leri
   - İş kuralları

2. **Application Layer** (Use Cases)
   - CQRS Command/Query Handlers (MediatR)
   - DTO'lar
   - Servis interface'leri

3. **Infrastructure Layer** (Teknik detallar)
   - Entity Framework DbContext
   - Repository implementasyonları
   - Dış servis integrationları

4. **API Layer** (REST Endpoints)
   - Controller'lar
   - Middleware'ler
   - Configuration

### Reference Akışı

```
API Layer → Infrastructure Layer → Application Layer → Domain Layer
```

Geriye doğru referans **YASAKLANMIŞTIR** (No circular dependencies).

## 🔒 Güvenlik & 12 Factor App

✅ **Ortam Değişkenleri Yönetimi**
- JWT Secret, Connection Strings gibi hassas bilgiler `appsettings.json` dosyasında yer almaz
- Ortam değişkenlerinden okunur
- `.env` dosyaları `.gitignore` tarafından ignore edilir

✅ **Konfigürasyon Hiyerarşisi**
1. `appsettings.json` (temel ayarlar)
2. `appsettings.{Environment}.json` (ortama özgü)
3. Ortam değişkenleri (en yüksek öncelik)

## 📊 Service Portları

| Service | HTTP | HTTPS | Swagger UI |
|---------|------|-------|-----------|
| ApiGateway | 5000 | 7000 | http://localhost:5000/swagger |
| AuthService | 5001 | 7001 | http://localhost:5001/swagger |
| ProductService | 5002 | 7002 | http://localhost:5002/swagger |
| LogService | 5003 | 7003 | http://localhost:5003/swagger |

## 🐳 Docker Destek

`docker-compose.yml` ile geliştirme ortamını başlat:

```bash
# Başlat
docker-compose up -d

# Durdur
docker-compose down

# Logları görüntüle
docker-compose logs -f
```

Aktif Konteynerler:
- **SQL Server 2022** (Database)
- **Redis 7** (Caching)
- **Seq** (Centralized Logging)

## 📝 Swagger Entegrasyonu

Her servis Swagger/OpenAPI desteği ile gelir. Geliştirme ortamında tüm endpoint'leri İncelemenizi sağlar.

## 📊 Serilog JSON Logging

Structured logging formatı:
```json
{
  "Timestamp": "2026-03-24T10:30:45Z",
  "Level": "Information",
  "MessageTemplate": "User action",
  "Properties": { "UserId": "123" }
}
```

Loglar:
- ✅ Konsola yazılır (gerçek zamanlı)
- ✅ Dosyaya yazılır (`logs/` klasöründe, günlük rotation)
- 📊 Seq'e gönderilebilir (merkezi logging)

## 🔄 Başlama Adımları

1. **Ön Gereksinimler**
   - .NET 8 SDK
   - Docker Desktop
   - Visual Studio 2022 veya VS Code

2. **Repository'i klonla**
   ```bash
   git clone <url>
   cd microservice-commerce-backend
   ```

3. **Konteynerları başlat**
   ```bash
   docker-compose up -d
   ```

4. **Ortam değişkenlerini ayarla**
   - Windows: `setup-dev-env.bat`
   - Linux/macOS: `./setup-dev-env.sh`

5. **NuGet paketlerini geri yükle**
   ```bash
   dotnet restore
   ```

6. **Uygulamaları çalıştır**
   ```bash
   cd src/Services/AuthService/AuthService.API
   dotnet run
   # ...diğer servisler için tekrar et
   ```

7. **Swagger UI'ı ziyaret et**
   - http://localhost:5000/swagger (ApiGateway)
   - http://localhost:5001/swagger (AuthService)
   - http://localhost:5002/swagger (ProductService)
   - http://localhost:5003/swagger (LogService)

## 📚 Detaylı Rehberler

### Yeni Başlayanlar
- [QUICKSTART.md](QUICKSTART.md) - İlk 5 dakika

### Geliştirme
- [STRUCTURE.md](STRUCTURE.md) - Proje yapısını anlama
- [CONFIGURATION_SUMMARY.md](CONFIGURATION_SUMMARY.md) - Yapılandırma detayları
- [ENVIRONMENT_VARIABLES.md](ENVIRONMENT_VARIABLES.md) - Environment setup

## 🔗 Service'ler

### AuthService
- Kullanıcı kimlik doğrulaması (Authentication)
- JWT token oluşturma ve doğrulama
- Rol ve yetki yönetimi (Authorization)
- **Port**: 5001

### ProductService
- Ürün verı tabanı
- Ürün yönetimi (CRUD)
- Envanter yönetimi
- **Port**: 5002

### LogService
- Sistem çapında aktivite loglama
- Audit trail
- Hata kayıtları
- **Port**: 5003

### ApiGateway
- Tüm servislerin merkezi giriş noktası
- Request routing
- Load balancing (future)
- Request/Response transformation
- **Port**: 5000

## ✨ Özellikler

✅ Clean Architecture (Domain-Driven Design)
✅ CQRS Pattern (MediatR)
✅ Layered Architecture
✅ Repository Pattern
✅ Dependency Injection
✅ Entity Framework Core
✅ Swagger/OpenAPI
✅ Structured Logging (Serilog)
✅ Docker Support
✅ Environment-based Configuration
✅ 12 Factor App Uygunluğu

## 🤝 Katkı Kuralları

1. Feature branch oluştur (`git checkout -b feature/AmazingFeature`)
2. Değişiklikleri commit et (`git commit -m 'Add AmazingFeature'`)
3. Branch'ı push et (`git push origin feature/AmazingFeature`)
4. Pull Request aç

## 📄 Lisans

Bu proje [MIT License](LICENSE) altında lisanslanmıştır.

## 📧 İletişim

Sorularız mı? [Issues](https://github.com/yourusername/microservice-commerce-backend/issues) açabilirsin.

---

**Son Güncelleme**: 2026-03-24
**Versiyon**: 1.0

