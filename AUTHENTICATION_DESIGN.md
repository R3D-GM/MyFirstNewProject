# 🔐 Authentication & Authorization Design Document

## 1. Project Overview
**Project:** Customer Management System  
**Feature:** User Authentication & Role-Based Access Control  
**Technology:** ASP.NET Core Identity  

---

## 2. User Roles & Permissions

### 2.1 Admin (Full Access)
- ✅ View Dashboard
- ✅ View Customer List
- ✅ View Customer Details
- ✅ Add Customers
- ✅ Edit Customers
- ✅ Delete Customers
- ✅ Manage Users
- ✅ Manage Roles

### 2.2 Manager (Customer Management)
- ✅ View Dashboard
- ✅ View Customer List
- ✅ View Customer Details
- ✅ Add Customers
- ✅ Edit Customers
- ❌ Delete Customers
- ❌ Manage Users
- ❌ Manage Roles

### 2.3 User (Read Only)
- ✅ View Dashboard
- ✅ View Customer List
- ✅ View Customer Details
- ❌ Add Customers
- ❌ Edit Customers
- ❌ Delete Customers
- ❌ Manage Users
- ❌ Manage Roles

---

## 3. Protected Pages

| Page | Route | Admin | Manager | User |
|------|-------|-------|---------|------|
| Dashboard | /Dashboard | ✅ | ✅ | ✅ |
| Customer List | / | ✅ | ✅ | ✅ |
| Customer Details | /Home/Details/{id} | ✅ | ✅ | ✅ |
| Add Customer | /Customers/Create | ✅ | ✅ | ❌ |
| Edit Customer | /Customers/Edit/{id} | ✅ | ✅ | ❌ |
| Delete Customer | /Customers/Delete/{id} | ✅ | ❌ | ❌ |
| User Management | /Admin/Users | ✅ | ❌ | ❌ |
| Role Management | /Admin/Roles | ✅ | ❌ | ❌ |
| Login | /Account/Login | Public | Public | Public |
| Register | /Account/Register | Public | Public | Public |
| Logout | /Account/Logout | ✅ | ✅ | ✅ |

---

## 4. Technology Stack

### 4.1 Authentication
- ASP.NET Core Identity
- Cookie Authentication
- ASP.NET Core Authorization

### 4.2 Database
- SQLite (Development)
- Entity Framework Core

### 4.3 Packages
```xml
Microsoft.AspNetCore.Identity.EntityFrameworkCore
Microsoft.AspNetCore.Identity.UI
Microsoft.EntityFrameworkCore.Sqlite
Microsoft.EntityFrameworkCore.Tools