-- Tabla de Articulos por Sede
CREATE TABLE ItemStockByLocation (
    ItemStockLocationId INT IDENTITY(1,1) PRIMARY KEY,
    ItemId INT NOT NULL,
    LocationId INT NOT NULL,                           -- La bodega/sede ES la Location
    
    CurrentStock DECIMAL(18,2) DEFAULT 0 NOT NULL,
    ReservedStock DECIMAL(18,2) DEFAULT 0,
    AvailableStock AS (CurrentStock - ReservedStock),
    
    MinimumStock DECIMAL(18,2) DEFAULT 0,
    MaximumStock DECIMAL(18,2),
    
    LastMovementDate DATETIME,
    
    IsActive BIT DEFAULT 1,
    
    CONSTRAINT FK_ItemStockByLocation_Item FOREIGN KEY (ItemId) REFERENCES Items(ItemId),
    CONSTRAINT FK_ItemStockByLocation_Location FOREIGN KEY (LocationId) REFERENCES Locations(LocationId),
    CONSTRAINT UQ_ItemStockByLocation UNIQUE (ItemId, LocationId)
);

-- Agregar bodega a ItemStockByLocation
ALTER TABLE ItemStockByLocation 
ADD WarehouseId INT NULL,
    WarehouseLocationId INT NULL; -- Ubicación específica dentro de bodega

ALTER TABLE ItemStockByLocation 
ADD CONSTRAINT FK_ItemStock_Warehouse 
    FOREIGN KEY (WarehouseId) REFERENCES Warehouses(WarehouseId);

ALTER TABLE ItemStockByLocation 
ADD CONSTRAINT FK_ItemStock_WarehouseLocation 
    FOREIGN KEY (WarehouseLocationId) REFERENCES WarehouseLocations(WarehouseLocationId);

SELECT * FROM ItemStockByLocation