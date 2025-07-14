# Personal Finance Tracker – Enterprise Edition

## Loyiha tavsifi
Personal Finance Tracker – Enterprise Edition — bu foydalanuvchilarga o‘z moliyaviy harajat va daromadlarini boshqarish, statistik tahlil qilish va monitoring qilish imkonini beruvchi zamonaviy RESTful API. Loyiha real production muhitiga yaqin, xavfsiz, kengaytiriladigan va container texnologiyalari bilan to‘liq integratsiyalashgan.

## Asosiy imkoniyatlar
- Foydalanuvchi ro‘yxatdan o‘tish va login (JWT + refresh token)
- RBAC: admin va oddiy foydalanuvchi rollari
- Transaction va Category uchun CRUD, filter, sort, pagination
- Soft delete va optimistic concurrency (RowVersion)
- Audit log: har bir CRUD amalini loglash
- Statistika va summary endpointlar (oylik balans, eng ko‘p xarajat, trend)
- Kesh (MemoryCache/Redis) yordamida tezkor javob
- Serilog yordamida structured logging
- Health check endpoint
- Docker Compose: API, PostgreSQL, Redis
- Swagger UI orqali barcha endpointlar hujjatlari
- Unit va integration testlar (xUnit, Moq)

## Texnologiyalar
- C# (.NET 7+)
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- Redis
- Serilog
- Docker, Docker Compose
- xUnit, Moq
- Swagger (Swashbuckle)

## O‘rnatish va ishga tushirish

1. **Kodni klonlash:**
   ```bash
   git clone <repo-url>
   cd PersonalFinanceTracker–EnterpriseEdition
   ```

2. **Docker Compose orqali ishga tushirish:**
   ```bash
   docker-compose up --build
   ```
   - API: http://localhost:5000
   - Swagger UI: http://localhost:5000/swagger
   - Health check: http://localhost:5000/health

3. **Ma’lumotlar bazasi va Redis avtomatik ishga tushadi.**

4. **Swagger UI orqali barcha endpointlarni test qilish va JWT token bilan autentifikatsiya qilish mumkin.**

5. **Testlarni ishga tushirish:**
   ```bash
   dotnet test
   ```

## Loyiha tuzilmasi
- `src/PersonalFinanceTracker_EnterpriseEdition.Api` — API va controllerlar
- `src/PersonalFinanceTracker_EnterpriseEdition.Application` — servislar, DTO, interfeyslar
- `src/PersonalFinanceTracker_EnterpriseEdition.Domain` — domen modellar
- `src/PersonalFinanceTracker_EnterpriseEdition.Infrastructure` — repository, servis implementatsiyalari
- `src/PersonalFinanceTracker_EnterpriseEdition.Tests` — unit va integration testlar

## Muallif
- [Bekmurod] — 2025# PersonalFinanceTracker_EnterpriseEdition
