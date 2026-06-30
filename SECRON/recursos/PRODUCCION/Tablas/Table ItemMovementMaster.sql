 -- Tabla Maestra de Movimientos de Items
CREATE TABLE ItemMovementMaster (
    MovementMasterId INT IDENTITY(1,1) PRIMARY KEY,
    MovementNumber VARCHAR(30) NOT NULL UNIQUE,
    MovementDate DATETIME NOT NULL DEFAULT GETDATE(),
    MovementTypeId INT NOT NULL,
    LocationId INT NOT NULL,                           -- Sede origen
    
    -- Referencias
    SupplierId INT,
    ReferenceDocument VARCHAR(100),
    DestinationLocationId INT,                         -- Para transferencias
    
    -- Información general
    Remarks TEXT,
    TotalAmount DECIMAL(15,2) DEFAULT 0,               -- Suma de todos los ítems
    
    -- Auditoría
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy INT NOT NULL,
    ModifiedDate DATETIME,
    ModifiedBy INT,
    IsActive BIT DEFAULT 1,
    
    CONSTRAINT FK_ItemMovementMaster_MovementType FOREIGN KEY (MovementTypeId) REFERENCES MovementTypes(MovementTypeId),
    CONSTRAINT FK_ItemMovementMaster_Location FOREIGN KEY (LocationId) REFERENCES Locations(LocationId),
    CONSTRAINT FK_ItemMovementMaster_DestinationLocation FOREIGN KEY (DestinationLocationId) REFERENCES Locations(LocationId),
    CONSTRAINT FK_ItemMovementMaster_Supplier FOREIGN KEY (SupplierId) REFERENCES Suppliers(SupplierId),
    CONSTRAINT FK_ItemMovementMaster_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_ItemMovementMaster_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserId)
);
