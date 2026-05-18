-- ============================================================
-- SECRON - Drop Módulo de Activos Fijos si existe 
-- para que tome siempre la ultima versión
-- ============================================================

-- -------------------------------------------------------
-- 1. ELIMINAR FOREIGN KEYS DE TABLAS DEPENDIENTES
-- -------------------------------------------------------
ALTER TABLE FixedAssetAttributeValues      DROP CONSTRAINT FK_FAAV_Asset;
ALTER TABLE FixedAssetAttributeValues      DROP CONSTRAINT FK_FAAV_AttrDef;
ALTER TABLE AccountingEntryFixedAssets     DROP CONSTRAINT FK_AEFA_Entry;
ALTER TABLE AccountingEntryFixedAssets     DROP CONSTRAINT FK_AEFA_Asset;
ALTER TABLE FixedAssetTransfers            DROP CONSTRAINT FK_FAT_Asset;
ALTER TABLE FixedAssetTransfers            DROP CONSTRAINT FK_FAT_FromWarehouse;
ALTER TABLE FixedAssetTransfers            DROP CONSTRAINT FK_FAT_ToWarehouse;
ALTER TABLE FixedAssetTransfers            DROP CONSTRAINT FK_FAT_FromEmployee;
ALTER TABLE FixedAssetTransfers            DROP CONSTRAINT FK_FAT_ToEmployee;
ALTER TABLE FixedAssetTransfers            DROP CONSTRAINT FK_FAT_Status;
ALTER TABLE FixedAssets                    DROP CONSTRAINT FK_FA_Category;
ALTER TABLE FixedAssets                    DROP CONSTRAINT FK_FA_Warehouse;
ALTER TABLE FixedAssets                    DROP CONSTRAINT FK_FA_Employee;
ALTER TABLE FixedAssets                    DROP CONSTRAINT FK_FA_Supplier;
ALTER TABLE FixedAssetAttributeDefinitions DROP CONSTRAINT FK_FAAD_Category;
ALTER TABLE FixedAssetCategories           DROP CONSTRAINT FK_FAC_AccountsDep;
ALTER TABLE FixedAssetCategories           DROP CONSTRAINT FK_FAC_AccountsExp;
GO

-- -------------------------------------------------------
-- 2. ELIMINAR ÍNDICES
-- -------------------------------------------------------
DROP INDEX IX_FA_Category   ON FixedAssets;
DROP INDEX IX_FA_Warehouse  ON FixedAssets;
DROP INDEX IX_FA_Employee   ON FixedAssets;
DROP INDEX IX_FA_Status     ON FixedAssets;
DROP INDEX IX_FA_Supplier   ON FixedAssets;

DROP INDEX IX_FAAV_Asset    ON FixedAssetAttributeValues;
DROP INDEX IX_FAAV_AttrDef  ON FixedAssetAttributeValues;

DROP INDEX IX_AEFA_Entry    ON AccountingEntryFixedAssets;
DROP INDEX IX_AEFA_Asset    ON AccountingEntryFixedAssets;
DROP INDEX IX_AEFA_Period   ON AccountingEntryFixedAssets;

DROP INDEX IX_FAT_Asset     ON FixedAssetTransfers;
DROP INDEX IX_FAT_Status    ON FixedAssetTransfers;
DROP INDEX IX_FAT_Date      ON FixedAssetTransfers;
GO

-- -------------------------------------------------------
-- 3. ELIMINAR TABLAS (orden por dependencias)
-- -------------------------------------------------------
DROP TABLE FixedAssetAttributeValues;
DROP TABLE AccountingEntryFixedAssets;
DROP TABLE FixedAssetTransfers;
DROP TABLE FixedAssetTransferStatus;
DROP TABLE FixedAssets;
DROP TABLE FixedAssetAttributeDefinitions;
DROP TABLE FixedAssetCategories;
GO

