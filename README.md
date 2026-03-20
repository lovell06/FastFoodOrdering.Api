# FastFoodOrdering.Api

Backend Web API cho hệ thống đặt món ăn nhanh, được xây dựng bằng **ASP.NET Core** và **Entity Framework Core**.

---

## Yêu cầu môi trường

Trước khi chạy dự án, hãy đảm bảo máy đã cài:

- **.NET SDK 9**
- **SQL Server**
- **Entity Framework Core CLI**

Kiểm tra phiên bản .NET đang có:

```bash
dotnet --version
```

## Cài đặt các package cần thiết

Di chuyển tới thư mục chứa file .csproj, sau đó chạy lần lượt các lệnh sau:

```bash
dotnet add package Microsoft.Extensions.Logging --version 9
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 9
dotnet add package Microsoft.EntityFrameworkCore.Design --version 9
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 9
dotnet add package Scalar.AspNetCore
```

## Cấu hình dotnet-ef tools

```bash
dotnet tool install --global dotnet-ef
```

## Cập nhật database

```bash
dotnet ef database update
```

## Chạy dự án

```bash
dotnet run
```

## Tài liệu API

Dự án sử dụng Scalar để hiển thị tài liệu API.

Sau khi chạy project thành công, mở trình duyệt và truy cập:

```bash
/scalar
```

Ví dụ:

```bash
https://localhost:xxxx/scalar
```

## Ghi chú

- Các package Entity Framework Core đang sử dụng phiên bản 9.

- Scalar.AspNetCore được dùng để hiển thị tài liệu API thay cho Swagger UI.

- Hãy chắc chắn rằng SQL Server đang hoạt động trước khi chạy lệnh cập nhật database.

- Nếu dự án đã có migration sẵn, chỉ cần chạy dotnet ef database update.

## Các API cơ bản của App:

```API
GET /api/products
GET /api/products/detail/{id}
POST /api/auth/register
POST /api/auth/login
```
