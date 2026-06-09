-- ============================================================
-- SECRON - Script Módulo de Activos Fijos
-- ============================================================

-- ============================================================
-- SECCIÓN 1: ELIMINAR OBJETOS EXISTENTES
-- ============================================================

-- 1.1 Deshabilitar todas las FK para evitar errores de dependencia
EXEC sp_msforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';
GO

-- 1.2 Eliminar tablas dependientes (detalle primero)
IF OBJECT_ID('dbo.AccountingEntryFixedAssetsDetail', 'U') IS NOT NULL DROP TABLE dbo.AccountingEntryFixedAssetsDetail;
IF OBJECT_ID('dbo.AccountingEntryFixedAssets',       'U') IS NOT NULL DROP TABLE dbo.AccountingEntryFixedAssets;
IF OBJECT_ID('dbo.AccountingFixedAssetStatus',       'U') IS NOT NULL DROP TABLE dbo.AccountingFixedAssetStatus;
IF OBJECT_ID('dbo.FixedAssetTransferDetails',        'U') IS NOT NULL DROP TABLE dbo.FixedAssetTransferDetails;
IF OBJECT_ID('dbo.FixedAssetTransfers',              'U') IS NOT NULL DROP TABLE dbo.FixedAssetTransfers;
IF OBJECT_ID('dbo.FixedAssetTransferStatusTransitions','U') IS NOT NULL DROP TABLE dbo.FixedAssetTransferStatusTransitions;
IF OBJECT_ID('dbo.FixedAssetTransferStatus',         'U') IS NOT NULL DROP TABLE dbo.FixedAssetTransferStatus;
IF OBJECT_ID('dbo.FixedAssetAttributeValues',        'U') IS NOT NULL DROP TABLE dbo.FixedAssetAttributeValues;
IF OBJECT_ID('dbo.FixedAssets',                      'U') IS NOT NULL DROP TABLE dbo.FixedAssets;
IF OBJECT_ID('dbo.FixedAssetAttributeDefinitions',   'U') IS NOT NULL DROP TABLE dbo.FixedAssetAttributeDefinitions;
IF OBJECT_ID('dbo.FixedAssetCategories',             'U') IS NOT NULL DROP TABLE dbo.FixedAssetCategories;
GO

-- 1.3 Eliminar Stored Procedures
DROP PROCEDURE IF EXISTS SP_FixedAssetCategories_Insert;
DROP PROCEDURE IF EXISTS SP_FixedAssetCategories_Update;
DROP PROCEDURE IF EXISTS SP_FixedAssetAttributeDefinitions_Insert;
DROP PROCEDURE IF EXISTS SP_FixedAssetAttributeDefinitions_Update;
DROP PROCEDURE IF EXISTS SP_FixedAssetTransferStatus_Select;
DROP PROCEDURE IF EXISTS SP_FixedAssetTransferStatus_Insert;
DROP PROCEDURE IF EXISTS SP_FixedAssetTransferStatus_Update;
DROP PROCEDURE IF EXISTS SP_FixedAssetTransferStatus_Inactive;
DROP PROCEDURE IF EXISTS SP_FixedAssetTransferStatusTransitions_Select;
DROP PROCEDURE IF EXISTS SP_FixedAssetTransferStatusTransitions_Insert;
DROP PROCEDURE IF EXISTS SP_FixedAssetTransferStatusTransitions_Delete;
DROP PROCEDURE IF EXISTS SP_FixedAssetMovements_Select;
DROP PROCEDURE IF EXISTS SP_FixedAssetMovements_Insert;
DROP PROCEDURE IF EXISTS SP_FixedAssetMovements_Update;
DROP PROCEDURE IF EXISTS SP_FixedAssetMovements_Inactive;
GO

-- 1.4 Eliminar Views
DROP VIEW IF EXISTS V_FixedAssetTransferStatus;
DROP VIEW IF EXISTS V_FixedAssetTransferStatusTransitions;
DROP VIEW IF EXISTS V_FixedAssetMovements;
GO

-- 1.5 Rehabilitar todas las FK
EXEC sp_msforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL';
GO

-- ============================================================
-- SECCIÓN 2: CREAR TABLAS
-- ============================================================