-- -------------------------------------------------------
-- 4. ELIMINAR STORED PROCEDURES
-- -------------------------------------------------------
DROP PROCEDURE IF EXISTS SP_FixedAssetCategories_Insert;
DROP PROCEDURE IF EXISTS SP_FixedAssetCategories_Update;
DROP PROCEDURE IF EXISTS SP_FixedAssetAttributeDefinitions_Insert;
DROP PROCEDURE IF EXISTS SP_FixedAssetAttributeDefinitions_Update;
GO



-- ============================================================
-- SECRON - Módulo de Control de Activos Fijos
-- ============================================================

-- -------------------------------------------------------
-- 1. CATEGORÍAS DE ACTIVOS
-- -------------------------------------------------------
CREATE TABLE [FixedAssetCategories](
    [AssetCategoryId]       [int] IDENTITY(1,1) NOT NULL,
    [CategoryCode]          [varchar](20) NOT NULL,
    [CategoryName]          [varchar](100) NOT NULL,
    [Description]           [varchar](255) NULL,
    [DepreciationMethod]    [varchar](30) NOT NULL
        CONSTRAINT DF_FAC_Method DEFAULT 'LINEA_RECTA',
    [DepreciationYears]     [decimal](4,1) NOT NULL,
    [AccountAccumDepId]     [int] NOT NULL,
    [AccountExpenseId]      [int] NOT NULL,
    [IsActive]              [bit] NULL CONSTRAINT DF_FAC_Active DEFAULT 1,
    [CreatedDate]           [datetime] NULL CONSTRAINT DF_FAC_Created DEFAULT GETDATE(),
    [CreatedBy]             [int] NULL,
    [ModifiedDate]          [datetime] NULL,
    [ModifiedBy]            [int] NULL,
    IsTangible              bit     NOT NULL CONSTRAINT DF_FAC_IsTangible DEFAULT 1,
    CONSTRAINT PK_FixedAssetCategories PRIMARY KEY CLUSTERED ([AssetCategoryId] ASC),
    CONSTRAINT UK_FAC_Code UNIQUE ([CategoryCode]),
    CONSTRAINT FK_FAC_AccountsDep FOREIGN KEY ([AccountAccumDepId])
        REFERENCES [Accounts]([AccountId]),
    CONSTRAINT FK_FAC_AccountsExp FOREIGN KEY ([AccountExpenseId])
        REFERENCES [Accounts]([AccountId])
);
GO


-- -------------------------------------------------------
-- 2. DEFINICIÓN DE ATRIBUTOS POR CATEGORÍA (EAV)
-- -------------------------------------------------------
CREATE TABLE [dbo].[FixedAssetAttributeDefinitions](
    [AttributeDefId]    [int] IDENTITY(1,1) NOT NULL,
    [AssetCategoryId]   [int] NOT NULL,
    [AttributeKey]      [varchar](50) NOT NULL,
    [AttributeLabel]    [varchar](100) NOT NULL,
    [DataType]          [varchar](20) NOT NULL
        CONSTRAINT DF_FAAD_DataType DEFAULT 'TEXTO',
    [IsRequired]        [bit] NULL CONSTRAINT DF_FAAD_Required DEFAULT 0,
    [IsActive]          [bit] NULL CONSTRAINT DF_FAAD_Active DEFAULT 1,
    CONSTRAINT PK_FixedAssetAttributeDefinitions PRIMARY KEY ([AttributeDefId]),
    CONSTRAINT UK_FAAD_CategoryKey UNIQUE ([AssetCategoryId], [AttributeKey]),
    CONSTRAINT FK_FAAD_Category FOREIGN KEY ([AssetCategoryId])
        REFERENCES [dbo].[FixedAssetCategories]([AssetCategoryId])
);
GO

