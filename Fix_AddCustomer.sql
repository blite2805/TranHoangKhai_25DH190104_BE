-- Them Customer cho tai khoan khachhang (fix loi dat hang)
USE MyStore;
GO
IF NOT EXISTS (SELECT 1 FROM dbo.Customer WHERE Username = 'khachhang')
BEGIN
    INSERT INTO dbo.Customer (CustomerName, CustomerPhone, CustomerEmail, CustomerAddress, Username)
    VALUES (N'Khách Hàng', '0901234567', 'khachhang@example.com', N'123 Nguyễn Văn Linh, Quận 7, TP.HCM', 'khachhang');
    PRINT 'Da them Customer cho khachhang.';
END
ELSE
    PRINT 'Customer khachhang da ton tai.';
GO