-- -------------------------------------------------------
-- 1. CATEGORÍAS DE ACTIVOS
-- -------------------------------------------------------
CREATE TABLE [dbo].[FixedAssetCategories] (
    [AssetCategoryId]    [int]           IDENTITY(1,1) NOT NULL,
    [CategoryCode]       [varchar](20)   NOT NULL,
    [CategoryName]       [varchar](100)  NOT NULL,
    [Description]        [varchar](255)  NULL,
    [DepreciationMethod] [varchar](30)   NOT NULL CONSTRAINT DF_FAC_Method DEFAULT 'LINEA_RECTA',
    [DepreciationYears]  [decimal](4,1)  NOT NULL,
    [AccountAccumDepId]  [int]           NOT NULL,
    [AccountExpenseId]   [int]           NOT NULL,
    [IsActive]           [bit]           NULL CONSTRAINT DF_FAC_Active DEFAULT 1,
    [CreatedDate]        [datetime]      NULL CONSTRAINT DF_FAC_Created DEFAULT GETDATE(),
    [CreatedBy]          [int]           NULL,
    [ModifiedDate]       [datetime]      NULL,
    [ModifiedBy]         [int]           NULL,
    [IsTangible]         [bit]           NOT NULL CONSTRAINT DF_FAC_IsTangible DEFAULT 1,
    CONSTRAINT PK_FixedAssetCategories PRIMARY KEY CLUSTERED ([AssetCategoryId] ASC),
    CONSTRAINT UK_FAC_Code UNIQUE ([CategoryCode]),
    CONSTRAINT FK_FAC_AccountsDep FOREIGN KEY ([AccountAccumDepId]) REFERENCES [dbo].[Accounts]([AccountId]),
    CONSTRAINT FK_FAC_AccountsExp FOREIGN KEY ([AccountExpenseId])  REFERENCES [dbo].[Accounts]([AccountId])
);
GO

-- -------------------------------------------------------
-- 2. DEFINICIÓN DE ATRIBUTOS POR CATEGORÍA (EAV)
-- -------------------------------------------------------
CREATE TABLE [dbo].[FixedAssetAttributeDefinitions] (
    [AttributeDefId]  [int]           IDENTITY(1,1) NOT NULL,
    [AssetCategoryId] [int]           NOT NULL,
    [AttributeKey]    [varchar](50)   NOT NULL,
    [AttributeLabel]  [varchar](100)  NOT NULL,
    [DataType]        [varchar](20)   NOT NULL CONSTRAINT DF_FAAD_DataType DEFAULT 'TEXTO',
    [IsRequired]      [bit]           NULL CONSTRAINT DF_FAAD_Required DEFAULT 0,
    [IsActive]        [bit]           NULL CONSTRAINT DF_FAAD_Active DEFAULT 1,
    [IsSystem]        [bit]           NOT NULL CONSTRAINT DF_FAAD_IsSystem DEFAULT 0,
    CONSTRAINT PK_FixedAssetAttributeDefinitions PRIMARY KEY ([AttributeDefId]),
    CONSTRAINT UK_FAAD_CategoryKey UNIQUE ([AssetCategoryId], [AttributeKey]),
    CONSTRAINT FK_FAAD_Category FOREIGN KEY ([AssetCategoryId]) REFERENCES [dbo].[FixedAssetCategories]([AssetCategoryId])
);
GO

