# 🍰 Sweetfy API (Backend)

**Sweetfy API** is the backend REST API for **Sweetfy**, a bakery management system.  
It manages **costs**, **inventory (ingredients and services)**, **recipes**, **final products**, and **orders**, automatically calculating base costs and profit margins.

Built with **ASP.NET Core 8**, following a **3-tier architecture (Controllers → Services → Repositories)** to ensure scalability, security, and clean separation of concerns.

---

## 🚀 Features

### 🔐 Authentication & Roles
- JWT Authentication (Register, Login, Refresh Token)
- Role-based authorization using `[Authorize(Roles = "Admin")]`
- Admin-only endpoints for role creation and assignment

### 🧾 Cost Management
- Full CRUD for **Ingredients** and **Services (labor)**
- Simple cost tracking per bakery

### 🧑‍🍳 Recipe Management (Composite)
- Full CRUD for **Recipes**
- Recipes composed of multiple ingredients and services
- Costs are stored as snapshots at creation or update time

### 🎂 Product Management (Super-Composite)
- Full CRUD for **Products**
- Products can include ingredients, services, or recipes
- `BaseCost` is auto-calculated by the server
- `ProfitAmount` and `ProfitPercent` are generated from `SalePrice`

### 🧺 Order Management
- Full CRUD for **Orders** (or Quotes)
- Orders include products and recipes
- Total cost and total price are auto-calculated

### 🛡️ Security
- All endpoints (except Auth) are protected
- The Service Layer ensures users only access data from their own bakery (`BakeryId` filtering)

### 📘 Documentation
- Fully documented with **XML comments**
- Integrated with **Swagger (OpenAPI)**

---

## 🏗️ Architecture Overview

📂 SweetfyAPI
┣ 📁 Controllers → Handle HTTP requests & DTO validation
┣ 📁 Services → Business logic & cost/security validation
┣ 📁 Repositories → Data access using EF Core
┣ 📁 Models → Entities & DTOs
┣ 📁 Mappings → AutoMapper profiles
┗ 📁 wwwroot → Static files (if any)


---

## ⚙️ Tech Stack

- **.NET 8**
- **ASP.NET Core Web API**
- **Entity Framework Core 8**
- **SQL Server**
- **ASP.NET Identity** (user & role management)
- **JWT Authentication** (Access + Refresh Tokens)
- **AutoMapper**
- **Swagger (OpenAPI)**

---

## 🧩 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- A SQL Server instance (LocalDB, Express, or full)
- EF Core CLI tool:

```bash
dotnet tool install --global dotnet-ef

1️⃣ Clone the Repository
git clone [YOUR_REPOSITORY_URL]
cd SweetfyAPI

2️⃣ Configure User Secrets

This project uses User Secrets to securely store sensitive data (connection string and JWT key).

Initialize secrets:
dotnet user-secrets init

Add Connection String:
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=Your_SQL_Instance;Database=Your_DB_Name;Trusted_Connection=True;TrustServerCertificate=True;"

Add JWT Settings:
dotnet user-secrets set "JWT:ValidIssuer" "https://localhost:7286"
dotnet user-secrets set "JWT:ValidAudience" "https://localhost:7286"
dotnet user-secrets set "JWT:TokenValidityInMinutes" "30"
dotnet user-secrets set "JWT:RefreshTokenValidityInMinutes" "60"
dotnet user-secrets set "JWT:SecretKey" "YOUR_VERY_LONG_AND_SECURE_SECRET_KEY_HERE_12345!"

3️⃣ Apply Database Migrations
dotnet ef migrations add InitialCreate
dotnet ef database update

4️⃣ Run the Application
Run via Visual Studio (HTTPS profile) or terminal:
dotnet run

API available at:
➡️ https://localhost:7286

Testing with Swagger

1) Run the app and open:
https://localhost:7286/swagger

2) Register a new user:
POST /api/auth/register

3) Log in:
POST /api/auth/login

4) Copy your accessToken

5) Click Authorize in Swagger

6) Paste your token as:
[YOUR_TOKEN_HERE]

7) You can now access protected endpoints (Ingredients, Recipes, Products, Orders, etc.)
