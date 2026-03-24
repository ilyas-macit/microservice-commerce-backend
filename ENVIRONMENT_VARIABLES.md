# Environment Variables Configuration

Hassas bilgiler (JWT Secret, Connection Strings) `appsettings.json` dosyasında yer almayacak, bunun yerine ortam değişkenleri (Environment Variables) üzerinden okunacaktır. Bu, 12 Factor App prensiplerine uygun bir yaklaşımdır.

## Ortam Değişkenleri Konfigürasyonu

### Windows PowerShell'de Ayarlama

```powershell
# AuthService için
$env:Jwt__Secret = "your-jwt-secret-key-here"
$env:Jwt__Issuer = "AuthService"
$env:Jwt__Audience = "MicroserviceClients"
$env:ConnectionStrings__DefaultConnection = "Server=localhost;Database=AuthServiceDb;User Id=sa;Password=YourPassword123;"

# ProductService için
$env:Jwt__Secret = "your-jwt-secret-key-here"
$env:Jwt__Issuer = "ProductService"
$env:Jwt__Audience = "MicroserviceClients"
$env:ConnectionStrings__DefaultConnection = "Server=localhost;Database=ProductServiceDb;User Id=sa;Password=YourPassword123;"

# LogService için
$env:Jwt__Secret = "your-jwt-secret-key-here"
$env:Jwt__Issuer = "LogService"
$env:Jwt__Audience = "MicroserviceClients"
$env:ConnectionStrings__DefaultConnection = "Server=localhost;Database=LogServiceDb;User Id=sa;Password=YourPassword123;"
```

### .NET Environment Variables Format

.NET ortam değişkenleri `:` (iki nokta) yerine `__` (çift alt çizgi) kullanır. Yapılandırma hiyerarşisi otomatik olarak eşleştirilir:

```
appsettings.json:
{
  "Jwt": {
    "Secret": "value"
  }
}

Ortam Değişkeni: Jwt__Secret = "value"
```

## Servislerin Portları

| Servis | HTTP | HTTPS |
|--------|------|-------|
| AuthService | 5001 | 7001 |
| ProductService | 5002 | 7002 |
| LogService | 5003 | 7003 |
| ApiGateway | 5000 | 7000 |

## Geliştirme Ortamına Özgü Konfigürasyon

`appsettings.Development.json` dosyaları oluşturarak geliştirme ortamına özgü ayarlar yapabilirsiniz:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  },
  "Serilog": {
    "MinimumLevel": "Debug"
  }
}
```

## Konfigürasyon Okuma Sırası

`Program.cs` dosyasında tanımlanan konfigürasyon şöyledir:

```csharp
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();
```

Bu şu sırayla değerleri okur:
1. `appsettings.json` (genel ayarlar)
2. `appsettings.Development.json` veya `appsettings.Production.json` (ortama özgü ayarlar)
3. Ortam değişkenleri (en son, bu nedenle en yüksek önceliğe sahip)

## JSON Logging Konfigürasyonu

Tüm servisler Serilog ile JSON formatında loglama yapacak şekilde yapılandırılmıştır:

```json
"WriteTo": [
  {
    "Name": "Console",
    "Args": {}
  },
  {
    "Name": "File",
    "Args": {
      "path": "logs/service-log-.txt",
      "rollingInterval": "Day",
      "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
    }
  }
]
```

Bu konfigürasyon:
- Loglama çıktısını konsola ve dosyaya yazar
- Her gün yeni bir log dosyası oluşturur (rolling)
- Tarih, zaman, seviye ve mesajı JSON formatında kaydeder

## SQL Server Connection String Örneği

```
Server=localhost,1433;Database=AuthServiceDb;User Id=sa;Password=YourPassword123;
```

`Password=;` parametresi boş olarak bırakılmış, artık ortam değişkeninden okunacaktır.

## Güvenlik Best Practices

✅ **Yapılması gerekenler:**
- Ortam değişkenleri üzerinden hassas bilgiler yönet
- JWT Secret'ı güvenli bir yerde sakla (Key Vault, HashiCorp Vault vb.)
- Connection String'te şifreler boş bırak, ortam değişkeninden oku
- Production ortamında HTTPS kullan

❌ **Yapılmaması gerekenler:**
- Şifreler ve Secret'ları `appsettings.json` dosyasına yazma
- Hassas bilgileri source code'a commit etme
- JWT Secret'ı sadece client-side'da sakla

## Local Development için .env Dosyası (Opsiyonel)

Eğer bir .env dosyası kullanmak istiyorsanız, dotenv-cli paketini NuGet'ten kurabilirsiniz:

```bash
dotnet tool install -g dotenv-cli
```

Ardından `.env` dosyası oluşturup değişkenleri tanımlayabilirsiniz. Ancak, bu dosya hiçbir zaman source code'a commit edilmemelidir.

---

**Oluşturulma Tarihi**: 2026-03-24