-- -------------------------------------------------------
-- 3. CATÁLOGO MAESTRO DE ACTIVOS FIJOS
-- -------------------------------------------------------
CREATE TABLE [dbo].[FixedAssets](
    [AssetId]               [int] IDENTITY(1,1) NOT NULL,
    [AssetCode]             [varchar](30) NOT NULL,
    [AssetName]             [varchar](150) NOT NULL,
    [Description]           [varchar](500) NULL,
    [AssetCategoryId]       [int] NOT NULL,
    [Brand]                 [varchar](100) NULL,
    [Model]                 [varchar](100) NULL,
	[Serial]                 [varchar](100) NULL,
    [PurchaseDate]          [date] NULL,
    [PurchaseValue]         [decimal](18,2) NOT NULL,
	[ResidualValue]         [decimal](18,2) NOT NULL CONSTRAINT DF_FAC_Residual DEFAULT 0,
    [InvoiceNumber]         [varchar](50) NULL,
    [SupplierId]            [int] NULL,
    [WarrantyDocumentPath]  [varchar](500) NULL,
    [WarrantyExpirationDate][date] NULL,
    -- Depreciación
    [DepreciationStartDate] [date] NULL,
    [ResidualValueAct]      [decimal](18,2) NOT NULL
        CONSTRAINT DF_FA_Residual DEFAULT 0,
    -- Asignación actual
    [CurrentWarehouseId]    [int] NULL,
    [AssignedToEmployeeId]     [int] NULL,
    -- Estado
    [AssetStatus]           [varchar](30) NOT NULL
        CONSTRAINT DF_FA_Status DEFAULT 'ACTIVE',
    [DisposalDate]          [date] NULL,
    [DisposalReason]        [varchar](255) NULL,
    [DisposalValue]         [decimal](18,2) NULL,
    [Notes]                 [varchar](1000) NULL,
    [IsActive]              [bit] NULL CONSTRAINT DF_FA_Active DEFAULT 1,
    [CreatedDate]           [datetime] NULL CONSTRAINT DF_FA_Created DEFAULT GETDATE(),
    [CreatedBy]             [int] NULL,
    [ModifiedDate]          [datetime] NULL,
    [ModifiedBy]            [int] NULL,
    CONSTRAINT PK_FixedAssets PRIMARY KEY CLUSTERED ([AssetId] ASC),
    CONSTRAINT UK_FA_Code UNIQUE ([AssetCode]),
    CONSTRAINT FK_FA_Category FOREIGN KEY ([AssetCategoryId])
        REFERENCES [dbo].[FixedAssetCategories]([AssetCategoryId]),
    CONSTRAINT FK_FA_Warehouse FOREIGN KEY ([CurrentWarehouseId])
        REFERENCES [dbo].[Warehouses]([WarehouseId]),
    CONSTRAINT FK_FA_Employee FOREIGN KEY ([AssignedToEmployeeId])
        REFERENCES [dbo].[Employees]([EmployeeId]),
    CONSTRAINT FK_FA_Supplier FOREIGN KEY ([SupplierId])
        REFERENCES [dbo].[Suppliers]([SupplierId])
);
GO

-- -------------------------------------------------------
-- 4. VALORES EAV (atributos específicos por activo)
-- -------------------------------------------------------
CREATE TABLE [dbo].[FixedAssetAttributeValues](
    [AttributeValueId]  [int] IDENTITY(1,1) NOT NULL,
    [AssetId]           [int] NOT NULL,
    [AttributeDefId]    [int] NOT NULL,
    [Value]             [nvarchar](500) NULL,
    [CreatedDate]       [datetime] NULL CONSTRAINT DF_FAAV_Created DEFAULT GETDATE(),
    [CreatedBy]         [int] NULL,
    [ModifiedDate]      [datetime] NULL,
    [ModifiedBy]        [int] NULL,
    CONSTRAINT PK_FixedAssetAttributeValues PRIMARY KEY ([AttributeValueId]),
    CONSTRAINT UK_FAAV_AssetAttr UNIQUE ([AssetId], [AttributeDefId]),
    CONSTRAINT FK_FAAV_Asset FOREIGN KEY ([AssetId])
        REFERENCES [dbo].[FixedAssets]([AssetId]),
    CONSTRAINT FK_FAAV_AttrDef FOREIGN KEY ([AttributeDefId])
        REFERENCES [dbo].[FixedAssetAttributeDefinitions]([AttributeDefId])
);
GO

