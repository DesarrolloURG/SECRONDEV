-- ============================================================
-- SCRIPT COMPLETO: MÓDULO KARDEX WAREHOUSE
-- ============================================================

-- ============================================================
-- 1. DROP de tablas 
-- ============================================================

IF OBJECT_ID('dbo.ItemMovementDetails', 'U') IS NOT NULL
    DROP TABLE dbo.ItemMovementDetails;
GO

IF OBJECT_ID('dbo.ItemMovementMaster', 'U') IS NOT NULL
    DROP TABLE dbo.ItemMovementMaster;
GO

IF OBJECT_ID('dbo.WarehouseReorderNotifications', 'U') IS NOT NULL
    DROP TABLE dbo.WarehouseReorderNotifications;
GO

IF OBJECT_ID('dbo.WarehouseManagerPermissions', 'U') IS NOT NULL
    DROP TABLE dbo.WarehouseManagerPermissions;
GO

IF OBJECT_ID('dbo.WarehousePermissions', 'U') IS NOT NULL
    DROP TABLE dbo.WarehousePermissions;
GO

IF OBJECT_ID('dbo.ItemWarehouseStock', 'U') IS NOT NULL
    DROP TABLE dbo.ItemWarehouseStock;
GO

IF OBJECT_ID('dbo.WarehouseManagers', 'U') IS NOT NULL
    DROP TABLE dbo.WarehouseManagers;
GO

-- ============================================================
-- 2. CREATE TABLE
-- ============================================================

--
-- MAESTRO DETALLE PARA LOS ENCARGADOS DE BODEGA
--
CREATE TABLE WarehouseManagers (
    WarehouseManagerId INT IDENTITY(1,1) PRIMARY KEY,
    WarehouseId INT NOT NULL,
    UserId INT NOT NULL,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy INT NOT NULL,
    ModifiedDate DATETIME NULL,
    ModifiedBy INT NULL,
    CONSTRAINT UQ_WarehouseManagers UNIQUE (WarehouseId, UserId),
    CONSTRAINT FK_WarehouseManagers_Warehouse FOREIGN KEY (WarehouseId) REFERENCES Warehouses(WarehouseId),
    CONSTRAINT FK_WarehouseManagers_User FOREIGN KEY (UserId) REFERENCES Users(UserId),
    CONSTRAINT FK_WarehouseManagers_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_WarehouseManagers_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserId)
);
GO

--
-- SALDOS Y LÍMITES DE STOCK POR BODEGA
--
CREATE TABLE ItemWarehouseStock (
    ItemWarehouseStockId INT IDENTITY(1,1) PRIMARY KEY,
    ItemId INT NOT NULL,
    WarehouseId INT NOT NULL,
    CurrentStock DECIMAL(18,2) NOT NULL DEFAULT 0,
    MovementCounter INT NOT NULL DEFAULT 0,
    LastMovementDate DATETIME NULL,
    MinimumStock DECIMAL(18,2) NULL DEFAULT 0,
    MaximumStock DECIMAL(18,2) NULL,
    ReorderPoint DECIMAL(18,2) NULL,
    CreatedDate DATETIME DEFAULT GETDATE(),
    ModifiedDate DATETIME NULL,
    CONSTRAINT UQ_ItemWarehouseStock UNIQUE (ItemId, WarehouseId),
    CONSTRAINT FK_ItemWarehouseStock_Item FOREIGN KEY (ItemId) REFERENCES Items(ItemId),
    CONSTRAINT FK_ItemWarehouseStock_Warehouse FOREIGN KEY (WarehouseId) REFERENCES Warehouses(WarehouseId)
);
GO

