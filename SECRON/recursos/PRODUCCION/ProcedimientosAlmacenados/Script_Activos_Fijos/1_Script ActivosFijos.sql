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