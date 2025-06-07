# Infertility Treatment Management System

## 📋 Tổng quan Dự án

Hệ thống quản lý và theo dõi toàn diện quá trình điều trị hiếm muộn cho cơ sở y tế, được xây dựng với kiến trúc 4-layer sử dụng ASP.NET Core Web API và Entity Framework Core.

### 🎯 Mục tiêu
- Quản lý chu kỳ điều trị hiếm muộn (IUI, IVF)
- Theo dõi tiến trình điều trị của bệnh nhân
- Hệ thống đặt lịch hẹn thông minh
- Dashboard báo cáo và thống kê

### 👥 Đối tượng Sử dụng
- **Customer**: Bệnh nhân đăng ký điều trị
- **Doctor**: Bác sĩ điều trị
- **Manager**: Quản lý cơ sở y tế
- **Admin**: Quản trị hệ thống

---

## 🏗️ Kiến trúc Hệ thống

### **4-Layer Architecture**

```
📁 InfertilityTreatment.sln
├── 📁 InfertilityTreatment.API/          → Presentation Layer
├── 📁 InfertilityTreatment.Business/     → Business Logic Layer  
├── 📁 InfertilityTreatment.Data/         → Data Access Layer
└── 📁 InfertilityTreatment.Entity/       → Entity Layer
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

- **Visual Studio 2022** (Community/Professional)
- **.NET 8.0 SDK**
- **SQL Server** (Express/LocalDB/Full)
- **Git** for version control

### **🔧 Bước 1: Clone Repository**

```bash
git clone <repository-url>
cd InfertilityTreatment
```

### **🔧 Bước 2: Mở Solution**

1. **Mở Visual Studio 2022**
2. **File** → **Open** → **Project or Solution**
3. Chọn file `InfertilityTreatment.sln`

### **🔧 Bước 3: Restore NuGet Packages**

```bash
# Trong Visual Studio
Build → Restore NuGet Packages
```

Hoặc sử dụng Package Manager Console:
```bash
dotnet restore
```

### **🔧 Bước 4: Cấu hình Database**

#### **4.1. Cấu hình Connection String**

Mở file `appsettings.json` trong project **InfertilityTreatment.API** và cập nhật:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=InfertilityTreatmentDB;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

**Chú thích Connection String:**
- `Server=.` → SQL Server local instance
- `Database=InfertilityTreatmentDB` → Tên database
- `Trusted_Connection=true` → Windows Authentication
- `TrustServerCertificate=true` → Bỏ qua SSL certificate check

#### **4.2. Tạo Database với Migration**

Mở **Package Manager Console** trong Visual Studio:
- **Tools** → **NuGet Package Manager** → **Package Manager Console**

```bash
# Set default project
Default project: InfertilityTreatment.Data

# Run migration
Update-Database -StartupProject InfertilityTreatment.API
```

### **🔧 Bước 5: Build và Run**

1. **Set Startup Project:**
   - Right-click `InfertilityTreatment.API` → **Set as Startup Project**

2. **Build Solution:**
   ```bash
   Build → Build Solution (Ctrl+Shift+B)
   ```

3. **Run Application:**
   ```bash
   Debug → Start Debugging (F5)
   ```

### **🔧 Bước 6: Verify Setup**

Sau khi run thành công:

1. **Browser tự động mở:** `https://localhost:7178/swagger`
2. **Test Health Endpoint:** `https://localhost:7178/health`
   - Expected response: `"API is running! 🚀"`
3. **Swagger UI hiển thị các endpoints:**
   - GET `/health`
   - GET `/api/Test/public`
   - GET `/api/Test/protected`
   - GET `/api/Test/admin`

---

## 📦 Package Dependencies

### **API Project**
```xml
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
<PackageReference Include="Serilog.AspNetCore" Version="7.0.0" />
<PackageReference Include="FluentValidation.AspNetCore" Version="11.3.0" />
```

### **Entity Project**
```xml
<PackageReference Include="System.ComponentModel.Annotations" Version="5.0.0" />
```

### **Data Project**
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0" />
```

### **Business Project**
```xml
<PackageReference Include="FluentValidation" Version="11.8.0" />
<PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
<PackageReference Include="AutoMapper" Version="12.0.1" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="7.0.0" />
```

---

## 🗄️ Database Schema

### **Entities hiện tại (Issue #BE002 & #BE003 hoàn thành):**

#### **User Management**
- **Users** - Thông tin cơ bản người dùng
- **Customers** - Chi tiết bệnh nhân
- **Doctors** - Chi tiết bác sĩ
- **RefreshTokens** - JWT refresh tokens

#### **Treatment Services**
- **TreatmentServices** - Các dịch vụ (IUI, IVF)
- **TreatmentPackages** - Gói điều trị cụ thể
- **TreatmentCycles** - Chu kỳ điều trị của bệnh nhân

### **Database Migration Files:**
```
📁 Data/Migrations/
├── 20250607113314_InitialCreate.cs
├── 20250607113314_InitialCreate.Designer.cs
└── ApplicationDbContextModelSnapshot.cs
```

---

## 🔐 Authentication & Authorization

### **JWT Configuration**
```json
"JwtSettings": {
  "SecretKey": "InfertilityTreatment-Super-Secret-Key-256-Bits-Long-For-JWT-Security-2024",
  "Issuer": "InfertilityTreatmentAPI",
  "Audience": "InfertilityTreatmentClient",
  "AccessTokenExpiryMinutes": 60,
  "RefreshTokenExpiryDays": 7
}
```

### **Authorization Policies**
- **CustomerOnly** - Chỉ bệnh nhân
- **DoctorOnly** - Chỉ bác sĩ
- **AdminOnly** - Admin và Manager

---

## 📝 API Documentation

### **Swagger UI:** `https://localhost:7178/swagger`

