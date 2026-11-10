# KHARI Store — ASP.NET MVC 5 (25DH190104_TranHoangKhai_BE)
Website bán hàng (KHARI Store, trước đây là FASTOR) viết bằng **ASP.NET MVC 5 + .NET Framework 4.7.2 + Entity Framework 6 (Database First) + SQL Server**.
Đề tài: Môn Phát triển Web Backend — MSSV 25DH190104 — Trần Hoàng Khải.

## Yêu cầu hệ thống
- Windows 10/11
- Visual Studio 2022 (tick workload **ASP.NET and web development** — tự kèm SQL Server LocalDB)
- *(Tùy chọn)* SQL Server Management Studio (SSMS)

## Cách chạy (4 bước)
### 1. Tạo database `MyStore`
- Mở SSMS (hoặc trong VS: `View → SQL Server Object Explorer`) → kết nối tới `(LocalDB)\MSSQLLocalDB`
- Mở file **`MyStore_CreateDB.sql`** ở thư mục gốc → Execute (F5)
- Nếu DB đã tồn tại từ phần trước và chỉ muốn cập nhật schema Phần 3: chạy **`Update_Part3.sql`** (thêm cột DeliveryMethod/PaymentMethod, đổi tên AddressDelivery→ShippingAddress, thêm computed column TotalPrice vào OrderDetail).
> Nếu dùng SQL Server Express: chạy script trên `.\SQLEXPRESS`, rồi mở `Web.config` sửa `data source=(LocalDB)\MSSQLLocalDB` thành `data source=.\SQLEXPRESS`.

### 2. Mở solution
- Mở `25DH190104_TranHoangKhai_BE.sln` bằng Visual Studio
- Chuột phải Solution → **Restore NuGet Packages** (gồm PagedList + PagedList.Mvc đã khai báo trong `packages.config`)

### 3. Build & chạy
- `Ctrl+Shift+B` (Build) → `Ctrl+F5`
- Trình duyệt mở `http://localhost:<port>`

### 4. Tài khoản đăng nhập
| Vai trò | Username | Password | Ghi chú |
|---|---|---|---|
| Admin | `admin` | `123456` | Truy cập `/Admin` để quản trị |
| Khách hàng | `khachhang` | `123456` | Mua hàng, xem lịch sử đơn |

## Phần 3 — Customer Site (đã cập nhật)
### Controllers
- **AccountController** (thay UserController): Register (GET/POST, kiểm tra trùng username), Login (GET/POST, FormsAuthentication + Session, tự khôi phục giỏ hàng), Logout, ProfileInfo (xem + sửa SĐT/Email/Địa chỉ), UpdateProfile (POST), ChangePassword (POST).
- **OrderController** (tách checkout khỏi Cart): Checkout (GET/POST, mặc định ShippingAddress từ Customer, nút "Chỉnh sửa" địa chỉ, tạo Order+OrderDetail, xóa giỏ sau khi đặt), OrderSuccess, MyOrder (lịch sử mua hàng, tab theo trạng thái + tìm kiếm), OrderDetail.
- **CartController**: GetCartService, Index (nhóm sản phẩm theo danh mục), AddToCart (POST, trùng SP → cộng dồn số lượng, chặn số lượng âm), RemoveFromCart (POST), ClearCart, UpdateQuantity (POST). Giữ cơ chế `UserCarts` static khôi phục giỏ khi login/logout.
- **HomeController**: Index (HomeProductVM: FeaturedProducts top10 bán chạy + NewProducts phân trang 6/trang), ProductList (theo danh mục + tìm kiếm + sắp xếp + phân trang), ProductDetail (GET/POST, RelatedProducts 8sp + TopProducts 8sp, POST thêm vào giỏ).
- **Admin** (giữ nguyên Areas/Admin): Categories/Products — bổ sung kiểm tra trùng tên khi thêm, bắt `DbUpdateException` khi xóa bản ghi có ràng buộc khóa ngoại → thông báo thân thiện.

### ViewModels (`Models/ViewModel/`)
RegisterVM, LoginVM, PersonalMenuVM, HomeProductVM, ProductDetailVM, CheckoutVM — đều có Data Annotation ([Required], [StringLength], [EmailAddress], [Phone]/[RegularExpression], [Compare]).

