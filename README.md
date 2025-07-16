# Personal Finance Tracker Enterprise Edition

A comprehensive personal finance management system built with .NET 9.0, featuring enterprise-grade architecture, real-time analytics, and secure transaction management.

## 🚀 Features

### Core Functionality

- **Transaction Management**: Create, update, delete, and categorize financial transactions
- **Category Management**: Organize transactions with customizable categories and colors
- **User Authentication**: Secure JWT-based authentication and authorization
- **Real-time Analytics**: Monthly summaries, expense trends, and category statistics
- **Data Export**: Excel export functionality for financial reports
- **Audit Logging**: Complete audit trail for all data modifications

### Enterprise Features

- **Layered Architecture**: Clean separation of concerns with Domain, Application, Infrastructure, and API layers
- **Caching**: Redis-based caching for improved performance
- **Background Services**: Automated Excel export processing
- **Health Checks**: Application health monitoring
- **Structured Logging**: Serilog integration with file rotation and retention
- **Docker Support**: Containerized deployment with Docker Compose

## 🛠️ Technology Stack

- **.NET 9.0**: Latest .NET framework
- **Entity Framework Core**: ORM for data access
- **Postgresql**: Primary database
- **Redis**: Caching layer
- **JWT**: Authentication
- **Serilog**: Structured logging
- **xUnit**: Unit testing framework
- **Docker**: Containerization

## 📋 Prerequisites

- .NET 9.0 SDK
- Postgresql (Local or Docker)
- Redis (Local or Docker)
- Docker & Docker Compose (for containerized deployment)

## 🚀 O'rnatish va Ishlatish

1. **Loyihani yuklab olish**

   ```bash
   git clone https://github.com/bekmurod21/PersonalFinanceTracker_EnterpriseEdition.git
   cd PersonalFinanceTracker_EnterpriseEdition
   ```

2. **Dasturni ishga tushirish**

   ```bash
   docker-compose up -d
   ```

4. **Brauzerda ochish**
   - API: http://localhost:5000
   - Swagger UI (API dokumentatsiyasi): http://localhost:5000/swagger

## 📚 API Dokumentatsiyasi

### 🔐 Autentifikatsiya

#### Foydalanuvchi ro'yxatdan o'tish
```http
POST /api/auth/signup
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "StrongPassword123!",
  "firstName": "John",
  "lastName": "Doe"
}
```

**Javob:**
```json
{
  "success": true,
  "message": "User registered successfully",
  "data": {
    "id": "user-guid",
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe"
  }
}
```

#### Tizimga kirish

```http
POST /api/auth/signin
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "StrongPassword123!"
}
```

**Javob:**

```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "user": {
      "id": "user-guid",
      "email": "user@example.com",
      "firstName": "John",
      "lastName": "Doe"
    }
  }
}
```

#### Joriy foydalanuvchi ma'lumotlari

```http
GET /api/auth/me
Authorization: Bearer your-jwt-token
```

### 💰 Tranzaksiyalar

#### Barcha tranzaksiyalarni olish

```http
GET /api/transactions?page=1&pageSize=10&sort=createdAt&filter=salary
Authorization: Bearer your-jwt-token
```

**Parametrlar:**

- `page` - Sahifa raqami (default: 1)
- `pageSize` - Sahifadagi elementlar soni (default: 10)
- `sort` - Tartiblash (amount, createdAt)
- `filter` - Qidiruv so'zi

#### Yangi tranzaksiya yaratish

```http
POST /api/transactions
Authorization: Bearer your-jwt-token
Content-Type: application/json

{
  "amount": 1000.50,
  "type": "Income",
  "categoryId": "category-guid",
  "note": "Oylik maosh"
}
```

#### Tranzaksiyani yangilash

```http
PUT /api/transactions/{id}
Authorization: Bearer your-jwt-token
Content-Type: application/json

{
  "amount": 1200.00,
  "type": "Income",
  "categoryId": "category-guid",
  "note": "Oylik maosh + bonus",
  "rowVersion": "base64-encoded-version"
}
```

#### Tranzaksiyani o'chirish

```http
DELETE /api/transactions/{id}
Authorization: Bearer your-jwt-token
```

### 📂 Kategoriyalar

#### Barcha kategoriyalarni olish

```http
GET /api/categories
Authorization: Bearer your-jwt-token
```

#### Yangi kategoriya yaratish

```http
POST /api/categories
Authorization: Bearer your-jwt-token
Content-Type: application/json

{
  "name": "Oziq-ovqat",
  "color": "#FF5733"
}
```

#### Kategoriyani yangilash

```http
PUT /api/categories/{id}
Authorization: Bearer your-jwt-token
Content-Type: application/json

{
  "name": "Oziq-ovqat va ichimliklar",
  "color": "#FF5733"
}
```

### 📊 Statistika

#### Oylik hisobot

```http
GET /api/statistics/monthly-summary?year=2024&month=7
Authorization: Bearer your-jwt-token
```

**Javob:**

```json
{
  "success": true,
  "data": {
    "totalIncome": 5000.0,
    "totalExpense": 3200.5,
    "balance": 1799.5
  }
}
```

#### Oylik trend

```http
GET /api/statistics/monthly-trend?monthsCount=6
Authorization: Bearer your-jwt-token
```

**Javob:**

```json
{
  "success": true,
  "data": [
    {
      "year": 2024,
      "month": 2,
      "income": 4500.0,
      "expense": 2800.0
    },
    {
      "year": 2024,
      "month": 3,
      "income": 5000.0,
      "expense": 3200.5
    }
  ]
}
```

#### Kategoriya bo'yicha xarajatlar

```http
GET /api/statistics/category-expenses?year=2024&month=7&top=5
Authorization: Bearer your-jwt-token
```

**Javob:**

```json
{
  "success": true,
  "data": [
    {
      "id": "category-guid",
      "name": "Oziq-ovqat",
      "totalExpense": 800.0
    },
    {
      "id": "category-guid-2",
      "name": "Transport",
      "totalExpense": 500.0
    }
  ]
}
```