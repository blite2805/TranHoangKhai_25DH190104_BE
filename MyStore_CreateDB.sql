-- ============================================================
-- Script tao database MyStore cho project 25DH190104_TranHoangKhai_BE
-- Cach dung: Mo SQL Server Management Studio (SSMS) -> New Query -> paste -> Execute
-- Hoac trong Visual Studio: View -> SQL Server Object Explorer -> chua server -> New Query
-- ============================================================

USE master;
GO
IF DB_ID(N'MyStore') IS NULL
    CREATE DATABASE MyStore;
GO
USE MyStore;
GO

-- Xoa bang cu neu co (theo thu tu nguoc dependency)
IF OBJECT_ID('dbo.OrderDetail', 'U') IS NOT NULL DROP TABLE dbo.OrderDetail;
IF OBJECT_ID('dbo.[Order]', 'U') IS NOT NULL DROP TABLE dbo.[Order];
IF OBJECT_ID('dbo.Product', 'U') IS NOT NULL DROP TABLE dbo.Product;
IF OBJECT_ID('dbo.Customer', 'U') IS NOT NULL DROP TABLE dbo.Customer;
IF OBJECT_ID('dbo.Category', 'U') IS NOT NULL DROP TABLE dbo.Category;
IF OBJECT_ID('dbo.[User]', 'U') IS NOT NULL DROP TABLE dbo.[User];
IF OBJECT_ID('dbo.sysdiagrams', 'U') IS NOT NULL DROP TABLE dbo.sysdiagrams;
GO

-- Bang User (quan tri + khach hang dang nhap)
CREATE TABLE dbo.[User] (
    Username    nvarchar(255) NOT NULL PRIMARY KEY,
    [Password]  nchar(50)     NOT NULL,
    UserRole    nchar(1)      NOT NULL  -- A = Admin, C = Customer
);
GO

-- Bang Category (danh muc san pham)
CREATE TABLE dbo.Category (
    CategoryID   int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    CategoryName nvarchar(max)     NOT NULL
);
GO

-- Bang Customer (thong tin khach hang)
CREATE TABLE dbo.Customer (
    CustomerID      int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    CustomerName    nvarchar(max)     NOT NULL,
    CustomerPhone   nvarchar(15)      NOT NULL,
    CustomerEmail   nvarchar(max)     NULL,
    CustomerAddress nvarchar(max)     NULL,
    Username        nvarchar(255)     NOT NULL
        CONSTRAINT FK_User_Customer FOREIGN KEY REFERENCES dbo.[User](Username)
);
GO

-- Bang Product (san pham)
CREATE TABLE dbo.Product (
    ProductID          int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    CategoryID         int               NOT NULL
        CONSTRAINT FK_Pro_Category FOREIGN KEY REFERENCES dbo.Category(CategoryID),
    ProductName        nvarchar(max)     NOT NULL,
    ProductDescription nvarchar(max)     NOT NULL,
    ProductPrice       decimal(18,2)     NOT NULL,
    ProductImage       nvarchar(max)     NULL
);
GO

-- Bang Order (don hang)
CREATE TABLE dbo.[Order] (
    OrderID         int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    CustomerID      int               NOT NULL
        CONSTRAINT FK_Order_Customer FOREIGN KEY REFERENCES dbo.Customer(CustomerID),
    OrderDate       date              NOT NULL,
    TotalAmount     decimal(18,2)     NOT NULL,
    PaymentStatus   nvarchar(max)     NULL,
    AddressDelivery nvarchar(max)     NOT NULL
);
GO

-- Bang OrderDetail (chi tiet don hang)
CREATE TABLE dbo.OrderDetail (
    ID        int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    ProductID int               NOT NULL
        CONSTRAINT FK_OrderDetail_Product FOREIGN KEY REFERENCES dbo.Product(ProductID),
    OrderID   int               NOT NULL
        CONSTRAINT FK_OrderDetail_Order FOREIGN KEY REFERENCES dbo.[Order](OrderID),
    Quantity  int               NOT NULL,
    UnitPrice decimal(18,2)     NOT NULL
);
GO

-- Bang sysdiagrams (he thong, EF co map nen can co de tranh loi)
CREATE TABLE dbo.sysdiagrams (
    name         nvarchar(128)   NOT NULL,
    principal_id int             NOT NULL,
    diagram_id   int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    version      int             NULL,
    definition   varbinary(max)  NULL,
    CONSTRAINT UK_principal_name UNIQUE (principal_id, name)
);
GO

-- ============================================================
-- DU LIEU MAU
-- ============================================================

-- Tai khoan: admin / 123456 (quyen Admin) ; khachhang / 123456 (quyen Customer)
INSERT INTO dbo.[User] (Username, [Password], UserRole) VALUES
    (N'admin',      N'123456', N'A'),
    (N'khachhang',  N'123456', N'C');
GO

INSERT INTO dbo.Category (CategoryName) VALUES
    (N'Điện thoại'),
    (N'Laptop'),
    (N'Phụ kiện'),
    (N'Đồng hồ thông minh');
GO

SET IDENTITY_INSERT dbo.Product ON;
INSERT INTO dbo.Product (ProductID, CategoryID, ProductName, ProductDescription, ProductPrice, ProductImage) VALUES
    (1, 1, N'iPhone 15 Pro Max', N'iPhone 15 Pro Max 256GB Titan Tự Nhiên, mới chính hãng.', 32990000, NULL),
    (2, 1, N'Samsung Galaxy S24 Ultra', N'Samsung Galaxy S24 Ultra 5G 512GB, bút S Pen tích hợp.', 28990000, NULL),
    (3, 2, N'MacBook Air M3', N'MacBook Air M3 13-inch 8GB/256GB, nhẹ mỏng bền bỉ.', 25990000, NULL),
    (4, 2, N'Laptop Lenovo IdeaPad Slim 5', N'Lenovo IdeaPad Slim 5 Core i5-13420H 16GB/512GB.', 14990000, NULL),
    (5, 3, N'Tai nghe AirPods Pro 2', N'AirPods Pro 2 (USB-C) chống ồn chủ động.', 5990000, NULL),
    (6, 3, N'Sạc dự phòng Anker 20000mAh', N'Sạc dự phòng Anker PowerCore 20000mAh sạc nhanh 20W.', 790000, NULL),
    (7, 4, N'Apple Watch Series 9', N'Apple Watch Series 9 GPS 45mm viền nhôm.', 11990000, NULL),
    (8, 4, N'Xiaomi Watch S3', N'Xiaomi Watch S3 AMOLED 1.43 inch, 5ATM, pin 15 ngày.', 3490000, NULL);
SET IDENTITY_INSERT dbo.Product OFF;
GO

PRINT '=== Tao database MyStore thanh cong! ===';
PRINT 'Tai khoan Admin: admin / 123456';
PRINT 'Tai khoan Khach hang: khachhang / 123456';
GO