### Views & Partial Views
- `Views/Shared/_CustomerLayout.cshtml` (layout khách hàng, header tách partial: `_CategoriesPV.cshtml`, `_PersonalMenuPV.cshtml`; footer thông tin đề tài). `_ViewStart.cshtml` trỏ về layout này.
- `Views/Account/`: Register, Login, ProfileInfo.
- `Views/Order/`: Checkout, OrderSuccess, MyOrder, OrderDetail.
- `Views/Cart/`: Index (nhóm theo danh mục) + `_PVSimilarProduct.cshtml`.
- `Views/Home/`: Index (banner `~/Content/images/banner_home.jpg`), ProductList, ProductDetail + partial `_PVFeatureProduct.cshtml`, `_PVNewProduct.cshtml`, `_PVRelatedProduct.cshtml`, `_PVTopProduct.cshtml`.
- CSS riêng: `Content/myStyle/CustomerHome.css`, `CustomerDetailProduct.css` (bundle `~/Content/customerCss`).

### Điều kiện kiểm tra/validation (theo yêu cầu)
1. **Chưa có dữ liệu**: mọi trang list (Home, ProductList, Cart, MyOrder, Admin) hiển thị "Không có dữ liệu" khi danh sách rỗng.
2. **Xóa bản ghi có constraint**: Admin Categories/Products bắt `DbUpdateException` → thông báo lỗi thân thiện, không crash.
3. **Thêm trùng**: Register kiểm tra trùng Username; Admin kiểm tra trùng tên Category/Product; AddToCart trùng sản phẩm → cộng dồn số lượng.
4. **Kiểm tra định dạng**: [EmailAddress], [RegularExpression] SĐT (chữ số), [RegularExpression] Username (chữ+số).
5. **Kiểm tra độ dài**: [StringLength] cho mọi trường text (Username ≤50, Password 6–50, CustomerName ≤100, Phone 10–15, Address ≤200...).
6. Bổ sung: chặn số lượng âm/không hợp lệ khi thêm giỏ, kiểm tra null, `TempData` thông báo thành công/lỗi.

### SQL
- `Update_Part3.sql`: chạy khi DB đã có từ phần trước (ALTER TABLE an toàn, idempotent).
- `MyStore_CreateDB.sql`: đã bao gồm sẵn các thay đổi (ShippingAddress, DeliveryMethod, PaymentMethod, computed TotalPrice).

## Công khai website (deploy)
Môi trường Linux không build được .NET Framework — cần Visual Studio trên Windows để publish:
1. **Azure App Service (F1 free)**: Trong VS → chuột phải project → **Publish** → Azure → Azure App Service (Linux/Windows) → tạo plan giá Free F1 → publish. Cần SQL Server public: dùng **Azure SQL Database** (có lớp Basic/Serverless rẻ hoặc dùng free trial), rồi đổi connection string trong Web.config / Application Settings của App Service.
2. **Hosting ASP.NET miễn phí**: SmarterASP.NET hoặc Somee.com hỗ trợ .NET Framework 4.7.2 + SQL Server miễn phí (giới hạn băng thông/lưu trữ). Upload file publish (FileSystem) qua FTP + tạo SQL Server trên hosting, chạy script `MyStore_CreateDB.sql` trên DB của hosting, cập nhật connection string.
3. Lưu ý: sau khi chạy `Update_Part3.sql` trên DB, nên **Update Model from Database** trong EDMX (double-click `MyStoreModel.edmx` → Update Model from Database) để EF đồng bộ các cột mới (DeliveryMethod/PaymentMethod/ShippingAddress/TotalPrice).

## Cấu trúc dự án
- `25DH190104_TranHoangKhai_BE/` — project web (Controllers, Models, Views, Areas/Admin)
- `Models/MyStoreModel.edmx` — Entity Framework model (Database First)
- `MyStore_CreateDB.sql` — script tạo database + dữ liệu mẫu
- `Update_Part3.sql` — script cập nhật schema Phần 3

## Lưu ý
- Mật khẩu lưu dạng plaintext trong bảng `User` (dự án môn học, không dùng cho production).
- Đổi connection string trong `Web.config` (thẻ `<connectionStrings>`) nếu dùng SQL Server instance khác.
- Thương hiệu: KHARI Store (đổi từ FASTOR). Banner trang chủ: `Content/images/banner_home.jpg`.
