# CarService

Avtomobil ta'mirlash ustaxonasi va ehtiyot qismlar do'koni uchun veb-sayt.

- **Backend:** ASP.NET Core Web API (.NET 10, Controllers), Clean Architecture, EF Core, PostgreSQL
- **Frontend:** React, TypeScript, Vite, Tailwind CSS, shadcn/ui, TanStack Query, React Router, i18next (uz/ru)

## Tuzilma

```
src/
  CarService.Domain/          entitylar, value objectlar, enumlar (paketsiz)
  CarService.Application/     MediatR command/query, validatorlar, interfeyslar
  CarService.Infrastructure/  AppDbContext, migratsiyalar, Identity + JWT
  CarService.Api/             controllerlar, middleware, Program.cs
tests/
  CarService.Domain.Tests/
  CarService.Application.Tests/
client/                       React ilova
```

Bog'liqlik: `Api → Application, Infrastructure`, `Infrastructure → Application`, `Application → Domain`.

## Ishga tushirish

### Docker bilan (hammasi birga)

```bash
cp .env.example .env        # JWT_KEY ni o'zgartiring
docker compose up --build
```

- Sayt: http://localhost:3000
- API: http://localhost:8080 (`/health`)

### Lokal ishlab chiqish

Talablar: .NET 10 SDK, Node.js 22, PostgreSQL 16.

```bash
# PostgreSQL'ni Docker'da ko'tarish
docker compose up -d db

# Backend (Development rejimida migratsiyalar avtomatik qo'llanadi)
dotnet run --project src/CarService.Api --launch-profile http
# Swagger: http://localhost:5262/swagger

# Frontend
cd client
npm install
npm run dev
# http://localhost:5173 (/api so'rovlari backend'ga proxy qilinadi)
```

### Migratsiya qo'shish

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add <Nomi> -p src/CarService.Infrastructure -s src/CarService.Api -o Persistence/Migrations
```

## Testlar

```bash
dotnet test
cd client && npm run lint && npm run build
```

## API (hozircha)

| Metod | Yo'l | Tavsif |
| --- | --- | --- |
| POST | `/api/auth/register` | Telefon raqam, ism va parol bilan ro'yxatdan o'tish |
| POST | `/api/auth/login` | Kirish, JWT qaytaradi |
| GET | `/api/auth/me` | Joriy foydalanuvchi (token kerak) |
| GET | `/api/services` | Xizmatlar va narx oralig'i |
| GET | `/api/catalog/brands` | Markalar va ularning modellari |
| GET | `/api/catalog/categories` | Qism kategoriyalari (daraxt, `parentId` bilan) |
| GET | `/api/catalog/vin/{vin}` | VIN'dan marka va model yilini aniqlash |
| GET | `/api/parts` | Qism qidirish: `brandId`, `carModelId`, `year`, `categoryId`, `search`, `page`, `pageSize` (≤ 100) |
| GET | `/api/parts/{id}` | Qism ma'lumoti va qaysi mashinalarga mosligi |
| POST | `/api/parts/import` | CSV import, `file` maydoni (faqat Admin) |
| GET | `/health` | Holat tekshiruvi |

### Qismlarni CSV'dan import qilish

Excel'da jadvalni "Saqlash → CSV" orqali saqlang (ajratuvchi `;` yoki `,` bo'lishi mumkin). Namuna: [`docs/parts-import-sample.csv`](docs/parts-import-sample.csv).

- Majburiy ustunlar: `Sku`, `Name`, `Category`, `Manufacturer`, `Price`. Qo'shimcha: `IsOriginal` (ha/yo'q), `WarrantyMonths`, `StockQuantity`, `ImageUrl`, `Compatibility`.
- `Compatibility`: `Marka:Model:2013-2023`, bir nechtasi `|` bilan; oxirgi yil bo'sh bo'lsa (`2013-`) hozirgacha.
- Bir xil `Sku` qayta yuklansa, qism yangilanadi. Yangi kategoriya, marka va modellar avtomatik yaratiladi.
- Xato qatorlar o'tkazib yuboriladi va javobdagi `errors` ro'yxatida qator raqami bilan ko'rsatiladi.

Development rejimida admin foydalanuvchi `appsettings.Development.json` dagi `Seed:Admin` sozlamasidan yaratiladi. Bo'sh bazaga namunaviy katalog (Chevrolet, Kia, Hyundai modellari va 13 ta qism) qo'shiladi.
