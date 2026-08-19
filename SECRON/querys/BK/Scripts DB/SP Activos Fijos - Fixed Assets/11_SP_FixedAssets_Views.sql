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
-- ============================================================
-- VISTA MAESTRO
-- ============================================================
CREATE OR ALTER VIEW [dbo].[V_FixedAssetTransfers]
AS
SELECT
    t.[TransferId],
    t.[TransferCode],
    t.[TransferDate],
    t.[ToWarehouseId],
    w.[WarehouseName]   AS [ToWarehouseName],
    t.[ToEmployeeId],
    CONCAT(e.[FirstName], ' ', e.[LastName]) AS [ToEmployeeName],
    t.[TransferStatusId],
    ts.[StatusCode],
    ts.[StatusName],
    t.[Reason],
    t.[ApprovedByUserId],
    t.[ApprovedDate],
    t.[CompletedDate],
    t.[CreatedDate],
    t.[CreatedBy],
    uc.[FullName]       AS [CreatedByName],
    t.[ModifiedDate],
    t.[ModifiedBy],
    um.[FullName]       AS [ModifiedByName]
FROM [dbo].[FixedAssetTransfers] t
LEFT JOIN [dbo].[Warehouses] w
    ON w.[WarehouseId] = t.[ToWarehouseId]
LEFT JOIN [dbo].[Employees] e
    ON e.[EmployeeId] = t.[ToEmployeeId]
INNER JOIN [dbo].[FixedAssetTransferStatus] ts
    ON ts.[TransferStatusId] = t.[TransferStatusId]
LEFT JOIN [dbo].[Users] uc
    ON uc.[UserId] = t.[CreatedBy]
LEFT JOIN [dbo].[Users] um
    ON um.[UserId] = t.[ModifiedBy];
GO

-- ============================================================
-- VISTA DETALLE
-- ============================================================
CREATE OR ALTER VIEW [dbo].[V_FixedAssetTransferDetails]
AS
SELECT
    d.[TransferDetailId],
    d.[TransferId],
    d.[AssetId],
    a.[AssetCode],
    a.[AssetName],
    d.[FromWarehouseId],
    fw.[WarehouseName]  AS [FromWarehouseName],
    d.[FromEmployeeId],
    CONCAT(fe.[FirstName], ' ', fe.[LastName]) AS [FromEmployeeName],
    d.[CreatedDate],
    d.[CreatedBy],
    uc.[FullName]       AS [CreatedByName]
FROM [dbo].[FixedAssetTransferDetails] d
INNER JOIN [dbo].[FixedAssets] a
    ON a.[AssetId] = d.[AssetId]
LEFT JOIN [dbo].[Warehouses] fw
    ON fw.[WarehouseId] = d.[FromWarehouseId]
LEFT JOIN [dbo].[Employees] fe
    ON fe.[EmployeeId] = d.[FromEmployeeId]
LEFT JOIN [dbo].[Users] uc
    ON uc.[UserId] = d.[CreatedBy];
GO