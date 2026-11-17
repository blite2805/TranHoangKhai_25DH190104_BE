-- =====================================================================
-- MyStore - Tao CSDL + bang + sua cot theo yeu cau bai tap + seed du lieu
-- Chay trong SQL Server (SSMS) voi tu quyen admin (sa).
-- =====================================================================
IF DB_ID('MyStore') IS NULL CREATE DATABASE MyStore;
GO
USE MyStore;
GO

-- Bang User: quan ly tai khoan cua Admin va Khach hang
IF OBJECT_ID('dbo.[User]') IS NULL
CREATE TABLE [dbo].[User] (
    [Username] NVARCHAR(255) NOT NULL,
    [Password] NCHAR(50)     NOT NULL,
    [UserRole] NCHAR(1)      NOT NULL, -- A: admin, C: customer
    PRIMARY KEY CLUSTERED ([Username] ASC)
);
GO

-- Bang Customer
IF OBJECT_ID('dbo.Customer') IS NULL
CREATE TABLE [dbo].[Customer] (
    [CustomerID]      INT IDENTITY(1,1) NOT NULL,
    [CustomerName]    NVARCHAR(MAX) NOT NULL,
    [CustomerPhone]   NVARCHAR(15)  NOT NULL,
    [CustomerEmail]   NVARCHAR(MAX) NULL,
    [CustomerAddress] NVARCHAR(MAX) NULL,
    [Username]        NVARCHAR(255) NOT NULL,
    PRIMARY KEY CLUSTERED ([CustomerID] ASC),
    CONSTRAINT [FK_User_Customer] FOREIGN KEY ([Username]) REFERENCES [dbo].[User]([Username])
);
GO

-- Bang Category
IF OBJECT_ID('dbo.Category') IS NULL
CREATE TABLE [dbo].[Category] (
    [CategoryID]   INT IDENTITY(1,1) NOT NULL,
    [CategoryName] NVARCHAR(MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([CategoryID] ASC)
);
GO

-- Bang Product (luu y ten cot dung theo de bai: ProductDecription)
IF OBJECT_ID('dbo.Product') IS NULL
CREATE TABLE [dbo].[Product] (
    [ProductID]         INT IDENTITY(1,1) NOT NULL,
    [CategoryID]        INT NOT NULL,
    [ProductName]       NVARCHAR(MAX) NOT NULL,
    [ProductDecription] NVARCHAR(MAX) NOT NULL,
    [ProductPrice]      DECIMAL(18,2) NOT NULL,
    [ProductImage]      NVARCHAR(MAX) NULL,
    PRIMARY KEY CLUSTERED ([ProductID] ASC),
    CONSTRAINT [FK_Pro_Category] FOREIGN KEY ([CategoryID]) REFERENCES [dbo].[Category]([CategoryID])
);
GO

-- Bang Order (them DeliveryMethod, PaymentMethod; doi ten AddressDelivery -> ShippingAddress)
IF OBJECT_ID('dbo.[Order]') IS NULL
CREATE TABLE [dbo].[Order] (
    [OrderID]         INT IDENTITY(1,1) NOT NULL,
    [CustomerID]      INT NOT NULL,
    [OrderDate]       DATE NOT NULL,
    [TotalAmount]     DECIMAL(18,2) NOT NULL,
    [PaymentStatus]   NVARCHAR(MAX) NULL,
    [ShippingAddress] NVARCHAR(MAX) NOT NULL,
    [DeliveryMethod]  NVARCHAR(MAX) NULL,
    [PaymentMethod]   NVARCHAR(MAX) NULL,
    PRIMARY KEY CLUSTERED ([OrderID] ASC),
    FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[Customer]([CustomerID])
);
GO

-- Bang OrderDetail (them cot tinh TotalPrice = Quantity * UnitPrice PERSISTED)
IF OBJECT_ID('dbo.OrderDetail') IS NULL
CREATE TABLE [dbo].[OrderDetail] (
    [ID]        INT IDENTITY(1,1) NOT NULL,
    [ProductID] INT NOT NULL,
    [OrderID]   INT NOT NULL,
    [Quantity]  INT NOT NULL,
    [UnitPrice] DECIMAL(18,2) NOT NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC),
    FOREIGN KEY ([ProductID]) REFERENCES [dbo].[Product]([ProductID]),
    FOREIGN KEY ([OrderID])   REFERENCES [dbo].[Order]([OrderID])
);
GO
IF COL_LENGTH('dbo.OrderDetail','TotalPrice') IS NULL
ALTER TABLE dbo.OrderDetail ADD TotalPrice AS (Quantity * UnitPrice) PERSISTED;
GO

ALTER AUTHORIZATION ON DATABASE::MyStore TO sa;
GO

-- ======================= SEED DU LIEU MAU =======================
IF NOT EXISTS (SELECT 1 FROM [dbo].[User] WHERE Username='admin')
    INSERT INTO [dbo].[User](Username,Password,UserRole) VALUES('admin','Admin@123','A');
GO
IF NOT EXISTS (SELECT 1 FROM dbo.Category)
    INSERT INTO dbo.Category(CategoryName) VALUES
     (N'Điện thoại'),(N'Laptop & Tablet'),(N'Âm thanh'),(N'Phụ kiện');
GO
IF NOT EXISTS (SELECT 1 FROM dbo.Product)
    INSERT INTO dbo.Product(CategoryID,ProductName,ProductDecription,ProductPrice,ProductImage) VALUES
     (1,N'iPhone 16 Pro 256GB',N'Chip A18 Pro, camera 48MP, khung titan.',28990000,''),
     (1,N'Samsung Galaxy S25 Ultra',N'Màn hình 6.9 inch, bút S Pen, zoom 100x.',31990000,''),
     (1,N'Xiaomi 14T Pro',N'Sạc nhanh 120W, ống kính Leica.',13990000,''),
     (2,N'MacBook Air M3 13 inch',N'Pin 18 giờ, nặng 1.24kg, không quạt.',27490000,''),
     (2,N'iPad Air M2 11 inch',N'Hỗ trợ Apple Pencil Pro, màn Liquid Retina.',16990000,''),
     (3,N'AirPods Pro 2',N'Chống ồn chủ động, cổng USB-C.',5890000,''),
     (3,N'Sony WH-1000XM5',N'Tai nghe chụp tai chống ồn hàng đầu.',7490000,''),
     (4,N'Sạc nhanh Anker 65W',N'3 cổng, GaN nhỏ gọn.',890000,''),
     (4,N'Pin dự phòng 20.000mAh',N'Sạc nhanh 22.5W, hai cổng ra.',650000,'');
GO
PRINT 'MyStore da duoc tao va seed du lieu mau. Admin: admin / Admin@123';