-- -------------------------------------------------------
-- 5. RELACIÓN ACTIVO - PARTIDA CONTABLE
-- -------------------------------------------------------
CREATE TABLE [dbo].[AccountingEntryFixedAssets](
    [EntryAssetId]      [int] IDENTITY(1,1) NOT NULL,
    [EntryMasterId]     [int] NOT NULL,
    [AssetId]           [int] NOT NULL,
    [MovementType]      [varchar](30) NOT NULL,
    -- 'PURCHASE','DEPRECIATION','DISPOSAL','TRANSFER'
    [Period]            [varchar](7) NULL, -- 'YYYY-MM', solo para DEPRECIATION
    CONSTRAINT PK_AEFA PRIMARY KEY ([EntryAssetId]),
    CONSTRAINT FK_AEFA_Entry FOREIGN KEY ([EntryMasterId])
        REFERENCES [dbo].[AccountingEntryMaster]([EntryMasterId]),
    CONSTRAINT FK_AEFA_Asset FOREIGN KEY ([AssetId])
        REFERENCES [dbo].[FixedAssets]([AssetId])
);
GO

-- -------------------------------------------------------
-- 6. STATUS DE TRASLADOS
-- -------------------------------------------------------
CREATE TABLE [dbo].[FixedAssetTransferStatus](
    [TransferStatusId]  [int] IDENTITY(1,1) NOT NULL,
    [StatusCode]        [varchar](20) NOT NULL,
    [StatusName]        [varchar](50) NOT NULL,
    CONSTRAINT PK_FATS PRIMARY KEY ([TransferStatusId]),
    CONSTRAINT UK_FATS_Code UNIQUE ([StatusCode])
);
GO

INSERT INTO [dbo].[FixedAssetTransferStatus] ([StatusCode],[StatusName]) VALUES
('PENDING',   'Pendiente'),
('APPROVED',  'Aprobado'),
('REJECTED',  'Rechazado'),
('COMPLETED', 'Completado');
GO

-- -------------------------------------------------------
-- 7. TRASLADOS DE ACTIVOS
-- -------------------------------------------------------
CREATE TABLE [dbo].[FixedAssetTransfers](
    [TransferId]            [int] IDENTITY(1,1) NOT NULL,
    [TransferCode]          [varchar](30) NOT NULL,
    [AssetId]               [int] NOT NULL,
    [TransferDate]          [date] NOT NULL,
    -- Origen
    [FromWarehouseId]       [int] NULL,
    [FromEmployeeId]        [int] NULL,
    -- Destino
    [ToWarehouseId]         [int] NULL,
    [ToEmployeeId]          [int] NULL,
    -- Control
    [TransferStatusId]      [int] NOT NULL,
    [Reason]                [varchar](500) NULL,
    [ApprovedByUserId]      [int] NULL,   -- Sin FK por ahora, flujo pendiente
    [ApprovedDate]          [datetime] NULL,
    [CompletedDate]         [datetime] NULL,
    [CreatedDate]           [datetime] NULL CONSTRAINT DF_FAT_Created DEFAULT GETDATE(),
    [CreatedBy]             [int] NULL,
    [ModifiedDate]          [datetime] NULL,
    [ModifiedBy]            [int] NULL,
    CONSTRAINT PK_FixedAssetTransfers PRIMARY KEY CLUSTERED ([TransferId] ASC),
    CONSTRAINT UK_FAT_Code UNIQUE ([TransferCode]),
    CONSTRAINT FK_FAT_Asset FOREIGN KEY ([AssetId])
        REFERENCES [dbo].[FixedAssets]([AssetId]),
    CONSTRAINT FK_FAT_FromWarehouse FOREIGN KEY ([FromWarehouseId])
        REFERENCES [dbo].[Warehouses]([WarehouseId]),
    CONSTRAINT FK_FAT_ToWarehouse FOREIGN KEY ([ToWarehouseId])
        REFERENCES [dbo].[Warehouses]([WarehouseId]),
    CONSTRAINT FK_FAT_FromEmployee FOREIGN KEY ([FromEmployeeId])
        REFERENCES [dbo].[Employees]([EmployeeId]),
    CONSTRAINT FK_FAT_ToEmployee FOREIGN KEY ([ToEmployeeId])
        REFERENCES [dbo].[Employees]([EmployeeId]),
    CONSTRAINT FK_FAT_Status FOREIGN KEY ([TransferStatusId])
        REFERENCES [dbo].[FixedAssetTransferStatus]([TransferStatusId])
);
GO

