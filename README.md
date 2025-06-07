# Infertility Treatment Management System

## 📋 Tổng quan Dự án

Hệ thống quản lý và theo dõi toàn diện quá trình điều trị hiếm muộn cho cơ sở y tế, được xây dựng với kiến trúc 4-layer sử dụng ASP.NET Core Web API và Entity Framework Core.

### 🌟 Mục tiêu

* Quản lý chu kỳ điều trị hiếm muộn (IUI, IVF)
* Theo dõi tiến trình điều trị của bệnh nhân
* Hệ thống đặt lịch hẹn thông minh
* Dashboard báo cáo và thống kê

### 👥 Đối tượng Sử dụng

* **Customer**: Bệnh nhân đăng ký điều trị
* **Doctor**: Bác sĩ điều trị
* **Manager**: Quản lý cơ sở y tế
* **Admin**: Quản trị hệ thống

---

## 🏗️ Kiến trúc Hệ thống

### **4-Layer Architecture**

```
📁 InfertilityTreatment.sln
🔗 InfertilityTreatment.API/          → Presentation Layer
🔗 InfertilityTreatment.Business/     → Business Logic Layer  
🔗 InfertilityTreatment.Data/         → Data Access Layer
🔗 InfertilityTreatment.Entity/       → Entity Layer
```

### **Dependencies Flow**

```
API → Business + Entity
Business → Data + Entity  
Data → Entity
Entity → No dependencies
```

---

## 🚀 Hướng dẫn Setup Dự án

### **📋 Yêu cầu Hệ thống**

* Visual Studio 2022
* .NET 8.0 SDK
* SQL Server (LocalDB / Express / Full)
* Git

### **🔧 Bước 1: Clone Repository**

```bash
git clone <repository-url>
cd InfertilityTreatment
```

### **🔧 Bước 2: Mở Solution**

* Mở file `InfertilityTreatment.sln` bằng Visual Studio

### **🔧 Bước 3: Restore NuGet Packages**

```bash
# Trong Visual Studio
Build → Restore NuGet Packages

# Hoặc bằng terminal
cd InfertilityTreatment.API
dotnet restore
```

### **🔧 Bước 4: Cấu hình Database**

#### **4.1. Connection String**

Mở `appsettings.json` trong project `InfertilityTreatment.API`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=InfertilityTreatmentDB;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

#### **4.2. Migration & Update Database**

Mở **Package Manager Console**:

```bash
# Set project mặc định
PM> Set-DefaultProject InfertilityTreatment.Data

# Tạo migration mới
PM> Add-Migration InitialCreate -StartupProject InfertilityTreatment.API

# Update database
PM> Update-Database -StartupProject InfertilityTreatment.API
```

---

## 🚨 Migrations Cheat Sheet

```bash
# Tạo migration mới
Add-Migration [MigrationName] -StartupProject InfertilityTreatment.API

# Cập nhật DB
Update-Database -StartupProject InfertilityTreatment.API

# Xóa DB (cẩn thận!)
Drop-Database -StartupProject InfertilityTreatment.API
```

---

## 🚀 Build & Run Project

1. Set `InfertilityTreatment.API` là **Startup Project**
2. Build Solution: `Ctrl + Shift + B`
3. Run app: `F5`

**Swagger UI:** `https://localhost:7178/swagger`
**Health Check:** `https://localhost:7178/health`

---

## 📦 Package Dependencies

### API Project

* `Microsoft.AspNetCore.Authentication.JwtBearer`
* `Microsoft.EntityFrameworkCore.Design`
* `Swashbuckle.AspNetCore`
* `Serilog.AspNetCore`
* `FluentValidation.AspNetCore`

### Business Project

* `FluentValidation`
* `BCrypt.Net-Next`
* `AutoMapper`
* `System.IdentityModel.Tokens.Jwt`

### Data Project

* `Microsoft.EntityFrameworkCore.SqlServer`
* `Microsoft.EntityFrameworkCore.Tools`
* `Microsoft.EntityFrameworkCore.Design`

### Entity Project

* `System.ComponentModel.Annotations`

---

## 📁 Database Entities

### **User Management**

* Users
* Customers
* Doctors
* RefreshTokens

### **Treatment Services**

* TreatmentServices
* TreatmentPackages
* TreatmentCycles

---

## 🔐 Authentication & Authorization

### JWT Settings (`appsettings.json`)

```json
"JwtSettings": {
  "SecretKey": "InfertilityTreatment-Super-Secret-Key-256-Bits-Long-For-JWT-Security-2024",
  "Issuer": "InfertilityTreatmentAPI",
  "Audience": "InfertilityTreatmentClient",
  "AccessTokenExpiryMinutes": 60,
  "RefreshTokenExpiryDays": 7
}
```

### Authorization Policies:

* `CustomerOnly`
* `DoctorOnly`
* `AdminOnly`

---

## 📅 API Endpoints (Swagger UI)

```http
GET /health                      // API health
GET /api/Test/public             // Public
GET /api/Test/protected          // Authenticated users
GET /api/Test/admin              // Admin only
```

---

## 📆 Git Workflow

### Branches:

```
main
└ dev
   ├─ feature/ELF
   ├─ feature/config-data-layer
   └─ feature/[task-name]
```

### Commit Convention:

```
feat: Add User entity and configurations
fix: Fix connection string issue
docs: Update README
refactor: Improve repository pattern
```

### Quy trình cho Feature Mới:

```bash
git checkout -b feature/[issue-name]
# Code & Test
# Commit & Push
# PR → dev branch
```

---

## 🔮 Troubleshooting

| Lỗi               | Nguyên nhân          | Giải pháp                                              |
| ----------------- | -------------------- | ------------------------------------------------------ |
| Missing EF Design | Thiếu package        | `Install-Package Microsoft.EntityFrameworkCore.Design` |
| SQL Error         | Chưa chạy SQL Server | Mở SQL Server + Kiểm tra chuỗi kết nối                 |
| Port in use       | Trùng port           |                                                        |

```bash
netstat -ano | findstr :7178
taskkill /PID [PID] /F
```

---

## 📄 License

Proprietary software for ITMM. All rights reserved.
