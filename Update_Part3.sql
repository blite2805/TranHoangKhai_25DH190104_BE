-- ============================================================
-- Update_Part3.sql — Cập nhật schema CSDL MyStore cho Phần 3 Customer Site
-- Cách dùng: Mở SSMS / Visual Studio -> kết nối (LocalDB)\MSSQLLocalDB -> New Query -> paste -> Execute
-- Mọi lệnh đều kiểm tra IF EXISTS trước khi chạy -> chạy nhiều lần vẫn an toàn (idempotent).
-- ============================================================
USE MyStore;
GO

-- 1. Thêm cột DeliveryMethod (phương thức giao hàng) vào bảng [Order] nếu chưa có
IF COL_LENGTH('dbo.[Order]', 'DeliveryMethod') IS NULL
BEGIN
    ALTER TABLE dbo.[Order] ADD DeliveryMethod nvarchar(50) NULL;
    PRINT 'Da them cot DeliveryMethod vao bang [Order]';
END
ELSE
    PRINT 'Cot DeliveryMethod da ton tai, bo qua';
GO

-- 2. Thêm cột PaymentMethod (phương thức thanh toán) vào bảng [Order] nếu chưa có
IF COL_LENGTH('dbo.[Order]', 'PaymentMethod') IS NULL
BEGIN
    ALTER TABLE dbo.[Order] ADD PaymentMethod nvarchar(50) NULL;
    PRINT 'Da them cot PaymentMethod vao bang [Order]';
END
ELSE
    PRINT 'Cot PaymentMethod da ton tai, bo qua';
GO

-- 3. Đổi tên cột AddressDelivery thành ShippingAddress trong bảng [Order]
--    Chỉ đổi khi cột cũ vẫn còn và cột mới chưa tồn tại.
IF COL_LENGTH('dbo.[Order]', 'AddressDelivery') IS NOT NULL
   AND COL_LENGTH('dbo.[Order]', 'ShippingAddress') IS NULL
BEGIN
    EXEC sp_rename 'dbo.[Order].AddressDelivery', 'ShippingAddress', 'COLUMN';
    PRINT 'Da doi ten cot AddressDelivery -> ShippingAddress';
END
ELSE
    PRINT 'Cot AddressDelivery khong ton tai hoac ShippingAddress da ton tai, bo qua';
GO

-- 4. Thêm computed column TotalPrice = Quantity * UnitPrice (PERSISTED) vào bảng OrderDetail nếu chưa có.
--    PERSISTED: SQL Server tự tính và lưu giá trị khi insert/update; EF KHÔNG cần insert cột này.
IF COL_LENGTH('dbo.OrderDetail', 'TotalPrice') IS NULL
BEGIN
    ALTER TABLE dbo.OrderDetail ADD TotalPrice AS (Quantity * UnitPrice) PERSISTED;
    PRINT 'Da them computed column TotalPrice (PERSISTED) vao bang OrderDetail';
END
ELSE
    PRINT 'Cot TotalPrice da ton tai, bo qua';
GO

PRINT '=== Update_Part3.sql chay thanh cong! ===';
GO
