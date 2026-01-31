# E-Commerce Web Application (ASP.NET Core MVC)

A web-based **E-Commerce application** built using **ASP.NET Core MVC**.  
The project focuses on practicing **MVC pattern, Entity Framework Core, Identity, and CRUD-based business logic** in a real-looking web application.

---

## 🧩 What This Project Does

### 👤 Users & Identity
- User registration and login
- Email confirmation & OTP validation
- Password reset
- User profile management
- Role-based separation (Admin / Customer)

### 🛒 Customer Side
- Browse products
- View product details
- Shopping cart
- Place orders
- View order history and order details

### 🧑‍💼 Admin Panel
- Manage products
- Manage categories & brands
- Manage promotions
- Manage users
- View and manage orders

---

## 🛠️ Technical Stack

- **Framework:** ASP.NET Core MVC
- **Language:** C#
- **ORM:** Entity Framework Core
- **Database:** SQL Server
- **Authentication:** ASP.NET Core Identity
- **Frontend:** Razor Views, Bootstrap, jQuery
- **Architecture Style:** MVC + Service/Repository pattern

---

## 📁 Project Structure (High Level)

- `Areas`
  - `Admin` – Admin dashboard & management
  - `Customer` – Customer-facing pages
  - `Identity` – Authentication & account logic

- `Models` – Entity models
- `ViewModels` – Data shaping for views
- `Repositories` – Data access abstraction
- `Services` – Business logic layer
- `DataAccess` – DbContext
- `Migrations` – EF Core migrations
- `wwwroot` – Static files (CSS, JS, images)

---

## 🧠 Design Notes (Realistic)

- Uses **MVC pattern** (as intended by ASP.NET Core)
- Business logic is mostly handled in **Services**
- Database access goes through **Repositories**
- No strict Clean Architecture
- No CQRS / MediatR / DDD

This is a **classic ASP.NET Core MVC application**, not an over-engineered system.

---

## ▶️ How to Run

1. Open `ECommerce.sln` in Visual Studio
2. Update the connection string
3. Run `Update-Database`
4. Start the application

---

## 📌 Project Purpose

This project was built to:
- Practice real ASP.NET Core MVC development
- Understand Identity and authentication flows
- Work with EF Core and migrations
- Build a non-trivial CRUD-based web application

It is intended as a **learning and portfolio project**, not a production-ready system.

---

## 📝 Notes

- No automated tests
- No API layer (MVC only)
- Focus is on functionality and structure, not scalability

