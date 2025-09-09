# Transaction w Auth

This project is a **Fullstack Application** with the backend built using **.NET Core (C#)** and the frontend using **React + Vite + TailwindCSS**. The system is designed to manage products, orders, and user authentication, with database integration through **Entity Framework Core**.

---

## 📂 Repository Links
- **Frontend : [`https://netcore-app-samz.vercel.app`](https://netcore-app-samz.vercel.app/)**
- **Backend : [`http://samz.runasp.net/swagger/index.html`](http://samz.runasp.net/swagger/index.html)**

---

## 🚀 Tech Stack

### Backend
- **ASP.NET Core Web API 9**
- **Entity Framework Core (EF Core)**
- **SQL Server**
- **JWT Authentication**
- **EF Core Migrations**

### Frontend
- **React 19 + Vite**
- **TailwindCSS**
- **React Router**
- **Fetch API / Axios**

---

## 🏗️ Business Process Flow

1. **Authentication & Registration**
   - User registers → data is sent to the `AuthController` API.
   - Backend saves the user in the database (with hashed password).
   - On login, the backend returns a **JWT Token**.

2. **Product Management**
   - Admin can add/edit/delete products (`ProductsController`).
   - Users can view the product list via the frontend.

3. **Order Management**
   - User places an order → handled by the `OrdersController` API.
   - Backend stores order details (products, quantity, shipment).

---

## ⚙️ How to Run

### Backend
```sh
cd backend
dotnet restore
dotnet ef database update
dotnet run
```

### Frontend
```sh
cd frontend
npm install
npm run dev
```
