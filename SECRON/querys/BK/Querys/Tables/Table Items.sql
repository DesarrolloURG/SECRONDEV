-- Tabla de artículos
CREATE TABLE Items (
    ItemId INT IDENTITY(1,1) PRIMARY KEY,
    ItemCode VARCHAR(50) NOT NULL UNIQUE,
    ItemName VARCHAR(200) NOT NULL,
    Description VARCHAR(500),
    CategoryId INT NOT NULL,
    UnitId INT NOT NULL,
    
    -- Control de Stock (valores por defecto globales)
    MinimumStock DECIMAL(18,2) DEFAULT 0,
    MaximumStock DECIMAL(18,2),
    ReorderPoint DECIMAL(18,2),
    
    -- Valorización
    UnitCost DECIMAL(15,2) DEFAULT 0,
    LastPurchasePrice DECIMAL(15,2),
    
    -- Control
    HasLotControl BIT DEFAULT 0,
    HasExpiryDate BIT DEFAULT 0,
    IsActive BIT DEFAULT 1,
    
    -- Auditoría
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy INT,
    ModifiedDate DATETIME,
    ModifiedBy INT,
    
    CONSTRAINT FK_Items_Category FOREIGN KEY (CategoryId) REFERENCES ItemCategories(CategoryId),
    CONSTRAINT FK_Items_Unit FOREIGN KEY (UnitId) REFERENCES MeasurementUnits(UnitId),
    CONSTRAINT FK_Items_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_Items_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserId)
);

SELECT * FROM Items


DELETE FROM Items Where ItemCode = '000002'
UPDATE Items SET IsActive = 1 WHERE ItemCode = '000001'