-- ============================================================
-- ÍNDICES PARA BÚSQUEDAS
-- ============================================================

-- FixedAssets
CREATE INDEX IX_FA_Category    ON [dbo].[FixedAssets]([AssetCategoryId]);
CREATE INDEX IX_FA_Warehouse   ON [dbo].[FixedAssets]([CurrentWarehouseId]);
CREATE INDEX IX_FA_Employee    ON [dbo].[FixedAssets]([AssignedToEmployeeId]);
CREATE INDEX IX_FA_Status      ON [dbo].[FixedAssets]([AssetStatus]);
CREATE INDEX IX_FA_Supplier    ON [dbo].[FixedAssets]([SupplierId]);

-- FixedAssetAttributeValues
CREATE INDEX IX_FAAV_Asset     ON [dbo].[FixedAssetAttributeValues]([AssetId]);
CREATE INDEX IX_FAAV_AttrDef   ON [dbo].[FixedAssetAttributeValues]([AttributeDefId]);

-- AccountingEntryFixedAssets
CREATE INDEX IX_AEFA_Entry     ON [dbo].[AccountingEntryFixedAssets]([EntryMasterId]);
CREATE INDEX IX_AEFA_Asset     ON [dbo].[AccountingEntryFixedAssets]([AssetId]);
CREATE INDEX IX_AEFA_Period    ON [dbo].[AccountingEntryFixedAssets]([Period]);

-- FixedAssetTransfers
CREATE INDEX IX_FAT_Asset      ON [dbo].[FixedAssetTransfers]([AssetId]);
CREATE INDEX IX_FAT_Status     ON [dbo].[FixedAssetTransfers]([TransferStatusId]);
CREATE INDEX IX_FAT_Date       ON [dbo].[FixedAssetTransfers]([TransferDate]);
GO


-- Categorías
INSERT INTO [dbo].[FixedAssetCategories]
    ([CategoryCode],[CategoryName],[DepreciationYears],[DepreciationMethod],
     [AccountAccumDepId],[AccountExpenseId], IsTangible)
VALUES
    ('VEHICLE',   'Vehículos',          5,  'LINEA_RECTA', 416, 301, 1),
    ('COMPUTER',  'Equipo de Cómputo',  3,  'LINEA_RECTA', 417, 302, 1);
GO

-- Atributos para VEHICLE (CategoryId = 1)
INSERT INTO [dbo].[FixedAssetAttributeDefinitions]
    ([AssetCategoryId],[AttributeKey],[AttributeLabel],[DataType],[IsRequired])
VALUES
    (1,'VIN',       'Número VIN',       'TEXT',    1),
    (1,'PLATE',     'Placa',            'TEXT',    1),
    (1,'YEAR',      'Año',              'NUMBER',  1),
    (1,'COLOR',     'Color',            'TEXT',    0),
    (1,'FUEL_TYPE', 'Tipo de Combustible','TEXT',  0),
    (1,'ENGINE_CC', 'Cilindraje (cc)',  'NUMBER',  0);
GO

-- Atributos para COMPUTER (CategoryId = 2)
INSERT INTO [dbo].[FixedAssetAttributeDefinitions]
    ([AssetCategoryId],[AttributeKey],[AttributeLabel],[DataType],[IsRequired])
VALUES
    (2,'SERIAL',      'Número de Serie', 'TEXT',   1),
    (2,'PROCESSOR',   'Procesador',      'TEXT',   1),
    (2,'MAC_ADDRESS', 'MAC Address',      'TEXT',   1),
    (2,'STORAGE_GB',  'Almacenamiento (GB)','NUMBER',1),
    (2,'OS',          'Sistema Operativo','TEXT',   0),
    (2,'RAM_GB',      'RAM (GB)',         'NUMBER', 1);
GO

