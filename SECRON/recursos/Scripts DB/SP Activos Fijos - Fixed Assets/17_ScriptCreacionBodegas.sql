--SCRIPT DE CREACIÓN DE BODEGAS POR LOCATION (1 BODEGA POR CADA SEDE)
--
INSERT INTO Warehouses (WarehouseCode, WarehouseName, Description, Address, WarehouseType, IsActive, CreatedBy)
SELECT 
    'WH-' + RIGHT('0000' + CAST(LocationCode AS VARCHAR), 3),
    LocationName,
    LocationName,
    ISNULL(Address, '') + ISNULL(' ' + City, ''),
    'REGIONAL',
    1,
    1
FROM Locations
WHERE IsActive = 1
AND LocationId NOT IN (SELECT ISNULL(PrimaryWarehouseId, 0) FROM Locations WHERE PrimaryWarehouseId IS NOT NULL)