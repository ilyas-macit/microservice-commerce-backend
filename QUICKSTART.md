# Quick Start Guide

## 🚀 5 Dakika ile Başlayın

### Step 1: Ön Gereksinimler

Aşağıdakilerin yüklü olduğundan emin olun:

- ✅ .NET 8 SDK ([indir](https://dotnet.microsoft.com/download))
- ✅ Visual Studio 2022 veya VS Code
- ✅ Docker Desktop ([indir](https://www.docker.com/products/docker-desktop))
- ✅ Git

### Step 2: Repository'i Clone Edin

```bash
git clone <repository-url>
cd microservice-commerce-backend
```

### Step 3: Docker Ortamını Başlatın

```bash
# SQL Server, Redis ve Seq başlat
docker-compose up -d

# Konteynerler çalışıyor mu kontrol et
docker-compose ps
```

Beklenen çıktı:
```
NAME                    STATUS
mssql-server-2022       Up (healthy)
redis-cache             Up (healthy)
seq-logging             Up (healthy)
```

### Step 4: Ortam Değişkenlerini Ayarla

#### Windows (PowerShell):
```powershell
# setup-dev-env.bat'ı çalıştır
.\setup-dev-env.bat

# Terminal'i yeniden başlat
```

#### Linux/macOS:
```bash
# Script'e execute izni ver
chmod +x setup-dev-env.sh

# Çalıştır
./setup-dev-env.sh

# Profile'ı güncelle
source ~/.bashrc  # veya source ~/.zshrc
```

### Step 5: Çözümü Visual Studio'da Aç

```bash
# VS Code
code .

# Veya Visual Studio 2022
start MicroservicesCase.sln
```

### Step 6: NuGet Paketlerini Geri Yükle

```bash
dotnet restore
```

### Step 7: Database'leri Oluştur (Opsiyonel)

```bash
# AuthService
cd src/Services/AuthService/AuthService.Infrastructure
dotnet ef database update

# ProductService
cd ../../../ProductService/ProductService.Infrastructure
dotnet ef database update

# LogService
cd ../../../LogService/LogService.Infrastructure
dotnet ef database update

# Geri ana dizine dön
cd ../../../..
```

### Step 8: Uygulamaları Çalıştır

**Option A: VS Code/Visual Studio'da**

Her servis için ayrı terminal açıp:

```bash
# Terminal 1 - AuthService (5001)
cd src/Services/AuthService/AuthService.API
dotnet run

# Terminal 2 - ProductService (5002)
cd src/Services/ProductService/ProductService.API
dotnet run

# Terminal 3 - LogService (5003)
cd src/Services/LogService/LogService.API
dotnet run

# Terminal 4 - ApiGateway (5000)
cd src/ApiGateway
dotnet run
```

**Option B: Terminal'den All-in-One**

Projenin kökünden:

```powershell
# PowerShell
$jobs = @(
    { cd src/Services/AuthService/AuthService.API; dotnet run },
    { cd src/Services/ProductService/ProductService.API; dotnet run },
    { cd src/Services/LogService/LogService.API; dotnet run },
    { cd src/ApiGateway; dotnet run }
)

$jobs | ForEach-Object {
    Start-Job -ScriptBlock $_
}
```

### Step 9: Servislerin Çalıştığını Kontrol Et

Swagger UI'ları ziyaret et:

1. **AuthService Swagger**: http://localhost:5001/swagger
2. **ProductService Swagger**: http://localhost:5002/swagger
3. **LogService Swagger**: http://localhost:5003/swagger
4. **ApiGateway Swagger**: http://localhost:5000/swagger

Tüm sayfalar açılıyorsa ✅ **Başarılı!**

### Step 10: Seq Merkezi Loglama (Opsiyonel)

Logları merkezi olarak görmek için:

```bash
# Seq arayüzünü aç
http://localhost
```

## ⏱️ Hızlı Komutlar

```bash
# Tüm konteynerları başlat
docker-compose up -d

# Tüm konteynerları durdur
docker-compose down

# Belirli bir tervis başlat (AuthService)
cd src/Services/AuthService/AuthService.API && dotnet run

# Test et
curl http://localhost:5001/health

# Logları takip et
docker-compose logs -f

# Database'i sıfırla
dotnet ef database drop --force && dotnet ef database update
```

## 🐛 Sık Karşılaşılan Sorunlar

### ❌ "Port 5001 zaten kullanılmakta"

```bash
# Windows
netstat -ano | findstr :5001
taskkill /PID <PID> /F

# Linux/macOS
lsof -i :5001
kill -9 <PID>
```

### ❌ "SQL Server bağlantısı kurulamadı"

```bash
# Container'ın çalışıp çalışmadığını kontrol et
docker-compose ps

# Logs'u kontrol et
docker-compose logs sqlserver

# Container'ı yeniden başlat
docker-compose restart sqlserver
```

### ❌ "Ortam değişkenleri ayarlanmadı"

```bash
# Ayarlanan değişkenleri kontrol et
echo $Jwt__Secret  # Linux/macOS
echo %Jwt__Secret%  # Windows

# Yeniden set et
setx Jwt__Secret "your-value"  # Windows
export Jwt__Secret="your-value"  # Linux/macOS
```

### ❌ "NuGet paketleri yüklenmedi"

```bash
# Cache'i temizle
dotnet nuget locals all --clear

# Paketleri tekrar yükle
dotnet restore --no-cache
```

## 📚 Sonraki Adımlar

1. **Mimarı Anla**: [Detaylı Yapı](STRUCTURE.md) oku
2. **Konfigürasyonu Öğren**: [Konfigürasyon Özeti](CONFIGURATION_SUMMARY.md) oku
3. **Ortam Değişkenlerini Ayarla**: [Ortam Değişkenleri](ENVIRONMENT_VARIABLES.md) kılavuzunu oku
4. **İlk Endpoint'i Yaz**: AuthService'te bir controller yazıp test et
5. **Veritabanı Tasarımını Yapıl**: Domain entity'leri ve DbContext'i konfigure et

## 💡 Tips & Tricks

### Multi-Process İzleme

```bash
# PowerShell Console X
# Tüm dört servisi ayrı pencerede çalıştırmak daha etkili

# Terminal 1
cd src/Services/AuthService/AuthService.API
dotnet watch run

# Terminal 2
cd src/Services/ProductService/ProductService.API
dotnet watch run

# ... vs (dotnet watch otomatik reload sağlar)
```

### Serilog JSON Loglarını Gümeş Kodla

```bash
# Terminal
docker-compose logs seq

# veya
http://localhost (Seq UI)
```

### Health Check

```bash
# Tüm servislerin sağlığını kontrol et
curl http://localhost:5000/api/gateway/health  # ApiGateway
curl http://localhost:5001/api/auth/health     # AuthService
curl http://localhost:5002/api/product/health  # ProductService
curl http://localhost:5003/api/log/health      # LogService
```

---

**Sorular mı?** README.md veya ENVIRONMENT_VARIABLES.md dosyaları kontrol et.
