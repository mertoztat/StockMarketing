# StockMarketing

ASP.NET Core 8 ile geliştirilmiş, hisse senedi kayıtlarını ve kullanıcı portföylerini yöneten bir Web API. API; hisse senedi verileri, yorumlar, kullanıcı kaydı/girişi ve JWT tabanlı kimlik doğrulama özellikleri sunar.

## Teknolojiler

- .NET 8 / ASP.NET Core Web API
- Entity Framework Core 8 ve SQL Server
- ASP.NET Core Identity
- JWT Bearer kimlik doğrulama
- Swagger / OpenAPI (yalnızca Development ortamında)

## Gereksinimler

- .NET 8 SDK
- SQL Server erişimi
- EF Core CLI (`dotnet-ef`):

```bash
dotnet tool install --global dotnet-ef
```

## Yapılandırma

Uygulama `ConnectionStrings:DefaultConnection` ve `JWT:SigningKey` ayarlarını bekler. Bağlantı dizesi ve imzalama anahtarı kaynak koda eklenmemelidir. Yerel geliştirme için User Secrets kullanın:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=StockMarketing;Trusted_Connection=True;TrustServerCertificate=True"
dotnet user-secrets set "JWT:SigningKey" "en-az-32-karakterlik-gizli-ve-rastgele-bir-anahtar"
```

`JWT:Issuer` ve `JWT:Audience` değerleri `appsettings.json` içinde `http://localhost:5246` olarak ayarlanmıştır; farklı bir adres kullanıyorsanız bu değerleri yapılandırmanız gerekir. Ortam değişkenleriyle ayarlamak için `ConnectionStrings__DefaultConnection`, `JWT__SigningKey`, `JWT__Issuer` ve `JWT__Audience` adlarını kullanabilirsiniz.

## Çalıştırma

Depoyu klonladıktan sonra proje dizininde:

```bash
dotnet restore
dotnet ef database update
dotnet run
```

Uygulamayı başlattıktan sonra terminalde gösterilen adresi kullanın. Development ortamında Swagger arayüzü `/swagger` adresindedir. Geliştirme ortamı değilse Swagger etkinleştirilmez.

## API uç noktaları

| Metot | Uç nokta | Açıklama |
| --- | --- | --- |
| `POST` | `/api/account/register` | Kullanıcı kaydı; başarılı yanıtta JWT döner. |
| `POST` | `/api/account/login` | Kullanıcı girişi; başarılı yanıtta JWT döner. |
| `GET` | `/api/stock` | Hisse senetlerini listeler. JWT gerekir. |
| `GET` | `/api/stock/{id}` | ID ile hisse senedi getirir. JWT gerekir. |
| `POST` | `/api/stock` | Hisse senedi oluşturur. |
| `PUT` | `/api/stock/{id}` | Hisse senedini günceller. |
| `DELETE` | `/api/stock/{id}` | Hisse senedini siler. |
| `GET` | `/api/portfolio` | Giriş yapan kullanıcının portföyünü listeler. JWT gerekir. |
| `POST` | `/api/portfolio?symbol={symbol}` | Hisseyi kullanıcının portföyüne ekler. JWT gerekir. |
| `DELETE` | `/api/portfolio?symbol={symbol}` | Hisseyi kullanıcının portföyünden çıkarır. JWT gerekir. |
| `GET` | `/api/comment` | Yorumları listeler. |
| `GET` | `/api/comment/{id}` | ID ile yorum getirir. |
| `POST` | `/api/comment/{stockId}` | Hisseye yorum ekler. |
| `PUT` | `/api/comment/{id}` | Yorumu günceller. |
| `DELETE` | `/api/comment/{id}` | Yorumu siler. |

Hisse listesi `Symbol`, `CompanyName`, `SortBy`, `isDescending`, `PageNumber` ve `PageSize` sorgu parametrelerini destekler. `SortBy` için `Symbol` veya `CompanyName` kullanılabilir; varsayılan sayfa numarası `1`, sayfa boyutu `10`'dur.

Kimlik doğrulaması gerektiren uç noktalara erişirken token'ı şu başlıkla gönderin:

```http
Authorization: Bearer <token>
```

Örnek kayıt isteği:

```json
{
  "username": "kullanici",
  "email": "kullanici@example.com",
  "password": "GucluSifre1"
}
```

Örnek hisse oluşturma isteği:

```json
{
  "symbol": "ACME",
  "companyName": "Acme Corporation",
  "purchase": 125.50,
  "lastDividend": 1.25,
  "industry": "Technology",
  "marketCap": 1000000000
}
```

## Veritabanı migrasyonları

Yeni bir migrasyon oluşturmak ve veritabanına uygulamak için:

```bash
dotnet ef migrations add <MigrationName>
dotnet ef database update
```
