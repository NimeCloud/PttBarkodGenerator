# PTT Barkod Üretici (PTT Barcode Generator)

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/download/dotnet/10.0)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

PTT kargo gönderileri için barkod numarası üreten bir **C# .NET 8** konsol uygulaması. Verilen 10 haneli başlangıç numarasından itibaren, belirtilen adet kadar ardışık barkod üretir ve her biri için **check digit** (kontrol basamağı) hesaplar.

## 📋 İçindekiler

- [Barkod Formatı](#-barkod-formatı)
- [Check Digit Hesaplama Algoritması](#-check-digit-hesaplama-algoritması)
- [Örnek Çıktılar](#-örnek-çıktılar)
- [Gereksinimler](#-gereksinimler)
- [Kurulum ve Çalıştırma](#-kurulum-ve-çalıştırma)
- [Yapılandırma](#-yapılandırma)
- [Proje Yapısı](#-proje-yapısı)
- [API Referansı](#-api-referansı)

---

## 📦 Barkod Formatı

```
RR + 10 hane veri + 1 hane check digit = 13 karakter
```

| Bileşen | Uzunluk | Açıklama |
|---------|---------|----------|
| `RR`    | 2       | Sabit ön ek (registered mail / kayıtlı gönderi) |
| `DATA`  | 10      | Barkodun sayısal veri kısmı |
| `CD`    | 1       | **Check digit** — algoritma ile hesaplanan kontrol basamağı |

**Örnek:** `RR0608576613` + `8` = `RR06085766138`

---

## 🔢 Check Digit Hesaplama Algoritması

PTT kargo barkodlarında kullanılan check digit, **ağırlıklı toplama** (weighted sum) yöntemiyle hesaplanır.

### Adım Adım Hesaplama

1. **10 haneli verinin** her basamağı sırayla **1** ve **3** çarpanlarıyla çarpılır:
   - 1. basamak (index 0) → ×**1**
   - 2. basamak (index 1) → ×**3**
   - 3. basamak (index 2) → ×**1**
   - 4. basamak (index 3) → ×**3**
   - ... (bu şekilde devam eder)

2. **Tüm çarpımlar toplanır.**

3. **Check digit** şu formülle bulunur:

```
checkDigit = (10 - (toplam % 10)) % 10
```

> **Not:** Eğer toplam 10'un tam katı ise (toplam % 10 == 0), check digit **0**'dır.

### Örnek: `0608576613`

| Basamak    | 0 | 6 | 0 | 8 | 5 | 7 | 6 | 6 | 1 | 3 |
|------------|---|---|---|---|---|---|---|---|---|---|
| Çarpan     | 1 | 3 | 1 | 3 | 1 | 3 | 1 | 3 | 1 | 3 |
| Çarpım     | 0 | 18 | 0 | 24 | 5 | 21 | 6 | 18 | 1 | 9 |

```
Toplam = 0 + 18 + 0 + 24 + 5 + 21 + 6 + 18 + 1 + 9 = 102
Check Digit = (10 - (102 % 10)) % 10 = (10 - 2) % 10 = 8
Sonuç: RR06085766138 ✅
```

### Diğer Doğrulamalar

| Veri (10 hane) | Toplam | Check Digit | Barkod | Durum |
|----------------|--------|-------------|--------|-------|
| `0608576613` | 102 | **8** | `RR06085766138` | ✅ |
| `0608576614` | 105 | **5** | `RR06085766145` | ✅ |
| `0608576615` | 108 | **2** | `RR06085766152` | ✅ |
| `0608576616` | 111 | **9** | `RR06085766169` | ✅ |

---

## 📝 Örnek Çıktılar

```
==================================================
  PTT Barkod Üretici
==================================================
  Ön Ek:     RR
  Başlangıç: 0608576613
  Adet:      4
==================================================

  RR06085766138
  RR06085766145
  RR06085766152
  RR06085766169

==================================================
  Tamamlandı - 4 barkod üretildi.
==================================================
```

---

## ⚙️ Gereksinimler

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (veya üzeri)
- Windows, Linux veya macOS

---

## 🚀 Kurulum ve Çalıştırma

```bash
# 1. Repoyu klonlayın
git clone https://github.com/NimeCloud/PttBarkodGenerator.git
cd PttBarkodGenerator

# 2. Projeyi çalıştırın (otomatik derleme + çalıştırma)
dotnet run

# Sadece derlemek için:
dotnet build

# Derlenmiş hali çalıştırmak için (derleme sonrası):
dotnet run --no-build
```

---

## 🛠 Yapılandırma

Tüm ayarlar [`Program.cs`](Program.cs:8) dosyasında **hardcoded** olarak tanımlanmıştır. Kendi değerlerinizi girmek için bu dosyayı düzenleyin:

```csharp
var config = new BarcodeConfig
{
    Prefix = "RR",              // Barkod ön eki (isteğe bağlı değiştirilebilir)
    StartData = "0608576613",   // Başlangıç verisi (10 hane, sadece rakam)
    Quantity = 10               // Üretilecek barkod sayısı
};
```

| Parametre | Tip | Varsayılan | Açıklama |
|-----------|-----|-----------|----------|
| `Prefix` | `string` | `"RR"` | Barkod başına eklenecek ön ek |
| `StartData` | `string` | `"0608576613"` | 10 haneli başlangıç verisi (sadece rakam) |
| `Quantity` | `int` | `10` | Üretilecek ardışık barkod sayısı |

---

## 📁 Proje Yapısı

```
PttBarkodGenerator/
├── Program.cs                  # Ana giriş noktası (yapılandırma burada)
├── PttBarkodGenerator.csproj   # .NET 8 proje dosyası
├── Models/
│   └── BarcodeConfig.cs        # Yapılandırma modeli + validasyon
├── Services/
│   └── BarcodeGenerator.cs     # Check digit hesaplama & barkod üretme
├── plans/
│   └── ptt-barcode-generator-plan.md  # Mimari plan dökümanı
├── .gitignore
└── README.md
```

---

## 📚 API Referansı

### `BarcodeConfig` (Model)

| Metot | Açıklama |
|-------|----------|
| `Validate(out string errorMessage)` | Yapılandırmanın geçerli olup olmadığını kontrol eder |

### `BarcodeGenerator` (Servis)

| Metot | Parametreler | Dönüş | Açıklama |
|-------|--------------|-------|----------|
| `ValidateData(data, out errorMessage)` | `string data` | `bool` | 10 haneli verinin geçerliliğini kontrol eder |
| `CalculateCheckDigit(data)` | `string data` | `int` | 10 haneli veri için check digit hesaplar (0-9) |
| `GenerateSingle(data)` | `string data` | `string` | Tek bir barkod üretir (`RR` + veri + check digit) |
| `GenerateBatch()` | — | `List<string>` | Yapılandırmadaki ayarlara göre toplu üretim yapar |
| `GenerateBatch(startData, quantity)` | `string, int` | `List<string>` | Parametre ile toplu üretim yapar |

---

## 📄 Lisans

Bu proje MIT lisansı ile lisanslanmıştır. Detaylar için [`LICENSE`](LICENSE) dosyasına bakınız.
