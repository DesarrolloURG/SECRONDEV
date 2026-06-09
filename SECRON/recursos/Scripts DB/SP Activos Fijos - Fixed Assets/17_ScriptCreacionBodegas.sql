-- 1. Deshabilitar FK
EXEC sp_msforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';
GO

-- 2. Eliminar en lugar de truncar
DELETE FROM Warehouses;
GO

-- 3. Reinsertar con LocationId
INSERT INTO Warehouses (WarehouseCode, WarehouseName, Description, Address, WarehouseType, LocationId, IsActive, CreatedBy)
SELECT 
    'WH-' + RIGHT('0000' + CAST(LocationCode AS VARCHAR), 3),
    LocationName,
    LocationName,
    ISNULL(Address, '') + ISNULL(' ' + City, ''),
    'REGIONAL',
    LocationId,
    1,
    1
FROM Locations
WHERE IsActive = 1
AND LocationId NOT IN (SELECT ISNULL(PrimaryWarehouseId, 0) FROM Locations WHERE PrimaryWarehouseId IS NOT NULL);
GO

-- 4. Rehabilitar FK
EXEC sp_msforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL';
GO

SELECT WarehouseId, WarehouseCode, WarehouseName, LocationId 
FROM Warehouses;