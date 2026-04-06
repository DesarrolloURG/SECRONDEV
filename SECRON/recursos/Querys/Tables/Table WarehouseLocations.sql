-- Ubicaciones dentro de las Bodegas (Estantes, Pasillos, Secciones)
CREATE TABLE WarehouseLocations (
    WarehouseLocationId INT IDENTITY(1,1) PRIMARY KEY,
    WarehouseId INT NOT NULL,
    LocationCode VARCHAR(20) NOT NULL,
    LocationName VARCHAR(100) NOT NULL,
    Description VARCHAR(255),
    
    -- Estructura física
    Aisle VARCHAR(10),      -- Pasillo
    Rack VARCHAR(10),       -- Estante
    Shelf VARCHAR(10),      -- Nivel
    Position VARCHAR(10),   -- Posición
    
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy INT,
    
    CONSTRAINT FK_WarehouseLocations_Warehouse FOREIGN KEY (WarehouseId) 
        REFERENCES Warehouses(WarehouseId),
    CONSTRAINT FK_WarehouseLocations_CreatedBy FOREIGN KEY (CreatedBy) 
        REFERENCES Users(UserId),
    CONSTRAINT UK_WarehouseLocation UNIQUE (WarehouseId, LocationCode)
);

SELECT * FROM WarehouseLocations