-- -------------------------------------------------------
-- 3. CATÁLOGO MAESTRO DE ACTIVOS FIJOS
-- -------------------------------------------------------
CREATE TABLE [dbo].[FixedAssets] (
    [AssetId]               [int]           IDENTITY(1,1) NOT NULL,
    [AssetCode]             [varchar](50)   NOT NULL,
    [AssetName]             [varchar](150)  NOT NULL,
    [Description]           [varchar](500)  NULL,
    [AssetCategoryId]       [int]           NOT NULL,
    [PurchaseDate]          [date]          NULL,
    [PurchaseValue]         [decimal](18,2) NOT NULL,
    [ResidualValue]         [decimal](18,2) NOT NULL CONSTRAINT DF_FAC_Residual DEFAULT 0,
    [InvoiceNumber]         [varchar](50)   NULL,
    [SupplierId]            [int]           NULL,
    [WarrantyDocumentPath]  [varchar](500)  NULL,
    [WarrantyExpirationDate][date]          NULL,
    [DepreciationStartDate] [date]          NULL,
    [ResidualValueAct]      [decimal](18,2) NOT NULL CONSTRAINT DF_FA_Residual DEFAULT 0,
    [CurrentWarehouseId]    [int]           NULL,
    [AssignedToEmployeeId]  [int]           NULL,
    [AssetStatus]           [varchar](30)   NOT NULL CONSTRAINT DF_FA_Status DEFAULT 'ACTIVO',
    [DisposalDate]          [date]          NULL,
    [DisposalReason]        [varchar](255)  NULL,
    [DisposalValue]         [decimal](18,2) NULL,
    [Notes]                 [varchar](1000) NULL,
    [IsActive]              [bit]           NULL CONSTRAINT DF_FA_Active DEFAULT 1,
    [CreatedDate]           [datetime]      NULL CONSTRAINT DF_FA_Created DEFAULT GETDATE(),
    [CreatedBy]             [int]           NULL,
    [ModifiedDate]          [datetime]      NULL,
    [ModifiedBy]            [int]           NULL,
    CONSTRAINT PK_FixedAssets PRIMARY KEY CLUSTERED ([AssetId] ASC),
    CONSTRAINT UK_FA_Code UNIQUE ([AssetCode]),
    CONSTRAINT FK_FA_Category    FOREIGN KEY ([AssetCategoryId])      REFERENCES [dbo].[FixedAssetCategories]([AssetCategoryId]),
    CONSTRAINT FK_FA_Warehouse   FOREIGN KEY ([CurrentWarehouseId])   REFERENCES [dbo].[Warehouses]([WarehouseId]),
    CONSTRAINT FK_FA_Employee    FOREIGN KEY ([AssignedToEmployeeId]) REFERENCES [dbo].[Employees]([EmployeeId]),
    CONSTRAINT FK_FA_Supplier    FOREIGN KEY ([SupplierId])           REFERENCES [dbo].[Suppliers]([SupplierId])
);
GO

-- -------------------------------------------------------
-- 4. VALORES EAV (atributos específicos por activo)
-- -------------------------------------------------------
CREATE TABLE [dbo].[FixedAssetAttributeValues] (
    [AttributeValueId] [int]           IDENTITY(1,1) NOT NULL,
    [AssetId]          [int]           NOT NULL,
    [AttributeDefId]   [int]           NOT NULL,
    [Value]            [nvarchar](500) NULL,
    [CreatedDate]      [datetime]      NULL CONSTRAINT DF_FAAV_Created DEFAULT GETDATE(),
    [CreatedBy]        [int]           NULL,
    [ModifiedDate]     [datetime]      NULL,
    [ModifiedBy]       [int]           NULL,
    CONSTRAINT PK_FixedAssetAttributeValues PRIMARY KEY ([AttributeValueId]),
    CONSTRAINT UK_FAAV_AssetAttr UNIQUE ([AssetId], [AttributeDefId]),
    CONSTRAINT FK_FAAV_Asset    FOREIGN KEY ([AssetId])        REFERENCES [dbo].[FixedAssets]([AssetId]),
    CONSTRAINT FK_FAAV_AttrDef  FOREIGN KEY ([AttributeDefId]) REFERENCES [dbo].[FixedAssetAttributeDefinitions]([AttributeDefId])
);
GO

-- -------------------------------------------------------
-- 5. ESTADOS DE PARTIDAS CONTABLES DE ACTIVOS FIJOS
-- -------------------------------------------------------
CREATE TABLE [dbo].[AccountingFixedAssetStatus] (
    [StatusId]    [int]          NOT NULL IDENTITY(1,1),
    [StatusCode]  [varchar](30)  NOT NULL,
    [StatusName]  [varchar](100) NOT NULL,
    [Description] [varchar](255) NULL,
    [IsActive]    [bit]          NOT NULL DEFAULT 1,
    CONSTRAINT PK_AccountingFixedAssetStatus PRIMARY KEY ([StatusId]),
    CONSTRAINT UQ_AccountingFixedAssetStatus_Code UNIQUE ([StatusCode])
);
GO

INSERT INTO [dbo].[AccountingFixedAssetStatus] ([StatusCode], [StatusName], [Description]) VALUES
('APROBADO',  'APROBADO',  'Partida contabilizada y aprobada'),
('ANULADO',   'ANULADO',   'Partida anulada, no tiene efecto contable'),
('REVERTIDO', 'REVERTIDO', 'Partida que fue revertida por una contraparte');
GO

