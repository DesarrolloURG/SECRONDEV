-- Bodegas (Puntos físicos de almacenamiento)
CREATE TABLE Warehouses (
    WarehouseId INT IDENTITY(1,1) PRIMARY KEY,
    WarehouseCode VARCHAR(20) NOT NULL UNIQUE,
    WarehouseName VARCHAR(100) NOT NULL,
    Description VARCHAR(255),
    
    -- Ubicación física
    Address VARCHAR(500),
    PhoneNumber VARCHAR(20),
    
    -- Responsable
    ManagerUserId INT,
    
    -- Tipo de bodega
    WarehouseType VARCHAR(50), -- CENTRAL, REGIONAL, LOCAL
    
    -- Control
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy INT,
    ModifiedDate DATETIME,
    ModifiedBy INT,
    
    CONSTRAINT FK_Warehouses_Manager FOREIGN KEY (ManagerUserId) 
        REFERENCES Users(UserId),
    CONSTRAINT FK_Warehouses_CreatedBy FOREIGN KEY (CreatedBy) 
        REFERENCES Users(UserId),
    CONSTRAINT FK_Warehouses_ModifiedBy FOREIGN KEY (ModifiedBy) 
        REFERENCES Users(UserId)
);

SELECT * FROM Warehouses