--
-- CATÁLOGO DE PERMISOS CONTEXTUALES DE BODEGA
--
CREATE TABLE WarehousePermissions (
    WarehousePermissionId INT IDENTITY(1,1) PRIMARY KEY,
    PermissionCode VARCHAR(50) NOT NULL UNIQUE,
    PermissionName VARCHAR(100) NOT NULL,
    Description VARCHAR(255) NULL,
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO

--
-- ASIGNACIÓN DE PERMISOS A LOS ENCARGADOS DE BODEGA
-- (qué encargado, en qué bodega, con qué permiso)
--
CREATE TABLE WarehouseManagerPermissions (
    WarehouseManagerPermissionId INT IDENTITY(1,1) PRIMARY KEY,
    WarehouseManagerId INT NOT NULL,
    WarehousePermissionId INT NOT NULL,
    MaxQuantityPerDispatch DECIMAL(18,2) NULL, -- NULL = sin límite (ej. bodega central no necesita tope)
    IsGranted BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy INT NOT NULL,
    CONSTRAINT UQ_WarehouseManagerPermissions UNIQUE (WarehouseManagerId, WarehousePermissionId),
    CONSTRAINT FK_WMP_Manager FOREIGN KEY (WarehouseManagerId) REFERENCES WarehouseManagers(WarehouseManagerId),
    CONSTRAINT FK_WMP_Permission FOREIGN KEY (WarehousePermissionId) REFERENCES WarehousePermissions(WarehousePermissionId),
    CONSTRAINT FK_WMP_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId)
);
GO

--
-- MAESTRO DE MOVIMIENTOS DE INVENTARIO (ingreso, salida, transferencia)
--
CREATE TABLE ItemMovementMaster(
    MovementMasterId INT IDENTITY(1,1) PRIMARY KEY,
    MovementNumber VARCHAR(30) NOT NULL UNIQUE,
    MovementDate DATETIME DEFAULT GETDATE(),
    MovementTypeId INT NOT NULL,
    WarehouseId INT NOT NULL,
    DestinationWarehouseId INT NULL,
    SupplierId INT NULL,
    ReferenceDocument VARCHAR(100) NULL,
    PurchaseDate DATE NULL,
    DestinationEmployeeId INT NULL,
    Remarks TEXT NULL,
    TotalAmount DECIMAL(15,2) DEFAULT 0,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy INT NOT NULL,
    ModifiedDate DATETIME NULL,
    ModifiedBy INT NULL,
    IsActive BIT DEFAULT 1,
    CONSTRAINT FK_IMM_MovementType FOREIGN KEY (MovementTypeId) REFERENCES MovementTypes(MovementTypeId),
    CONSTRAINT FK_IMM_Warehouse FOREIGN KEY (WarehouseId) REFERENCES Warehouses(WarehouseId),
    CONSTRAINT FK_IMM_DestinationWarehouse FOREIGN KEY (DestinationWarehouseId) REFERENCES Warehouses(WarehouseId),
    CONSTRAINT FK_IMM_Supplier FOREIGN KEY (SupplierId) REFERENCES Suppliers(SupplierId),
    CONSTRAINT FK_IMM_DestinationEmployee FOREIGN KEY (DestinationEmployeeId) REFERENCES Employees(EmployeeId),
    CONSTRAINT FK_IMM_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_IMM_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserId)
);
GO