-- -------------------------------------------------------
-- 6. PARTIDAS CONTABLES DE ACTIVOS FIJOS — MAESTRO
-- -------------------------------------------------------
CREATE TABLE [dbo].[AccountingEntryFixedAssets] (
    [EntryMasterId] [int]           NOT NULL IDENTITY(1,1),
    [AssetId]       [int]           NOT NULL,
    [EntryDate]     [date]          NOT NULL,
    [Period]        [varchar](7)    NOT NULL,
    [MovementType]  [varchar](30)   NOT NULL,
    [Concept]       [varchar](300)  NOT NULL,
    [TotalAmount]   [decimal](18,2) NOT NULL,
    [StatusId]      [int]           NOT NULL,
    [IsActive]      [bit]           NOT NULL DEFAULT 1,
    [CreatedDate]   [datetime]      NOT NULL DEFAULT GETDATE(),
    [ModifiedDate]  [datetime]      NULL,
    CONSTRAINT PK_AccountingEntryFixedAssets PRIMARY KEY ([EntryMasterId]),
    CONSTRAINT FK_AEFA_Asset   FOREIGN KEY ([AssetId])   REFERENCES [dbo].[FixedAssets]([AssetId]),
    CONSTRAINT FK_AEFA_Status  FOREIGN KEY ([StatusId])  REFERENCES [dbo].[AccountingFixedAssetStatus]([StatusId])
);
GO

-- -------------------------------------------------------
-- 7. PARTIDAS CONTABLES DE ACTIVOS FIJOS — DETALLE
-- -------------------------------------------------------
CREATE TABLE [dbo].[AccountingEntryFixedAssetsDetail] (
    [EntryDetailId]  [int]           NOT NULL IDENTITY(1,1),
    [EntryMasterId]  [int]           NOT NULL,
    [AccountId]      [int]           NOT NULL,
    [Debit]          [decimal](18,2) NOT NULL DEFAULT 0,
    [Credit]         [decimal](18,2) NOT NULL DEFAULT 0,
    [Remarks]        [varchar](300)  NULL,
    CONSTRAINT PK_AccountingEntryFixedAssetsDetail PRIMARY KEY ([EntryDetailId]),
    CONSTRAINT FK_AEFAD_Master  FOREIGN KEY ([EntryMasterId]) REFERENCES [dbo].[AccountingEntryFixedAssets]([EntryMasterId]),
    CONSTRAINT FK_AEFAD_Account FOREIGN KEY ([AccountId])     REFERENCES [dbo].[Accounts]([AccountId])
);
GO

-- -------------------------------------------------------
-- 8. ESTADOS DE TRASLADOS
-- -------------------------------------------------------
CREATE TABLE [dbo].[FixedAssetTransferStatus] (
    [TransferStatusId] [int]          IDENTITY(1,1) NOT NULL,
    [StatusCode]       [varchar](20)  NOT NULL,
    [StatusName]       [varchar](50)  NOT NULL,
    [Description]      [varchar](255) NULL,
    [Order]            [int]          NOT NULL,
    [IsFinal]          [bit]          NOT NULL CONSTRAINT DF_FATS_IsFinal DEFAULT 0,
    [IsActive]         [bit]          NOT NULL CONSTRAINT DF_FATS_Active DEFAULT 1,
    [CreatedDate]      [datetime]     NULL CONSTRAINT DF_FATS_Created DEFAULT GETDATE(),
    [CreatedBy]        [int]          NULL,
    [ModifiedDate]     [datetime]     NULL,
    [ModifiedBy]       [int]          NULL,
    CONSTRAINT PK_FATS PRIMARY KEY ([TransferStatusId]),
    CONSTRAINT UK_FATS_Code UNIQUE ([StatusCode])
);
GO

-- ============================================================
-- ESTADOS DE TRASLADOS
-- ============================================================
SET IDENTITY_INSERT [dbo].[FixedAssetTransferStatus] ON;

INSERT INTO [dbo].[FixedAssetTransferStatus] 
    (TransferStatusId, StatusCode, StatusName, Description, [Order], IsFinal, IsActive, CreatedBy)
