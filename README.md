# 📊 ClientVault - Enterprise Customer Management System

## Project Overview

A comprehensive ASP.NET Core MVC application for managing customer data with authentication, authorization, reporting, analytics, and administrative controls.

**Built over 5 Days of Internship**

---

## 🚀 Features

### 🔐 Authentication & Authorization
- User Registration and Login
- 3 User Roles: Admin, Manager, User
- Role-Based Access Control
- Account Lockout after 5 failed attempts
- Secure Password Hashing
- Session Management (14-day cookie)

### 📊 Dashboard
- Executive Dashboard with 8 KPI Cards
- Interactive Charts (Pie, Doughnut, Bar)
- Quick Search
- Quick Actions Shortcuts
- Recent Activity Feed

### 👥 Customer Management
- Customer List with Search, Filter, Sort
- Pagination
- Customer Details Page
- Customer Profile Page
- Print Profile Functionality

### 📄 Reports & Export
- 3 Report Types: Customer Summary, Activity Log, User Report
- Export to Excel
- Export to PDF
- Print-Friendly Layout

### 👑 Administration
- Admin Dashboard
- User Management (View, Activate, Deactivate)
- Role Management (Create, Delete)
- Activity Logs (Audit Trail)
- System Settings (Company Profile, Display Options)

### 🛡️ Security Features
- ASP.NET Core Identity
- Role-Based Access Control
- Password Hashing
- Anti-Forgery Tokens
- Global Exception Handling
- API Fallback Data

### ⚡ Performance Optimization
- Response Caching
- Data Compression
- Lazy Loading Charts
- Global Loading Overlay

---

## 🛠️ Technology Stack

| Component | Technology |
|-----------|------------|
| Backend | ASP.NET Core MVC (.NET 8.0) |
| Frontend | Razor Views, HTML5, CSS3 |
| CSS Framework | Bootstrap 5 |
| Icons | Bootstrap Icons |
| Charts | Chart.js |
| API Client | HttpClient + Newtonsoft.Json |
| Database | Entity Framework Core (In-Memory) |
| PDF Generation | iTextSharp |
| Excel Generation | ClosedXML |
| Version Control | Git + GitHub |

---

## 📁 Project Structure
Project1/
├── Controllers/
│ ├── AccountController.cs # Login, Register, Logout
│ ├── AdminController.cs # Admin Dashboard, User/Role Management
│ ├── AnalyticsController.cs # Customer Analytics
│ ├── DashboardController.cs # Executive Dashboard
│ ├── ExportController.cs # Export to Excel/PDF
│ ├── HomeController.cs # Customer List, Details, Profile
│ ├── NotificationController.cs # User Notifications
│ ├── ReportController.cs # Reports Generation
│ └── SettingsController.cs # System Settings
├── Models/
│ ├── ActivityLog.cs # Audit Trail
│ ├── ApplicationUser.cs # Identity User
│ ├── Consignee.cs # Customer Model
│ ├── Notification.cs # User Notifications
│ └── SystemSettings.cs # App Configuration
├── Services/
│ ├── ActivityLogService.cs # Logging Service
│ ├── ConsigneeService.cs # API Service
│ ├── NotificationService.cs # Notification Service
│ ├── PerformanceService.cs # Performance Tracking
│ └── ValidationService.cs # Data Validation
├── Views/
│ ├── Account/
│ ├── Admin/
│ ├── Analytics/
│ ├── Dashboard/
│ ├── Export/
│ ├── Home/
│ ├── Notification/
│ ├── Report/
│ └── Settings/
├── ViewModels/
├── Data/
│ ├── ApplicationDbContext.cs
│ └── SeedData.cs
├── wwwroot/
├── Program.cs
├── appsettings.json
└── README.md

---

## 🎨 Color Palette

| Color Name | Hex Code | Usage |
|------------|----------|-------|
| Dark Background | `#0a0e1a` | Page background |
| Card Background | `#111827` | Cards |
| Blue | `#3b82f6` | Primary buttons, charts |
| Green | `#22c55e` | Active status |
| Red | `#ef4444` | Inactive status |
| Sage Green | `#8EB69B` | Companies |
| Light Blue | `#93c5fd` | Persons |

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [Git](https://git-scm.com/downloads)
- Modern web browser

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/R3D-GM/MyFirstNewProject.git
   cd Project1