--
-- DETALLE DE MOVIMIENTOS DE INVENTARIO
--
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
CREATE TABLE [dbo].[ItemMovementDetails](
	[MovementDetailId] [int] IDENTITY(1,1) NOT NULL,
	[MovementMasterId] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
	[Quantity] [decimal](18, 2) NOT NULL,
	[UnitCost] [decimal](15, 2) NULL,
	[TotalCost]  AS ([Quantity]*[UnitCost]),
	[StockBeforeMovement] [decimal](18, 2) NOT NULL,
	[StockAfterMovement] [decimal](18, 2) NOT NULL,
	[LotNumber] [varchar](50) NULL,
	[ExpiryDate] [date] NULL,
	[Remarks] [varchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[MovementDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY];
GO
ALTER TABLE [dbo].[ItemMovementDetails] ADD  DEFAULT ((0)) FOR [UnitCost];
GO
ALTER TABLE [dbo].[ItemMovementDetails]  WITH CHECK ADD  CONSTRAINT [FK_ItemMovementDetails_Item] FOREIGN KEY([ItemId])
REFERENCES [dbo].[Items] ([ItemId]);
GO
ALTER TABLE [dbo].[ItemMovementDetails] CHECK CONSTRAINT [FK_ItemMovementDetails_Item];
GO
ALTER TABLE [dbo].[ItemMovementDetails]  WITH CHECK ADD  CONSTRAINT [FK_ItemMovementDetails_Master] FOREIGN KEY([MovementMasterId])
REFERENCES [dbo].[ItemMovementMaster] ([MovementMasterId])
ON DELETE CASCADE;
GO
ALTER TABLE [dbo].[ItemMovementDetails] CHECK CONSTRAINT [FK_ItemMovementDetails_Master];
GO

--
-- NOTIFICACIONES DE PUNTO DE REORDEN
--
CREATE TABLE WarehouseReorderNotifications (
    WarehouseReorderNotificationId INT IDENTITY(1,1) PRIMARY KEY,
    WarehouseId INT NOT NULL,
    ItemId INT NOT NULL,
    NotifiedDate DATETIME NOT NULL DEFAULT GETDATE(),
    ResolvedDate DATETIME NULL,
    CONSTRAINT FK_WRN_Warehouse FOREIGN KEY (WarehouseId) REFERENCES Warehouses(WarehouseId),
    CONSTRAINT FK_WRN_Item FOREIGN KEY (ItemId) REFERENCES Items(ItemId)
);
GO
CREATE UNIQUE INDEX UQ_WRN_Active ON WarehouseReorderNotifications (WarehouseId, ItemId) WHERE ResolvedDate IS NULL;
GO

-- ============================================================
-- 3. INSERT 
-- ============================================================

INSERT INTO WarehousePermissions (PermissionCode, PermissionName, Description)
VALUES
('REGISTRO', 'REGISTRO', 'PERMITE REGISTRAR INGRESOS DE ARTICULOS EN LA BODEGA ASIGNADA'),
('DESPACHO_EMPLEADO', 'DESPACHO A EMPLEADO', 'PERMITE DESPACHAR ARTICULOS A UN EMPLEADO DESDE LA BODEGA ASIGNADA'),
('DESPACHO_BODEGA', 'DESPACHO A BODEGA', 'PERMITE TRANSFERIR ARTICULOS DESDE LA BODEGA CENTRAL HACIA OTRA BODEGA'),
('ADMIN_BODEGA', 'ADMINISTRADOR DE BODEGA', 'PERMITE ADMINISTRAR TODOS LOS ASPECTOS DE LA BODEGA ASIGNADA, INCLUYENDO GESTIONAR PERMISOS DE OTROS USUARIOS EN ELLA');
GO

INSERT INTO Permissions (PermissionCode, PermissionName, Description, ModuleName, ActionType)
SELECT * FROM (VALUES 
('KDX_044', 'KARDEX_WAREHOUSE_READ', 'PERMITE VISUALIZAR EL INVENTARIO DE LAS BODEGAS ASIGNADAS.', 'KARDEX', 'READ'),
('KDX_045', 'KARDEX_WAREHOUSE_REGISTER', 'PERMITE REGISTRAR INGRESOS DE ARTICULOS EN LA BODEGA CENTRAL.', 'KARDEX', 'CREATE'),
('KDX_046', 'KARDEX_WAREHOUSE_DISPATCH', 'PERMITE DESPACHAR ARTICULOS DESDE UNA BODEGA ASIGNADA.', 'KARDEX', 'CREATE'),
('KDX_047', 'KARDEX_WAREHOUSE_UPDATE', 'PERMITE EDITAR REGISTROS EN UN FORMULARIO.', 'KARDEX', 'UPDATE'),
('KDX_048', 'KARDEX_WAREHOUSE_ADMIN', 'PERMISOS PARA ADMINISTRAR TODA LA PARTE DE INVENTARIO DE BODEGAS.', 'KARDEX', 'ADMIN')
) AS nuevos(PermissionCode, PermissionName, Description, ModuleName, ActionType)
WHERE NOT EXISTS (SELECT 1 FROM Permissions p WHERE p.PermissionCode = nuevos.PermissionCode);
GO

-- ============================================================
-- 4. MIGRACIÓN — datos del esquema anterior (ItemStockByLocation → ItemWarehouseStock)
-- ============================================================

INSERT INTO ItemWarehouseStock (ItemId, WarehouseId, CurrentStock, MovementCounter, LastMovementDate, MinimumStock, MaximumStock, ReorderPoint)
SELECT c.ItemId, b.WarehouseId, c.CurrentStock, 0, c.LastMovementDate,
       c.MinimumStock, c.MaximumStock, d.ReorderPoint
FROM Locations a, Warehouses b, ItemStockByLocation c, Items d
WHERE a.LocationId = b.LocationId
  AND a.LocationId = c.LocationId
  AND c.ItemId = d.ItemId;
GO

-- ============================================================
-- 5. CONFIGURACIÓN INICIAL — encargados y permisos para SUPERADMIN
-- ============================================================

-- 5.1 Asignar todas las bodegas a cada SUPERADMIN, solo si no la tiene ya activa
INSERT INTO WarehouseManagers (WarehouseId, UserId, IsActive, CreatedDate, CreatedBy)
SELECT DISTINCT c.WarehouseId, a.UserId, 1, GETDATE(), a.UserId
FROM Users a, Roles b, Warehouses c
WHERE a.RoleId = b.RoleId
  AND b.RoleName IN ('SUPERADMIN')
  AND NOT EXISTS (
      SELECT 1 FROM WarehouseManagers wm
      WHERE wm.WarehouseId = c.WarehouseId
        AND wm.UserId = a.UserId
        AND wm.IsActive = 1
  );
GO

-- Reactivar las que ya existían pero estaban inactivas
UPDATE wm
   SET IsActive = 1
FROM WarehouseManagers wm
INNER JOIN Users a ON a.UserId = wm.UserId
INNER JOIN Roles b ON b.RoleId = a.RoleId
WHERE b.RoleName IN ('SUPERADMIN')
  AND wm.IsActive = 0;
GO

-- 5.2 DESPACHO_BODEGA
INSERT INTO WarehouseManagerPermissions (WarehouseManagerId, WarehousePermissionId, IsGranted, CreatedDate, CreatedBy)
SELECT wm.WarehouseManagerId,
       wp.WarehousePermissionId,
       1,
       GETDATE(),
       wm.UserId
FROM WarehouseManagers wm
CROSS JOIN WarehousePermissions wp
WHERE wp.PermissionCode = 'DESPACHO_BODEGA'
  AND NOT EXISTS (
      SELECT 1 FROM WarehouseManagerPermissions wmp
      WHERE wmp.WarehouseManagerId = wm.WarehouseManagerId
        AND wmp.WarehousePermissionId = wp.WarehousePermissionId
  );
GO

-- 5.3 DESPACHO_EMPLEADO
INSERT INTO WarehouseManagerPermissions (WarehouseManagerId, WarehousePermissionId, IsGranted, CreatedDate, CreatedBy)
SELECT wm.WarehouseManagerId,
       wp.WarehousePermissionId,
       1,
       GETDATE(),
       wm.UserId
FROM WarehouseManagers wm
CROSS JOIN WarehousePermissions wp
WHERE wp.PermissionCode = 'DESPACHO_EMPLEADO'
  AND NOT EXISTS (
      SELECT 1 FROM WarehouseManagerPermissions wmp
      WHERE wmp.WarehouseManagerId = wm.WarehouseManagerId
        AND wmp.WarehousePermissionId = wp.WarehousePermissionId
  );
GO

-- 5.4 ADMIN_BODEGA
INSERT INTO WarehouseManagerPermissions (WarehouseManagerId, WarehousePermissionId, IsGranted, CreatedDate, CreatedBy)
SELECT wm.WarehouseManagerId,
       wp.WarehousePermissionId,
       1,
       GETDATE(),
       wm.UserId
FROM WarehouseManagers wm
CROSS JOIN WarehousePermissions wp
WHERE wp.PermissionCode = 'ADMIN_BODEGA'
  AND NOT EXISTS (
      SELECT 1 FROM WarehouseManagerPermissions wmp
      WHERE wmp.WarehouseManagerId = wm.WarehouseManagerId
        AND wmp.WarehousePermissionId = wp.WarehousePermissionId
  );
GO

-- 5.5 REGISTRO
INSERT INTO WarehouseManagerPermissions (WarehouseManagerId, WarehousePermissionId, IsGranted, CreatedDate, CreatedBy)
SELECT wm.WarehouseManagerId,
       wp.WarehousePermissionId,
       1,
       GETDATE(),
       wm.UserId
FROM WarehouseManagers wm
CROSS JOIN WarehousePermissions wp
WHERE wp.PermissionCode = 'REGISTRO'
  AND NOT EXISTS (
      SELECT 1 FROM WarehouseManagerPermissions wmp
      WHERE wmp.WarehouseManagerId = wm.WarehouseManagerId
        AND wmp.WarehousePermissionId = wp.WarehousePermissionId
  );
GO