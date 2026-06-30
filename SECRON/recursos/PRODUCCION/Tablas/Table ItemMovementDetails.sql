-- Tabla Detalles Movimientos de Items
CREATE TABLE ItemMovementDetails (
    MovementDetailId INT IDENTITY(1,1) PRIMARY KEY,
    MovementMasterId INT NOT NULL,
    
    ItemId INT NOT NULL,
    Quantity DECIMAL(18,2) NOT NULL,
    UnitCost DECIMAL(15,2) DEFAULT 0,
    TotalCost AS (Quantity * UnitCost),                -- Calculado
    
    StockBeforeMovement DECIMAL(18,2) NOT NULL,        -- Stock antes del movimiento
    StockAfterMovement DECIMAL(18,2) NOT NULL,         -- Stock después del movimiento
    
    -- Lotes (si aplica)
    LotNumber VARCHAR(50),
    ExpiryDate DATE,
    
    -- Observaciones específicas del ítem
    Remarks VARCHAR(500),
    
    CONSTRAINT FK_ItemMovementDetails_Master FOREIGN KEY (MovementMasterId) REFERENCES ItemMovementMaster(MovementMasterId) ON DELETE CASCADE,
    CONSTRAINT FK_ItemMovementDetails_Item FOREIGN KEY (ItemId) REFERENCES Items(ItemId)
);