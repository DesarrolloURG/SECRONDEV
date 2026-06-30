-- ============================================================
-- 19. SPs - TRASLADOS DE ACTIVOS FIJOS
-- ============================================================

-- -------------------------------------------------------
-- SP_FixedAssetTransfers_Update
-- Actualiza estado del traslado validando transición permitida
-- Si nuevo estado = REJECTED → restaura activos a ACTIVE
-- -------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[SP_FixedAssetTransfers_Update]
    @TransferId         INT,
    @TransferCode       VARCHAR(30),
    @TransferDate       DATE,
    @ToWarehouseId      INT          = NULL,
    @ToEmployeeId       INT          = NULL,
    @TransferStatusId   INT,
    @Reason             VARCHAR(500) = NULL,
    @ApprovedByUserId   INT          = NULL,
    @ApprovedDate       DATETIME     = NULL,
    @CompletedDate      DATETIME     = NULL,
    @ModifiedBy         INT          = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION
    BEGIN TRY

        IF NOT EXISTS (SELECT 1 FROM [dbo].[FixedAssetTransfers]
                       WHERE [TransferId] = @TransferId)
        BEGIN ROLLBACK TRANSACTION; SELECT -1; RETURN; END

        IF EXISTS (SELECT 1 FROM [dbo].[FixedAssetTransfers]
                   WHERE [TransferCode] = UPPER(@TransferCode)
                     AND [TransferId] <> @TransferId)
        BEGIN ROLLBACK TRANSACTION; SELECT -2; RETURN; END

        IF @ToWarehouseId IS NULL AND @ToEmployeeId IS NULL
        BEGIN ROLLBACK TRANSACTION; SELECT -3; RETURN; END

        DECLARE @CurrentStatusId INT;
        SELECT @CurrentStatusId = [TransferStatusId]
        FROM [dbo].[FixedAssetTransfers]
        WHERE [TransferId] = @TransferId;

        IF @CurrentStatusId <> @TransferStatusId
        BEGIN
            IF NOT EXISTS (
                SELECT 1 FROM [dbo].[FixedAssetTransferStatusTransitions]
                WHERE [FromStatusId] = @CurrentStatusId
                  AND [ToStatusId]   = @TransferStatusId)
            BEGIN ROLLBACK TRANSACTION; SELECT -4; RETURN; END
        END

        UPDATE [dbo].[FixedAssetTransfers] SET
            [TransferCode]      = UPPER(@TransferCode),
            [TransferDate]      = @TransferDate,
            [ToWarehouseId]     = @ToWarehouseId,
            [ToEmployeeId]      = @ToEmployeeId,
            [TransferStatusId]  = @TransferStatusId,
            [Reason]            = UPPER(@Reason),
            [ApprovedByUserId]  = @ApprovedByUserId,
            [ApprovedDate]      = @ApprovedDate,
            [CompletedDate]     = @CompletedDate,
            [ModifiedDate]      = GETDATE(),
            [ModifiedBy]        = @ModifiedBy
        WHERE [TransferId] = @TransferId;

        -- Si nuevo estado es final → restaurar activos a ACTIVE
        IF EXISTS (SELECT 1 FROM [dbo].[FixedAssetTransferStatus]
                   WHERE [TransferStatusId] = @TransferStatusId
                     AND [IsFinal] = 1)
        BEGIN
            UPDATE fa SET
                fa.[AssetStatus]  = 'ACTIVO',
                fa.[ModifiedDate] = GETDATE(),
                fa.[ModifiedBy]   = @ModifiedBy
            FROM [dbo].[FixedAssets] fa
            INNER JOIN [dbo].[FixedAssetTransferDetails] fatd
                ON fatd.[AssetId] = fa.[AssetId]
            WHERE fatd.[TransferId] = @TransferId;
        END

        COMMIT TRANSACTION; SELECT 1;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO

-- -------------------------------------------------------
-- SP_FixedAssetTransfers_Inactive
-- Cancela traslado PENDING → REJECTED
-- Restaura AssetStatus = ACTIVE en todos sus activos
-- -------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[SP_FixedAssetTransfers_Inactive]
    @TransferId     INT,
    @ModifiedBy     INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION
    BEGIN TRY

        IF NOT EXISTS (SELECT 1 FROM [dbo].[FixedAssetTransfers]
                       WHERE [TransferId] = @TransferId)
        BEGIN ROLLBACK TRANSACTION; SELECT -1; RETURN; END

        -- Solo se pueden cancelar traslados que no sean finales (COMPLETED)
        IF EXISTS (
            SELECT 1 FROM [dbo].[FixedAssetTransfers] t
            INNER JOIN [dbo].[FixedAssetTransferStatus] ts
                ON ts.[TransferStatusId] = t.[TransferStatusId]
            WHERE t.[TransferId] = @TransferId
              AND ts.[IsFinal] = 1)
        BEGIN ROLLBACK TRANSACTION; SELECT -2; RETURN; END

        DECLARE @RejectedId INT;
        SELECT @RejectedId = [TransferStatusId]
        FROM [dbo].[FixedAssetTransferStatus]
        WHERE [StatusCode] = 'REJECTED';

        UPDATE [dbo].[FixedAssetTransfers] SET
            [TransferStatusId] = @RejectedId,
            [ModifiedDate]     = GETDATE(),
            [ModifiedBy]       = @ModifiedBy
        WHERE [TransferId] = @TransferId;

        -- Restaurar AssetStatus = ACTIVE en todos los activos del traslado
        UPDATE fa SET
            fa.[AssetStatus]  = 'ACTIVO',
            fa.[ModifiedDate] = GETDATE(),
            fa.[ModifiedBy]   = @ModifiedBy
        FROM [dbo].[FixedAssets] fa
        INNER JOIN [dbo].[FixedAssetTransferDetails] fatd
            ON fatd.[AssetId] = fa.[AssetId]
        WHERE fatd.[TransferId] = @TransferId;

        COMMIT TRANSACTION; SELECT 1;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO

-- -------------------------------------------------------
-- SP_FixedAssetTransferDetails_Delete
-- Elimina un activo del detalle y restaura AssetStatus = ACTIVE
-- -------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[SP_FixedAssetTransferDetails_Delete]
    @TransferDetailId   INT,
    @ModifiedBy         INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION
    BEGIN TRY

        IF NOT EXISTS (SELECT 1 FROM [dbo].[FixedAssetTransferDetails]
                       WHERE [TransferDetailId] = @TransferDetailId)
        BEGIN ROLLBACK TRANSACTION; SELECT -1; RETURN; END

        -- Solo se puede quitar activo si el traslado está en PENDING
        IF NOT EXISTS (
            SELECT 1 FROM [dbo].[FixedAssetTransferDetails] fatd
            INNER JOIN [dbo].[FixedAssetTransfers] t
                ON t.[TransferId] = fatd.[TransferId]
            INNER JOIN [dbo].[FixedAssetTransferStatus] ts
                ON ts.[TransferStatusId] = t.[TransferStatusId]
            WHERE fatd.[TransferDetailId] = @TransferDetailId
              AND ts.[StatusCode] = 'PENDING')
        BEGIN ROLLBACK TRANSACTION; SELECT -2; RETURN; END -- Solo en PENDING

        -- Restaurar AssetStatus = ACTIVE antes de eliminar
        UPDATE fa SET
            fa.[AssetStatus]  = 'ACTIVO',
            fa.[ModifiedDate] = GETDATE(),
            fa.[ModifiedBy]   = @ModifiedBy
        FROM [dbo].[FixedAssets] fa
        INNER JOIN [dbo].[FixedAssetTransferDetails] fatd
            ON fatd.[AssetId] = fa.[AssetId]
        WHERE fatd.[TransferDetailId] = @TransferDetailId;

        DELETE FROM [dbo].[FixedAssetTransferDetails]
        WHERE [TransferDetailId] = @TransferDetailId;

        COMMIT TRANSACTION; SELECT 1;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO