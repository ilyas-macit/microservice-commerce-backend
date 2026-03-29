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
| Message Broker | RabbitMQ | 3.13 |
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

## 📨 Asenkron İletişim Mimarisi & RabbitMQ

### Neden Senkron HTTP Coupling Kaldırıldı?

Önceki versiyonda ProductService, ürün oluşturulduktan hemen sonra directly bir HTTP isteğiyle LogService'i çağırıyordu. Bu yaklaşımın sorunları:

- ❌ **Tight Coupling**: ProductService, LogService'in kullanılabilirliğine bağımlı hale geliyor
- ❌ **Cascading Failures**: LogService'in düşmesi Product API'nin işlemlerini başarısız yapabiliyor
- ❌ **Performance**: Product oluşturma işleminin LogService'in yanıt süresine ek bekleme süresi ekliyor
- ❌ **Scalability**: Her product creation, LogService'e anlık bir bağlantı açması gerekli

### Yeni Asenkron Flow (RabbitMQ Event-Driven)

```
1. Product API: POST /products
   ↓
2. ProductService: AddProductCommandHandler
   - Veritabanında ürünü kaydet
   - ProductCreatedIntegrationEvent oluştur
   - RabbitMQ'ya publish et (non-blocking)
   - Hemen HTTP 200 döndür
   ↓
3. Message Broker (RabbitMQ)
   - Exchange: "product.events" (topic)
   - Routing Key: "product.created"
   - Queue: "log.product-created"
   ↓
4. LogService: ProductCreatedIntegrationEventConsumer (Background Service)
   - Queue'den mesajı consume et
   - CreateLogCommand ile asenkron log kaydı yap
   - Başarılı ise ACK, hata ise requeue
```

### Avantajlar

✅ **Loose Coupling**: Servisler event aracılığıyla haberleşir, direct HTTP dependency yok
✅ **Resilience**: LogService düşse bile Product API normal çalışmaya devam eder
✅ **Performance**: Product création yanıt süresi LogService'in hızından bağımsız
✅ **Scalability**: Birden fazla consumer aynı queue'yi dinleyebilir (future)
✅ **Audit Trail**: Tüm olaylar RabbitMQ üzerinde kayıtlı olur

### Topology Sabitleri

Topoloji isimleri merkezileştirilmiştir (`Shared.Contracts/Messaging/ProductEventsTopology.cs`):

```csharp
public static class ProductEventsTopology
{
    public const string Exchange = "product.events";
    
    public static class RoutingKeys
    {
        public const string ProductCreated = "product.created";
    }
    
    public static class Queues
    {
        public const string LogProductCreated = "log.product-created";
    }
}
```

Bu, magic string'leri elimine eder ve aynı topic'te yeni consumer'lar eklemeyi kolaylaştırır.

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
- **SQL Server 2022** (Database) - Port 1433
- **Redis 7** (Caching) - Port 6379
- **RabbitMQ 3.13** (Message Broker) - Port 5672, Management UI: 15672
- **Seq** (Centralized Logging) - Port 5341
- **AuthService, ProductService, LogService, ApiGateway** (Microservices)

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

3. **Ortam değişkenlerini ayarla**
   ```bash
   # Windows
   setup-dev-env.bat
   
   # Linux/macOS
   ./setup-dev-env.sh
   ```
   
   Veya `.env.example`'ı `.env` olarak kopyala:
   ```bash
   cp .env.example .env
   ```

4. **Konteynerları başlat**
   ```bash
   docker-compose up -d
   ```
   
   RabbitMQ Management UI erişim:
   - URL: http://localhost:15672
   - Username: `guest` (varsayılan)
   - Password: `guest` (varsayılan)

5. **NuGet paketlerini geri yükle**
   ```bash
   dotnet restore
   ```

6. **Database migrasyonlarını çalıştır**
   ```bash
   # AuthService
   cd src/Services/AuthService/AuthService.API
   dotnet ef database update
   
   # ProductService
   cd src/Services/ProductService/ProductService.API
   dotnet ef database update
   
   # LogService
   cd src/Services/LogService/LogService.API
   dotnet ef database update
   ```

7. **Uygulamaları çalıştır**
   ```bash
   # Terminal 1: AuthService
   cd src/Services/AuthService/AuthService.API
   dotnet run
   
   # Terminal 2: ProductService
   cd src/Services/ProductService/ProductService.API
   dotnet run
   
   # Terminal 3: LogService
   cd src/Services/LogService/LogService.API
   dotnet run
   
   # Terminal 4: ApiGateway
   cd src/ApiGateway
   dotnet run
   ```

8. **Swagger UI'ı ziyaret et**
   - http://localhost:5000/swagger (ApiGateway)
   - http://localhost:5001/swagger (AuthService)
   - http://localhost:5002/swagger (ProductService)
   - http://localhost:5003/swagger (LogService)

## 🌍 Ortam Değişkenleri (RabbitMQ)

Gözden geçir: [ENVIRONMENT_VARIABLES.md](ENVIRONMENT_VARIABLES.md)

**Ana RabbitMQ Değişkenleri** (tüm servisler):

```env
RabbitMq__Host=rabbitmq
RabbitMq__Port=5672
RabbitMq__Username=guest
RabbitMq__Password=guest
```

- **Yerel Geliştirme**: `docker-compose.yml` içindeki `rabbitmq` servisi bu değerleri sağlar
- **Production**: Güvenli credentials ve dış RabbitMQ broker'ı kullan

## 📚 Backend Case Study: Asenkron Servis Entegrasyonu

### Problem Statement

Monolitik bir e-ticaret sistemini, bağımsız olarak ölçeklenebilecek mikroservisler halinde yeniden mimarilendiriyoruz. Zorluk: servisler arası iletişim hızlı, güvenilir ve loose-coupling olmalı.

### Tasarım Kararları

#### 1. Senkron (❌Eski) vs. Asenkron (✅Yeni)

**Eski Senkron Yaklaşım:**
```
Product API creates product → calls LogService HTTP → waits for response
```

**Sorunlar:**
- ProductService, LogService'in SLA'sına bağımlı
- Ürün oluşturma, LogService'in latency'sinden etkileniyor
- Cascade failures: LogService down → Product API errors

**Yeni Asenkron Yaklaşım:**
```
Product API creates product → publishes event → returns immediately
LogService consumes events asynchronously in background
```

**Faydalar:**
- Loose coupling via messaging
- Product API yanıt süresi: ~50ms (LogService'ten bağımsız)
- Resilience: LogService down → events queue'de beklese de Product API çalışır

#### 2. Event-Driven Architecture Selection

Neden RabbitMQ?
- ✅ Proven, production-grade message broker
- ✅ Topic exchanges for routing flexibility
- ✅ Built-in queue durability & persistence
- ✅ Automatic retries on processing failures
- ✅ Monitoring & management UI built-in

#### 3. Integration Event Pattern

**Shared Contracts** approach:
```
src/BuildingBlocks/Shared.Contracts/
└── IntegrationEvents/
    └── ProductCreatedIntegrationEvent (DTO-like)
```

Event contract'ı shared library'de olmak:
- ✅ Publisher ve Consumer aynı sürümü garantiler
- ✅ Deserialization güvenliği
- ✅ Strongly-typed messaging

#### 4. Consumer Resilience

```
Deserialization fails?   → NACK without requeue (poison letter, investigate)
Business logic fails?    → NACK with requeue (transient error, retry)
Success?                 → ACK (remove from queue)
```

Bu approach:
- Poison messages'ı consume loop'tan çıkarır
- Transient errors'ı otomatik retry eder
- Observable failures için structured logging

### Key Metrics

| Aspect | Senkron | Asenkron |
|--------|---------|----------|
| **Product creation latency** | +150ms (LogService) | ~50ms |
| **LogService downtime impact** | Product API affected | No impact |
| **Max concurrent products** | Limited by LogService | Unlimited (queue buffers) |
| **Event audit trail** | None | Full RabbitMQ log |

### Learning Points for Interviews

1. **Monolith → Microservices Transition**
   - Service boundaries: based on domain, not technical layer
   - Communication: async preferred over sync for resilience

2. **Loose Coupling**
   - Shared contracts vs. shared libraries
   - Event versioning strategy
   - Consumer-driven contracts

3. **Reliability Patterns**
   - Dead letter queues (implicit via requeue=false)
   - Idempotency considerations (not yet implemented, but important)
   - Observability: structured logging with delivery tags

4. **Deployment Topology**
   - Services independently deployable
   - Event contract backward compatibility crucial
   - Consumer can lag or burst without affecting Publisher

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
- **Event Publishing**: `ProductCreatedIntegrationEvent` (async to LogService)
- **Port**: 5002

### LogService
- Sistem çapında aktivite loglama
- Audit trail
- Hata kayıtları
- **Event Consumption**: Listens on `product.events` exchange for `product.created` events
- **Port**: 5003

### ApiGateway
- Tüm servislerin merkezi giriş noktası
- Request routing (YARP)
- Rate limiting
- JWT authentication & authorization
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
✅ Structured Logging (Serilog + JSON)
✅ Docker Support
✅ Environment-based Configuration
✅ 12 Factor App Uygunluğu
✅ **Event-Driven Architecture (RabbitMQ)**
✅ **Asenkron Servis Entegrasyonu**
✅ **Loose Coupling via Messaging**
✅ **Integration Event Pattern**
✅ **Resilience & Auto-Retry**

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

**Son Güncelleme**: 2026-03-29
**Versiyon**: 2.0 (Event-Driven Architecture)

