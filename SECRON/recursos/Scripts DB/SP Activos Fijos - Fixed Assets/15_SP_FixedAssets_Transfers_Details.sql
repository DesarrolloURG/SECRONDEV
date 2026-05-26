-- ============================================================
-- SP: SELECT Detalle por TransferId
-- ============================================================
CREATE OR ALTER PROCEDURE [dbo].[SP_FixedAssetTransferDetails_Select]
    @TransferId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [TransferDetailId], [TransferId],
        [AssetId], [AssetCode], [AssetName],
        [FromWarehouseId], [FromWarehouseName],
        [FromEmployeeId], [FromEmployeeName],
        [CreatedDate], [CreatedBy], [CreatedByName]
    FROM [dbo].[V_FixedAssetTransferDetails]
    WHERE [TransferId] = @TransferId
    ORDER BY [TransferDetailId];
END
GO

-- ============================================================
-- SP: INSERT Detalle
-- ============================================================
CREATE OR ALTER PROCEDURE [dbo].[SP_FixedAssetTransferDetails_Insert]
    @TransferId         INT,
    @AssetId            INT,
    @FromWarehouseId    INT = NULL,
    @FromEmployeeId     INT = NULL,
    @CreatedBy          INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION
    BEGIN TRY

        IF NOT EXISTS (SELECT 1 FROM [dbo].[FixedAssetTransfers] WHERE [TransferId] = @TransferId)
        BEGIN ROLLBACK TRANSACTION; SELECT -1; RETURN; END -- Maestro no existe

        IF NOT EXISTS (SELECT 1 FROM [dbo].[FixedAssets] WHERE [AssetId] = @AssetId AND [IsActive] = 1)
        BEGIN ROLLBACK TRANSACTION; SELECT -2; RETURN; END -- Activo no válido

        IF EXISTS (SELECT 1 FROM [dbo].[FixedAssetTransferDetails]
                   WHERE [TransferId] = @TransferId AND [AssetId] = @AssetId)
        BEGIN ROLLBACK TRANSACTION; SELECT -3; RETURN; END -- Activo ya agregado

        INSERT INTO [dbo].[FixedAssetTransferDetails]
            ([TransferId],[AssetId],[FromWarehouseId],[FromEmployeeId],[CreatedBy])
        VALUES
            (@TransferId, @AssetId, @FromWarehouseId, @FromEmployeeId, @CreatedBy);

        COMMIT TRANSACTION; SELECT 1;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO

-- ============================================================
-- SP: DELETE Detalle (físico — quitar activo del traslado)
-- ============================================================
CREATE OR ALTER PROCEDURE [dbo].[SP_FixedAssetTransferDetails_Delete]
    @TransferDetailId INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION
    BEGIN TRY

        IF NOT EXISTS (SELECT 1 FROM [dbo].[FixedAssetTransferDetails]
                       WHERE [TransferDetailId] = @TransferDetailId)
        BEGIN ROLLBACK TRANSACTION; SELECT -1; RETURN; END

        DELETE FROM [dbo].[FixedAssetTransferDetails]
        WHERE [TransferDetailId] = @TransferDetailId;

        COMMIT TRANSACTION; SELECT 1;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO