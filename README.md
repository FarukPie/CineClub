# 🎬 IMDb Film Keşif & Blog Projesi

![.NET 8.0](https://img.shields.io/badge/.NET-8.0-purple)
![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET%20Core-MVC-blue)
![RapidAPI](https://img.shields.io/badge/API-RapidAPI-green)
![License](https://img.shields.io/badge/License-MIT-orange)

RapidAPI aracılığıyla IMDb verilerini çekerek **En İyi 250 Filmi** dinamik olarak listeleyen modern bir web uygulaması. **.NET 8.0** ve **ASP.NET Core MVC** ile geliştirilen bu proje; dış API tüketimi (API Consumption), sunucu taraflı render (SSR) ve dinamik içerik yönetimi yeteneklerini sergilemektedir.


## 🚀 Özellikler

* **IMDb Top 250:** IMDb'den alınan en yüksek puanlı filmleri dinamik olarak çeker ve listeler.
* **Gelişmiş Filtreleme:** Filmleri türlerine göre (Aksiyon, Drama, Bilim Kurgu vb.) filtreleme imkanı sunar.
* **Sayfalama (Pagination):** Büyük veri setlerini yönetmek için sayfalama yapısı entegre edilmiştir.
* **Film Asistanı (Chatbot):** Kullanıcılara film önerileri sunan basit bir chatbot entegrasyonu içerir.
* **Responsive Tasarım:** Bootstrap 5 ile her cihaza uyumlu (mobil/tablet/masaüstü) arayüz.
* **Modern Mimari:** MVC yapısı içerisinde N-Katmanlı (N-Layer) mantığına uygun kodlama.

## 🛠️ Kullanılan Teknolojiler

### Backend (Sunucu Tarafı)
* **Framework:** .NET 8.0 (ASP.NET Core MVC)
* **Dil:** C#
* **HTTP İstekleri:** `HttpClient` (Dış REST API'lere bağlanmak için).
* **Dependency Injection (DI):** ASP.NET Core'un yerleşik DI mekanizması.
* **Veri İşleme:** `Newtonsoft.Json` (JSON verilerini deserialize etmek için).

### Frontend (İstemci Tarafı)
* **View Engine:** Razor Views (.cshtml) - Server-Side Rendering için.
* **Stil:** Bootstrap 5 & Özel CSS.
* **Etkileşim:** jQuery & AJAX.
* **Validasyon:** jQuery Validation & Unobtrusive Validation.

### Dış Servisler
* **Veri Kaynağı:** [RapidAPI (IMDb Top 250 Movies)](https://rapidapi.com/rapihub-rapihub-default/api/imdb236)

## 🏗️ Mimari ve Çalışma Mantığı

1.  **İstek (Request):** Kullanıcı film listesi sayfasına gider.
2.  **Controller:** MVC Controller yapısı isteği karşılar ve Servis katmanını tetikler.
3.  **API Çağrısı:** `HttpClient`, RapidAPI uç noktasına (Endpoint) GET isteği atar.
4.  **İşleme:** Gelen JSON verisi `Newtonsoft.Json` ile C# nesnelerine (Models) dönüştürülür.
5.  **Render:** İşlenen veri Razor View'a gönderilir ve Bootstrap ile kullanıcıya sunulur.

## 📂 Proje Yapısı

├── Controllers/ # İstekleri karşılayan MVC Controller sınıfları ├── Models/ # ViewModel ve API Yanıt Modelleri ├── Views/ # Arayüz dosyaları (.cshtml) ├── Services/ # API Haberleşme mantığı (HttpClient) ├── wwwroot/ # Statik dosyalar (CSS, JS, Resimler) ├── Program.cs # Dependency Injection ve Middleware ayarları └── appsettings.json # API Konfigürasyonları



## 📄 Lisans

Bu proje MIT Lisansı ile dağıtılmaktadır. Daha fazla bilgi için `LICENSE` dosyasına bakın.

---

**Geliştirici: [Ömer Faruk Atak](https://www.linkedin.com/in/omer-faruk-atak-551025243/)**