VALUES
    (1,  'PENDING',    'PENDIENTE',    'TRASLADO CREADO, PENDIENTE DE APROBACIÓN',  1, 0, 1, 1),
    (2,  'APPROVED',   'APROBADO',     'TRASLADO APROBADO, LISTO PARA EMPAQUETAR',  2, 0, 1, 1),
    (3,  'REJECTED',   'RECHAZADO',    'TRASLADO RECHAZADO',                        2, 1, 1, 1),
    (4,  'PACKING',    'EMPAQUETADO',  'ACTIVOS EMPAQUETADOS LISTOS PARA ENVÍO',    3, 0, 1, 1),
    (5,  'SHIPPED',    'ENVIADO',      'ACTIVOS EN TRÁNSITO HACIA EL DESTINO',      4, 0, 1, 1),
    (6,  'INCIDENT',   'INCIDENCIA',   'SE REPORTÓ UNA INCIDENCIA EN EL TRASLADO',  4, 0, 1, 1),
    (7,  'ONDESTINY',  'EN DESTINO',   'ACTIVOS LLEGARON AL DESTINO',               5, 0, 1, 1),
    (8,  'RECEIVED',   'RECIBIDO',     'ACTIVOS RECIBIDOS Y VERIFICADOS',           6, 0, 1, 1),
    (9,  'COMPLETED',  'COMPLETADO',   'TRASLADO COMPLETADO EXITOSAMENTE',          7, 1, 1, 1);

SET IDENTITY_INSERT [dbo].[FixedAssetTransferStatus] OFF;
GO

-- -------------------------------------------------------
-- 9. TRANSICIONES PERMITIDAS ENTRE ESTADOS
-- -------------------------------------------------------
CREATE TABLE [dbo].[FixedAssetTransferStatusTransitions] (
    [TransitionId] [int]      IDENTITY(1,1) NOT NULL,
    [FromStatusId] [int]      NOT NULL,
    [ToStatusId]   [int]      NOT NULL,
    [CreatedDate]  [datetime] NULL CONSTRAINT DF_FATST_Created DEFAULT GETDATE(),
    [CreatedBy]    [int]      NULL,
    CONSTRAINT PK_FATST         PRIMARY KEY ([TransitionId]),
    CONSTRAINT UK_FATST_Pair    UNIQUE ([FromStatusId], [ToStatusId]),
    CONSTRAINT FK_FATST_From    FOREIGN KEY ([FromStatusId]) REFERENCES [dbo].[FixedAssetTransferStatus]([TransferStatusId]),
    CONSTRAINT FK_FATST_To      FOREIGN KEY ([ToStatusId])   REFERENCES [dbo].[FixedAssetTransferStatus]([TransferStatusId]),
    CONSTRAINT CHK_FATST_NoSelf CHECK ([FromStatusId] <> [ToStatusId])
);
GO

-- ============================================================
-- TRANSICIONES PERMITIDAS
-- ============================================================
INSERT INTO [dbo].[FixedAssetTransferStatusTransitions]
    (FromStatusId, ToStatusId, CreatedBy)
VALUES
    (1, 2, 1),  -- PENDIENTE    → APROBADO
    (1, 3, 1),  -- PENDIENTE    → RECHAZADO
    (2, 4, 1),  -- APROBADO     → EMPAQUETADO
    (4, 5, 1),  -- EMPAQUETADO  → ENVIADO
    (5, 7, 1),  -- ENVIADO      → EN DESTINO
    (5, 6, 1),  -- ENVIADO      → INCIDENCIA
    (6, 3, 1),  -- INCIDENCIA   → RECHAZADO
    (7, 8, 1),  -- EN DESTINO   → RECIBIDO
    (8, 9, 1),  -- RECIBIDO     → COMPLETADO
    (8, 6, 1);  -- RECIBIDO     → INCIDENCIA
GO

-- -------------------------------------------------------
-- 10. TRASLADOS — MAESTRO
-- -------------------------------------------------------
CREATE TABLE [dbo].[FixedAssetTransfers] (
    [TransferId]       [int]           IDENTITY(1,1) NOT NULL,
    [TransferCode]     [varchar](30)   NOT NULL,
    [TransferDate]     [date]          NOT NULL,
    [ToWarehouseId]    [int]           NULL,
    [ToEmployeeId]     [int]           NULL,
    [TransferStatusId] [int]           NOT NULL,
    [Reason]           [varchar](500)  NULL,
    [ApprovedByUserId] [int]           NULL,
    [ApprovedDate]     [datetime]      NULL,
    [CompletedDate]    [datetime]      NULL,
    [CreatedDate]      [datetime]      NULL CONSTRAINT DF_FAT_Created DEFAULT GETDATE(),
    [CreatedBy]        [int]           NULL,
    [ModifiedDate]     [datetime]      NULL,
    [ModifiedBy]       [int]           NULL,
    CONSTRAINT PK_FixedAssetTransfers PRIMARY KEY CLUSTERED ([TransferId] ASC),
    CONSTRAINT UK_FAT_Code        UNIQUE ([TransferCode]),
    CONSTRAINT FK_FAT_ToWarehouse FOREIGN KEY ([ToWarehouseId])    REFERENCES [dbo].[Warehouses]([WarehouseId]),
    CONSTRAINT FK_FAT_ToEmployee  FOREIGN KEY ([ToEmployeeId])     REFERENCES [dbo].[Employees]([EmployeeId]),
    CONSTRAINT FK_FAT_Status      FOREIGN KEY ([TransferStatusId]) REFERENCES [dbo].[FixedAssetTransferStatus]([TransferStatusId])
);
GO

-- -------------------------------------------------------
-- 11. TRASLADOS — DETALLE
-- -------------------------------------------------------
CREATE TABLE [dbo].[FixedAssetTransferDetails] (
    [TransferDetailId] [int]      IDENTITY(1,1) NOT NULL,
    [TransferId]       [int]      NOT NULL,
    [AssetId]          [int]      NOT NULL,
    [FromWarehouseId]  [int]      NULL,
    [FromEmployeeId]   [int]      NULL,
    [CreatedDate]      [datetime] NULL CONSTRAINT DF_FATD_Created DEFAULT GETDATE(),
    [CreatedBy]        [int]      NULL,
    CONSTRAINT PK_FixedAssetTransferDetails PRIMARY KEY CLUSTERED ([TransferDetailId] ASC),
    CONSTRAINT UK_FATD_Transfer_Asset  UNIQUE ([TransferId], [AssetId]),
    CONSTRAINT FK_FATD_Transfer        FOREIGN KEY ([TransferId])       REFERENCES [dbo].[FixedAssetTransfers]([TransferId]) ON DELETE CASCADE,
    CONSTRAINT FK_FATD_Asset           FOREIGN KEY ([AssetId])          REFERENCES [dbo].[FixedAssets]([AssetId]),
    CONSTRAINT FK_FATD_FromWarehouse   FOREIGN KEY ([FromWarehouseId])  REFERENCES [dbo].[Warehouses]([WarehouseId]),
    CONSTRAINT FK_FATD_FromEmployee    FOREIGN KEY ([FromEmployeeId])   REFERENCES [dbo].[Employees]([EmployeeId])
);
GO

-- ============================================================
-- SECCIÓN 3: ÍNDICES
-- ============================================================
CREATE INDEX IX_FA_Category  ON [dbo].[FixedAssets]([AssetCategoryId]);
CREATE INDEX IX_FA_Warehouse ON [dbo].[FixedAssets]([CurrentWarehouseId]);
CREATE INDEX IX_FA_Employee  ON [dbo].[FixedAssets]([AssignedToEmployeeId]);
CREATE INDEX IX_FA_Status    ON [dbo].[FixedAssets]([AssetStatus]);
CREATE INDEX IX_FA_Supplier  ON [dbo].[FixedAssets]([SupplierId]);

CREATE INDEX IX_FAAV_Asset   ON [dbo].[FixedAssetAttributeValues]([AssetId]);
CREATE INDEX IX_FAAV_AttrDef ON [dbo].[FixedAssetAttributeValues]([AttributeDefId]);

CREATE INDEX IX_AEFA_Asset   ON [dbo].[AccountingEntryFixedAssets]([AssetId]);
CREATE INDEX IX_AEFA_Period  ON [dbo].[AccountingEntryFixedAssets]([Period]);

CREATE INDEX IX_FAT_Status   ON [dbo].[FixedAssetTransfers]([TransferStatusId]);
CREATE INDEX IX_FAT_Date     ON [dbo].[FixedAssetTransfers]([TransferDate]);

CREATE INDEX IX_FATST_From   ON [dbo].[FixedAssetTransferStatusTransitions]([FromStatusId]);
CREATE INDEX IX_FATST_To     ON [dbo].[FixedAssetTransferStatusTransitions]([ToStatusId]);
GO