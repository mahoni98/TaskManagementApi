# Görev Yönetim API'si (Task Management REST API)

Bu proje, kullanıcıların görev (task) oluşturabildiği, görevleri listeleyebildiği, güncelleyebildiği ve silebildiği basit bir görev yönetim sistemidir. API, temel CRUD operasyonlarını sağlamakta, JWT ile kimlik doğrulaması kullanmakta ve temiz, anlaşılır bir katmanlı mimariye (Controller-Service-Repository) sahiptir.

##  Teknolojiler

* **Dil ve Çerçeve:** C# (.NET 8) & ASP.NET Core Web API
* **ORM:** Entity Framework Core
* **Veritabanı:** SQL Server
* **Kimlik Doğrulama:** JWT (JSON Web Token)
* **API Belgelendirme:** Swagger (Swashbuckle)

## ✨ Özellikler

* **Kullanıcı Kimlik Doğrulama:** JWT tabanlı güvenli kullanıcı girişi ve kaydı.
* **Görev Yönetimi:** Kullanıcılara özel görev oluşturma, listeleme, güncelleme ve silme.
* **Kullanıcıya Özel Erişim:** Her kullanıcı yalnızca kendi oluşturduğu görevleri görüntüleyebilir ve yönetebilir.
* **RESTful Uç Noktalar:** API, RESTful prensiplere uygun olarak tasarlanmıştır.
* **Katmanlı Mimari:** Sorumlulukların Controller, Service ve Repository katmanları arasında ayrılmasıyla temiz ve sürdürülebilir kod yapısı.


## Kurulum ve Çalıştırma

Projeyi lokalinizde çalıştırabilmek için aşağıdaki adımları takip edin:

### Ön Koşullar

* [.NET SDK 8.0 veya üstü](https://dotnet.microsoft.com/download/dotnet/8.0)
* [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (SQL Server Express sürümü yeterlidir)
* (İsteğe Bağlı) [SQL Server Management Studio (SSMS)](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms) veya benzeri bir veritabanı yönetim aracı.

### Adımlar

1.  **Projeyi Klonlayın:**
    ```bash
    git clone <proje-depo-url>
    cd <proje-klasoru> # Örneğin: cd GorevYonetimAPI
    ```

2.  **Veritabanı Bağlantı Dizgesini Ayarlayın:**
    * `appsettings.json` dosyasını açın.
    * `"ConnectionStrings"` altındaki `"DefaultConnection"` değerini kendi yerel SQL Server kurulumunuza göre güncelleyin.
        * `Data Source=` kısmına kendi SQL Server sunucu adınızı yazın (örneğin: `(localdb)\\MSSQLLocalDB` veya `localhost\\SQLEXPRESS`).
        * `Initial Catalog=EduDb;` kısmındaki `EduDb` veritabanı adıdır, dilerseniz değiştirebilirsiniz.

    ```json
    // appsettings.json örneği
    {
      "ConnectionStrings": {
        "DefaultConnection": "Data Source=YOUR_SERVER_NAME;Initial Catalog=EduDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"
      },
      // ...
    }
    ```

3.  **Veritabanı Migrasyonlarını Uygulayın:**
    * Proje klasörünün kök dizininde (yani `.sln` dosyasının olduğu yerde) bir terminal veya komut istemcisi açın.
    * Aşağıdaki komutu çalıştırarak veritabanını oluşturun ve migrasyonları uygulayın:
        ```bash
        dotnet ef database update
        ```
        *Bu komut, veritabanını oluşturacak ve (`ApplicationDBContext.cs` içindeki `OnModelCreating` ile) "Admin" ve "User" rollerini ekleyecektir.*

4.  **Projeyi Çalıştırın:**
    ```bash
    dotnet run
    ```
    * Proje başarıyla başladığında, API'ye erişim portunu (`https://localhost:<port>`) terminalde göreceksiniz. Genellikle `5001` veya `7XXX` gibi bir port numarası olur.

## 🧪 API Kullanımı (Swagger UI)

Proje çalıştığında, API endpoint'lerini test etmek için otomatik olarak oluşturulan Swagger UI'a erişebilirsiniz:

* Tarayıcınızda şu adresi açın: `https://localhost:<API_PORT>/swagger` (Örneğin: `https://localhost:5001/swagger`)

### Kullanım Senaryosu: Görev Oluşturma ve Yönetme

1.  **Kullanıcı Kaydı (`/api/account/register` - POST):**
    * Swagger UI'da `Account` bölümünü genişletin.
    * `/api/account/register` POST endpoint'ini bulun ve "Try it out" butonuna tıklayın.
    * `username`, `email` ve `password` bilgilerini içeren bir istek gövdesi girin (örn: `{"userName": "testuser", "email": "test@example.com", "password": "Password123!"}`).
    * "Execute" butonuna tıklayın. Başarılı bir yanıtta, bir JWT `token` alacaksınız. Bu token'ı kopyalayın.

2.  **Giriş (`/api/account/login` - POST) - *Zaten kayıtlıysanız bu adımı atlayabilirsiniz.***
    * Eğer zaten kayıtlı bir kullanıcınız varsa veya token süresi dolmuşsa, bu endpoint ile giriş yapıp yeni bir token alabilirsiniz.
    * Kayıt olduğunuz `username` ve `password` ile giriş yapın ve yeni `token`'ı kopyalayın.

3.  **API'ye Yetkilendirme (Swagger UI):**
    * Swagger UI'ın sağ üst tarafındaki **"Authorize"** butonuna tıklayın.
    * Açılan pencerede `Bearer` seçeneğini seçin.
    * `Value` alanına, kopyaladığınız JWT token'ı `Bearer ` önekiyle birlikte yapıştırın (örnek: `Bearer eyJhbGciOi...`).
    * "Authorize" butonuna tıklayın ve ardından pencereyi kapatın.

4.  **Görev Oluşturma (`/api/task` - POST):**
    * Swagger UI'da `Task` bölümünü genişletin.
    * `/api/task` POST endpoint'ini bulun ve "Try it out" butonuna tıklayın.
    * `title`, `description`, `isCompleted` ve `dueDate` bilgilerini içeren bir istek gövdesi girin.
    * "Execute" butonuna tıklayın. Başarılı bir yanıt (200 OK) ile yeni oluşturulan görevin DTO'sunu göreceksiniz.

5.  **Görevleri Listeleme (`/api/task` - GET):**
    * `/api/task` GET endpoint'ini bulun.
    * "Try it out" ve "Execute" butonlarına tıklayarak mevcut kullanıcının tüm görevlerini listeleyebilirsiniz. (İsteğe bağlı olarak `title`, `sortBy` gibi sorgu parametrelerini kullanabilirsiniz.)

6.  **Görev Güncelleme, Silme ve Detay Görüntüleme:**
    * Diğer `TasksController` endpoint'lerini (`GET /{id}`, `PUT /{id}`, `DELETE /{id}`) kullanarak görevleri ID'ye göre görüntüleyebilir, güncelleyebilir veya silebilirsiniz. **Unutmayın, sadece kendi görevlerinizi yönetebilirsiniz.**

---