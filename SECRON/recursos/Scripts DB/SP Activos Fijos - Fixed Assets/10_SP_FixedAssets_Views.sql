-- ============================================================
-- VIEWS - MÓDULO DE ACTIVOS FIJOS
-- ============================================================

-- -------------------------------------------------------
-- V_FixedAssetTransferStatus
-- -------------------------------------------------------
CREATE OR ALTER VIEW [dbo].[V_FixedAssetTransferStatus]
AS
SELECT
    s.[TransferStatusId],
    s.[StatusCode],
    s.[StatusName],
    s.[Description],
    s.[Order],
    s.[IsFinal],
    s.[IsActive],
    s.[CreatedDate],
    s.[CreatedBy],
    uc.[FullName]   AS CreatedByName,
    s.[ModifiedDate],
    s.[ModifiedBy],
    um.[FullName]   AS ModifiedByName
FROM [dbo].[FixedAssetTransferStatus] s
LEFT JOIN [dbo].[Users] uc ON uc.[UserId] = s.[CreatedBy]
LEFT JOIN [dbo].[Users] um ON um.[UserId] = s.[ModifiedBy];
GO

-- -------------------------------------------------------
-- V_FixedAssetTransferStatusTransitions
-- -------------------------------------------------------
CREATE OR ALTER VIEW [dbo].[V_FixedAssetTransferStatusTransitions]
AS
SELECT
    t.[TransitionId],
    t.[FromStatusId],
    sf.[StatusCode]  AS FromStatusCode,
    sf.[StatusName]  AS FromStatusName,
    t.[ToStatusId],
    st.[StatusCode]  AS ToStatusCode,
    st.[StatusName]  AS ToStatusName,
    t.[CreatedDate],
    t.[CreatedBy],
    uc.[FullName]    AS CreatedByName
FROM [dbo].[FixedAssetTransferStatusTransitions] t
INNER JOIN [dbo].[FixedAssetTransferStatus] sf ON sf.[TransferStatusId] = t.[FromStatusId]
INNER JOIN [dbo].[FixedAssetTransferStatus] st ON st.[TransferStatusId] = t.[ToStatusId]
LEFT JOIN  [dbo].[Users] uc                    ON uc.[UserId]           = t.[CreatedBy];
GO

-- -------------------------------------------------------
-- V_FixedAssetMovements
-- -------------------------------------------------------
CREATE OR ALTER VIEW [dbo].[V_FixedAssetMovements]
AS
SELECT
    tr.[TransferId],
    tr.[TransferCode],
    tr.[AssetId],
    fa.[AssetCode],
    fa.[AssetName],
    tr.[TransferStatusId],
    ts.[StatusCode],
    ts.[StatusName],
    tr.[TransferDate],
    tr.[FromWarehouseId],
    wf.[WarehouseName]                          AS FromWarehouseName,
    tr.[FromEmployeeId],
    CONCAT(ef.[FirstName], ' ', ef.[LastName])  AS FromEmployeeName,
    tr.[ToWarehouseId],
    wt.[WarehouseName]                          AS ToWarehouseName,
    tr.[ToEmployeeId],
    CONCAT(et.[FirstName], ' ', et.[LastName])  AS ToEmployeeName,
    tr.[Reason],
    tr.[ApprovedByUserId],
    tr.[ApprovedDate],
    tr.[CompletedDate],
    tr.[CreatedDate],
    tr.[CreatedBy],
    uc.[FullName]                               AS CreatedByName,
    tr.[ModifiedDate],
    tr.[ModifiedBy],
    um.[FullName]                               AS ModifiedByName
FROM [dbo].[FixedAssetTransfers] tr
INNER JOIN [dbo].[FixedAssets]              fa  ON fa.[AssetId]          = tr.[AssetId]
INNER JOIN [dbo].[FixedAssetTransferStatus] ts  ON ts.[TransferStatusId] = tr.[TransferStatusId]
LEFT JOIN  [dbo].[Warehouses]               wf  ON wf.[WarehouseId]      = tr.[FromWarehouseId]
LEFT JOIN  [dbo].[Warehouses]               wt  ON wt.[WarehouseId]      = tr.[ToWarehouseId]
LEFT JOIN  [dbo].[Employees]                ef  ON ef.[EmployeeId]       = tr.[FromEmployeeId]
LEFT JOIN  [dbo].[Employees]                et  ON et.[EmployeeId]       = tr.[ToEmployeeId]
LEFT JOIN  [dbo].[Users]                    uc  ON uc.[UserId]           = tr.[CreatedBy]
LEFT JOIN  [dbo].[Users]                    um  ON um.[UserId]           = tr.[ModifiedBy];
GO