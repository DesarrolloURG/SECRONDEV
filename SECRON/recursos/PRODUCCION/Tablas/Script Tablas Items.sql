
------------------------------- TABLA DE UNIDADES DE MEDIDA DE ARTÍCULOS ---------------------------------------
-- Tabla de Unidades de Medida
CREATE TABLE MeasurementUnits (
    UnitId INT IDENTITY(1,1) PRIMARY KEY,
    UnitCode VARCHAR(10) NOT NULL UNIQUE,
    UnitName VARCHAR(50) NOT NULL,
    Abbreviation VARCHAR(10) NOT NULL,
    IsActive BIT DEFAULT 1
);

------------------------------- TABLA DE Tipos de Movimientos que se tienen para Los Artículos ---------------------------------------

-- Tabla de los tipos de movimientos que se tienen
CREATE TABLE MovementTypes (
    MovementTypeId INT IDENTITY(1,1) PRIMARY KEY,
    TypeCode VARCHAR(20) NOT NULL UNIQUE,
    TypeName VARCHAR(50) NOT NULL,
    AffectsStock VARCHAR(10) NOT NULL,                 -- '+', '-', '0'
    RequiresSupplier BIT DEFAULT 0,
    RequiresDestination BIT DEFAULT 0,
    IsActive BIT DEFAULT 1
);

--- Valores predeterminados

INSERT INTO MovementTypes (TypeCode, TypeName, AffectsStock, RequiresSupplier, RequiresDestination) VALUES
('COMPRA', 'COMPRA A PROVEEDOR', '+', 1, 0),
('ENTRADA_AJUSTE', 'AJUSTE DE ENTRADA', '+', 0, 0),
('SALIDA', 'SALIDA DE INVENTARIO', '-', 0, 0),
('SALIDA_AJUSTE', 'AJUSTE DE SALIDA', '-', 0, 0),
('TRANSFERENCIA', 'TRANSFERENCIA ENTRE SEDES', '0', 0, 1),
('DEVOLUCION', 'DEVOLUCIÓN A PROVEEDOR', '-', 1, 0);


------------------------------- TABLA DE CATEGORÍA DE ARTÍCULOS ---------------------------------------
CREATE TABLE ItemCategories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    CategoryCode VARCHAR(10) NOT NULL UNIQUE,
    CategoryName VARCHAR(100) NOT NULL,
    Description VARCHAR(255),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy INT,
    
    CONSTRAINT FK_ItemCategories_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId)
);

------------------------------- Tabla de Artículos ---------------------------------------
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

------------------------------- LEGACY ITEM STOCK BY LOCATION ---------------------------------------
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
------------------------------- LEGACY ITEM STOCK BY LOCATION ---------------------------------------

------------------------------- Tabla Maestra de Movimientos de Artículos ---------------------------------------
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

------------------------------- Tabla Detalles Movimientos de Artículos ---------------------------------------

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

------------------------------- Tabla de plantilla para stocks, por categoría (Tamaño) de sede ---------------------------------------


-- Plantillas de Stock por Categoría de Sede
CREATE TABLE ItemStockTemplates (
    TemplateId INT IDENTITY(1,1) PRIMARY KEY,
    LocationCategoryId INT NOT NULL,
    ItemId INT NOT NULL,
    MinimumStock DECIMAL(18,2) NOT NULL DEFAULT 0,
    MaximumStock DECIMAL(18,2) NOT NULL DEFAULT 0,
    ReorderPoint DECIMAL(18,2),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy INT,
    ModifiedDate DATETIME,
    ModifiedBy INT,
    
    CONSTRAINT FK_StockTemplate_Category FOREIGN KEY (LocationCategoryId) 
        REFERENCES LocationCategories(LocationCategoryId),
    CONSTRAINT FK_StockTemplate_Item FOREIGN KEY (ItemId) 
        REFERENCES Items(ItemId),
    CONSTRAINT FK_StockTemplate_CreatedBy FOREIGN KEY (CreatedBy) 
        REFERENCES Users(UserId),
    CONSTRAINT FK_StockTemplate_ModifiedBy FOREIGN KEY (ModifiedBy) 
        REFERENCES Users(UserId),
    CONSTRAINT UK_Template_Item_Category UNIQUE (LocationCategoryId, ItemId)
);