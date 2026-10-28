# MyStore — ASP.NET MVC 5 (25DH190104_TranHoangKhai_BE)

Website bán hàng (MyStore) viết bằng **ASP.NET MVC 5 + .NET Framework 4.7.2 + Entity Framework 6 (Database First) + SQL Server**.

## Yêu cầu hệ thống
- Windows 10/11
- Visual Studio 2022 (khi cài tick workload **ASP.NET and web development** — tự kèm SQL Server LocalDB)
- *(Tùy chọn)* SQL Server Management Studio (SSMS)

## Cách chạy (4 bước)

### 1. Tạo database `MyStore`
- Mở SSMS (hoặc trong VS: `View → SQL Server Object Explorer`) → kết nối tới `(LocalDB)\MSSQLLocalDB`
- Mở file **`MyStore_CreateDB.sql`** ở thư mục gốc → Execute (F5)
- Kết quả: tạo DB `MyStore` với 7 bảng + 8 sản phẩm mẫu + tài khoản.

> Nếu bạn dùng SQL Server Express thay vì LocalDB: chạy script trên `.\SQLEXPRESS`, rồi mở `Web.config` sửa `data source=(LocalDB)\MSSQLLocalDB` thành `data source=.\SQLEXPRESS`.

### 2. Mở solution
- Mở file `25DH190104_TranHoangKhai_BE.sln` bằng Visual Studio
- Chuột phải Solution → **Restore NuGet Packages**

### 3. Build & chạy
- `Ctrl+Shift+B` (Build) → `Ctrl+F5` (chạy không debug)
- Trình duyệt mở `http://localhost:<port>`

### 4. Tài khoản đăng nhập
| Vai trò | Username | Password | Ghi chú |
|---|---|---|---|
| Admin | `admin` | `123456` | Truy cập trang `/Admin` để quản trị |
| Khách hàng | `khachhang` | `123456` | Mua hàng, xem lịch sử đơn |

## Cấu trúc dự án
- `25DH190104_TranHoangKhai_BE/` — project web (Controllers, Models, Views, Areas/Admin)
- `Models/MyStoreModel.edmx` — Entity Framework model (Database First)
- `MyStore_CreateDB.sql` — script tạo database + dữ liệu mẫu

## Lưu ý
- Mật khẩu lưu dạng plaintext trong bảng `User` (dự án môn học, không dùng cho production).
- Đổi connection string trong `Web.config` (thẻ `<connectionStrings>`) nếu dùng SQL Server instance khác.