### **Current Endpoints:**

#### **Health Check**
```http
GET /health
Response: "API is running! 🚀"
```

#### **Test Endpoints**
```http
GET /api/Test/public           # Public access
GET /api/Test/protected        # Requires authentication
GET /api/Test/admin           # Requires Admin role
```

---

## 🛠️ Development Workflow

### **Git Branching Strategy**

```bash
main                    # Production branch
├── dev                # Development branch
├── feature/ELF        # Entity Layer Foundation (BE002)
├── feature/config-data-layer  # Data Layer Config (BE003)
└── feature/[task-name]        # New features
```

### **Commit Convention**
```
feat: Add User entity and configurations
fix: Fix connection string issue
docs: Update README with setup instructions
refactor: Improve repository pattern implementation
```

### **Development Steps cho Issues mới:**

1. **Tạo feature branch:**
   ```bash
   git checkout -b feature/[issue-name]
   ```

2. **Implement changes theo acceptance criteria**

3. **Test locally:**
   ```bash
   dotnet build
   dotnet run --project InfertilityTreatment.API
   ```

4. **Commit và push:**
   ```bash
   git add .
   git commit -m "feat: implement [feature-description]"
   git push origin feature/[issue-name]
   ```

---

## 🧪 Testing

### **Build và Test Commands**

```bash
# Build entire solution
dotnet build InfertilityTreatment.sln

# Build specific project
dotnet build InfertilityTreatment.API

# Run application
dotnet run --project InfertilityTreatment.API

# Test endpoints
curl https://localhost:7178/health
curl https://localhost:7178/api/test/public
```

### **Database Commands**

```bash
# Add new migration
Add-Migration [MigrationName] -StartupProject InfertilityTreatment.API

# Update database
Update-Database -StartupProject InfertilityTreatment.API

# Drop database (caution!)
Drop-Database -StartupProject InfertilityTreatment.API
```

---

## ⚠️ Troubleshooting

### **Lỗi thường gặp:**

#### **1. Migration Error**
```
Error: Your startup project doesn't reference Microsoft.EntityFrameworkCore.Design
```
**Solution:** Cài package `Microsoft.EntityFrameworkCore.Design` cho API project

#### **2. SQL Server Connection Error**
```
Error: Cannot open database
```
**Solution:** 
- Kiểm tra SQL Server service đang chạy
- Verify connection string trong appsettings.json
- Thử dùng SQL Server Object Explorer trong Visual Studio

#### **3. Build Error - Missing References**
```
Error: The type or namespace name could not be found
```
**Solution:**
- Restore NuGet packages: `dotnet restore`
- Kiểm tra project references đã đúng chưa
- Rebuild solution: `Ctrl+Shift+B`

#### **4. Port Already in Use**
```
Error: Failed to bind to address
```
**Solution:**
- Đổi port trong `Properties/launchSettings.json`
- Hoặc kill process đang dùng port:
  ```bash
  netstat -ano | findstr :7178
  taskkill /PID [PID_NUMBER] /F
  ```

---

## 📈 Development Roadmap

### **✅ Hoàn thành (Issues #BE001-003):**
- 4-layer project structure setup
- Entity Layer Foundation với base entities
- Data Layer với ApplicationDbContext và migrations
- JWT Authentication infrastructure
- Repository Pattern implementation

### **🔄 Đang triển khai (Issues #BE004-006):**
- JWT Authentication Service implementation
- User Management APIs
- Business Layer services

### **📋 Kế hoạch tiếp theo:**
- Doctor Management System
- Treatment Cycle Management
- Appointment Scheduling
- Test Results Tracking
- Notification System

---

## 🤝 Contributing

### **Quy trình làm việc cho team:**

1. **Assign Issue:** Lấy issue từ backlog
2. **Create Branch:** `feature/[issue-code]-[short-description]`
3. **Develop:** Follow acceptance criteria
4. **Test:** Local testing + swagger documentation
5. **Commit:** Follow commit convention
6. **Push & PR:** Create pull request to dev branch
7. **Code Review:** Team review trước khi merge

### **Code Standards:**
- Follow C# coding conventions
- Use async/await cho database operations
- Implement proper exception handling
- Write meaningful commit messages
- Comment complex business logic

---

## 📞 Support

### **Team Contacts:**
- **Backend Lead:** [Your Name] - [email]
- **Database:** [DB Developer] - [email]
- **DevOps:** [DevOps Engineer] - [email]

### **Resources:**
- **Project Documentation:** `docs/`
- **API Documentation:** `https://localhost:7178/swagger`
- **Database ERD:** `docs/database-erd.md`
- **Postman Collection:** `docs/api-collection.json`

---

## 📄 License

This project is proprietary software for [Company Name]. All rights reserved.

---

**Last Updated:** December 2024
**Version:** 1.0.0
**Status:** In Development 🚧
