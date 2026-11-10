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
- `Views/Home/`: Index (banner `~/Content/Images/banner_home.jpg`), ProductList, ProductDetail + partial `_PVFeatureProduct.cshtml`, `_PVNewProduct.cshtml`, `_PVRelatedProduct.cshtml`, `_PVTopProduct.cshtml`.
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

## Công khai website (deploy) — chạy trên web KHÔNG cần Visual Studio
Môi trường Linux không build được .NET Framework — cần máy Windows (có Visual Studio 2022) để publish, HOẶC dùng GitHub Actions (build trên máy ảo Windows của Microsoft, không cần máy của bạn). Dự án đã chuẩn bị sẵn:
- `Properties/PublishProfiles/AzureWebApp.pubxml` — profile publish lên Azure App Service.
- `.github/workflows/azure-deploy.yml` — workflow tự build + deploy mỗi khi push lên `main`.
- `Web.Release.config` — tự chuyển connection string sang production khi publish bản Release.

### Cách A — Deploy bằng GitHub Actions (không cần máy Windows, KHẤN NGHỊ)
1. **Tạo Azure App Service (miễn phí F1):**
   - Vào https://portal.azure.com → Create a resource → **Web App**.
   - Runtime stack: **.NET Framework 4.8** (tương thích 4.7.2), Operating System: **Windows**, Region gần Việt Nam (Southeast Asia / Singapore), Pricing plan: **Free F1**.
   - Đặt tên (vd: `kharistore`) → Create. *(Ảnh chụp: mục Create Web App trên Azure Portal)*
2. **Tạo Azure SQL Database:**
   - Azure Portal → Create a resource → **SQL Database**. Server: tạo mới (đặt admin login + password, region giống App Service). Compute + storage: chọn **Basic** (rẻ nhất) hoặc free trial. Cho phép Azure services truy cập server (Firewall → "Allow Azure services and resources to access this server" = Yes).
   - Sau khi tạo xong: vào SQL Database → **Query editor** (hoặc dùng SSMS kết nối tới server) → mở file `MyStore_CreateDB.sql` → paste → Run. *(Ảnh chụp: Query editor chạy script tạo bảng)*
3. **Khai báo connection string trong App Service (không lộ mật khẩu):**
   - App Service → Settings → **Configuration** → Connection strings → **New connection string**:
     - Name: `MyStoreEntities`
     - Type: **Custom** (QUAN TRỌNG — vì EF Database-First dùng EntityClient)
     - Value: `metadata=res://*/Models.MyStoreModel.csdl|res://*/Models.MyStoreModel.ssdl|res://*/Models.MyStoreModel.msl;provider=System.Data.SqlClient;provider connection string="data source=<TEN-SERVER>.database.windows.net;initial catalog=<TEN-DB>;user id=<ADMIN-LOGIN>;password=<ADMIN-PASSWORD>;trustservercertificate=True;MultipleActiveResultSets=True;App=EntityFramework"`
   - Save. *(Ảnh chụp: mục Connection strings trong Configuration)*
4. **Lấy Publish Profile:**
   - App Service → Overview → nút **Get publish profile** → tải file `.PublishSettings` về.
5. **Cấu hình GitHub Secrets:**
   - Trên GitHub repo → Settings → Secrets and variables → **Actions** → New repository secret:
     - `AZURE_WEBAPP_NAME` = tên App Service (vd: `kharistore`)
     - `AZURE_WEBAPP_PUBLISH_PROFILE` = toàn bộ nội dung file `.PublishSettings` (mở bằng Notepad, copy hết).
6. **Sửa tên trong profile:** mở `Properties/PublishProfiles/AzureWebApp.pubxml`, thay mọi chỗ `__TEN-WEBAPP__` bằng tên App Service (vd: `kharistore`). Commit + push.
7. **Xem kết quả:** tab **Actions** trên GitHub → chờ workflow xanh (3–5 phút) → truy cập `https://<ten-webapp>.azurewebsites.net`.

### Cách B — Publish thủ công bằng Visual Studio 2022 (Windows)
1. Thực hiện bước 1–4 ở Cách A (tạo App Service + Azure SQL + chạy script + khai báo connection string).
2. Mở solution trong VS → chuột phải project → **Publish** → **Import profile** → chọn file `.PublishSettings` đã tải. (Hoặc chọn profile "AzureWebApp" có sẵn, nhập password từ file .PublishSettings.)
3. Configuration chọn **Release** → Publish. Xong, trình duyệt mở trang web công khai.

### Cách C — Hosting ASP.NET miễn phí khác
SmarterASP.NET / Somee.com hỗ trợ .NET Framework 4.7.2 + SQL Server miễn phí (giới hạn băng thông/lưu trữ): publish kiểu FileSystem, upload qua FTP, tạo SQL trên hosting, chạy `MyStore_CreateDB.sql`, cập nhật connection string trong `Web.config`.

> **Lưu ý EDMX:** sau khi chạy `Update_Part3.sql` trên DB production, mở `MyStoreModel.edmx` trong VS → **Update Model from Database** để EF đồng bộ các cột mới (DeliveryMethod/PaymentMethod/ShippingAddress/TotalPrice), rồi publish lại.

## Cấu trúc dự án
- `25DH190104_TranHoangKhai_BE/` — project web (Controllers, Models, Views, Areas/Admin)
- `Models/MyStoreModel.edmx` — Entity Framework model (Database First)
- `MyStore_CreateDB.sql` — script tạo database + dữ liệu mẫu
- `Update_Part3.sql` — script cập nhật schema Phần 3

## Lưu ý
- Mật khẩu lưu dạng plaintext trong bảng `User` (dự án môn học, không dùng cho production).
- Đổi connection string trong `Web.config` (thẻ `<connectionStrings>`) nếu dùng SQL Server instance khác.
- Thương hiệu: KHARI Store (đổi từ FASTOR). Banner trang chủ: `Content/Images/banner_home.jpg`.
