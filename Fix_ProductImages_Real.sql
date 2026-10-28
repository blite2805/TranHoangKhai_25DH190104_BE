-- Gan anh that (da them vao thu muc Content/Images/) cho tung san pham
-- Chay tren database MyStore, sau do F5 trinh duyet la hien anh that.
USE MyStore;
GO

UPDATE Product SET ProductImage = '~/Content/Images/iphone_15_promax.png'     WHERE ProductName LIKE N'%iPhone%';
UPDATE Product SET ProductImage = '~/Content/Images/samsung_s24_ultra.png'    WHERE ProductName LIKE N'%Samsung%';
UPDATE Product SET ProductImage = '~/Content/Images/macbook_air_m3.png'       WHERE ProductName LIKE N'%MacBook%';
UPDATE Product SET ProductImage = '~/Content/Images/lenovo_ideapad.png'       WHERE ProductName LIKE N'%Lenovo%';
UPDATE Product SET ProductImage = '~/Content/Images/airpods_pro2.png'         WHERE ProductName LIKE N'%AirPods%';
UPDATE Product SET ProductImage = '~/Content/Images/anker_powerbank.png'      WHERE ProductName LIKE N'%Anker%';
UPDATE Product SET ProductImage = '~/Content/Images/apple_watch_series9.png'  WHERE ProductName LIKE N'%Apple Watch%';
UPDATE Product SET ProductImage = '~/Content/Images/xiaomi_watch_s3.png'      WHERE ProductName LIKE N'%Xiaomi%';
GO

PRINT '=== Da gan anh that cho 8 san pham. Bam F5 tren trinh duyet de xem! ===';
GO
