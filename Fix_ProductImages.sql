-- Gan anh mau cho cac san pham (chay tren database MyStore)
-- Sau khi Execute xong, quay lai trinh duyet bam F5 la hien anh.
USE MyStore;
GO

UPDATE Product SET ProductImage = 'https://picsum.photos/seed/iphone15/400/300' WHERE ProductName LIKE N'%iPhone%';
UPDATE Product SET ProductImage = 'https://picsum.photos/seed/samsungs24/400/300' WHERE ProductName LIKE N'%Samsung%';
UPDATE Product SET ProductImage = 'https://picsum.photos/seed/macbookair/400/300' WHERE ProductName LIKE N'%MacBook%';
UPDATE Product SET ProductImage = 'https://picsum.photos/seed/lenovo/400/300' WHERE ProductName LIKE N'%Lenovo%';
UPDATE Product SET ProductImage = 'https://picsum.photos/seed/airpods/400/300' WHERE ProductName LIKE N'%AirPods%';
UPDATE Product SET ProductImage = 'https://picsum.photos/seed/anker/400/300' WHERE ProductName LIKE N'%Anker%';
UPDATE Product SET ProductImage = 'https://picsum.photos/seed/applewatch/400/300' WHERE ProductName LIKE N'%Apple Watch%';
UPDATE Product SET ProductImage = 'https://picsum.photos/seed/xiaomiwatch/400/300' WHERE ProductName LIKE N'%Xiaomi%';
GO

PRINT '=== Da gan anh mau cho ' + CAST(@@ROWCOUNT AS nvarchar(10)) + ' san pham. Bam F5 tren trinh duyet de xem anh. ===';
GO
