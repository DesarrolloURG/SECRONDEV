-- ============================================================
-- SPs - TRASLADOS DE ACTIVOS
-- ============================================================

-- -------------------------------------------------------
-- SP_FixedAssetMovements_Select
-- -------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[SP_FixedAssetMovements_Select]
    @TransferCode       VARCHAR(30) = NULL,
    @AssetCode          VARCHAR(30) = NULL,
    @TransferStatusId   INT         = NULL,
    @FromWarehouseId    INT         = NULL,
    @ToWarehouseId      INT         = NULL,
    @FechaInicio        DATE        = NULL,
    @FechaFin           DATE        = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [TransferId],
        [TransferCode],
        [AssetId],
        [AssetCode],
        [AssetName],
        [TransferStatusId],
        [StatusCode],
        [StatusName],
        [TransferDate],
        [FromWarehouseId],
        [FromWarehouseName],
        [FromEmployeeId],
        [FromEmployeeName],
        [ToWarehouseId],
        [ToWarehouseName],
        [ToEmployeeId],
        [ToEmployeeName],
        [Reason],
        [ApprovedByUserId],
        [ApprovedDate],
        [CompletedDate],
        [CreatedDate],
        [CreatedBy],
        [CreatedByName],
        [ModifiedDate],
        [ModifiedBy],
        [ModifiedByName]
    FROM [dbo].[V_FixedAssetMovements]
    WHERE
        (@TransferCode     IS NULL OR [TransferCode] LIKE '%' + @TransferCode + '%')
    AND (@AssetCode        IS NULL OR [AssetCode]    LIKE '%' + @AssetCode    + '%')
    AND (@TransferStatusId IS NULL OR [TransferStatusId] = @TransferStatusId)
    AND (@FromWarehouseId  IS NULL OR [FromWarehouseId]  = @FromWarehouseId)
    AND (@ToWarehouseId    IS NULL OR [ToWarehouseId]    = @ToWarehouseId)
    AND (@FechaInicio      IS NULL OR [TransferDate]    >= @FechaInicio)
    AND (@FechaFin         IS NULL OR [TransferDate]    <= @FechaFin)
    ORDER BY [TransferDate] DESC, [TransferId] DESC;
END
GO

-- -------------------------------------------------------
-- SP_FixedAssetMovements_Insert
-- -------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[SP_FixedAssetMovements_Insert]
    @TransferCode       VARCHAR(30),
    @AssetId            INT,
    @TransferDate       DATE,
    @FromWarehouseId    INT          = NULL,
    @FromEmployeeId     INT          = NULL,
    @ToWarehouseId      INT          = NULL,
    @ToEmployeeId       INT          = NULL,
    @TransferStatusId   INT,
    @Reason             VARCHAR(500) = NULL,
    @CreatedBy          INT          = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION
    BEGIN TRY

        IF EXISTS (SELECT 1 FROM [dbo].[FixedAssetTransfers]
                   WHERE [TransferCode] = @TransferCode)
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -1  -- Código ya existe
            RETURN
        END

        IF NOT EXISTS (SELECT 1 FROM [dbo].[FixedAssets]
                       WHERE [AssetId] = @AssetId AND [IsActive] = 1)
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -2  -- Activo no válido
            RETURN
        END

        IF @ToWarehouseId IS NULL AND @ToEmployeeId IS NULL
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -3  -- Sin destino definido
            RETURN
        END

        INSERT INTO [dbo].[FixedAssetTransfers]
            ([TransferCode],[AssetId],[TransferDate],
             [FromWarehouseId],[FromEmployeeId],
             [ToWarehouseId],[ToEmployeeId],
             [TransferStatusId],[Reason],[CreatedBy])
        VALUES
            (@TransferCode, @AssetId, @TransferDate,
             @FromWarehouseId, @FromEmployeeId,
             @ToWarehouseId, @ToEmployeeId,
             @TransferStatusId, UPPER(@Reason), @CreatedBy)

        COMMIT TRANSACTION
        SELECT 1

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        SELECT 0
    END CATCH
END
GO

-- -------------------------------------------------------
-- SP_FixedAssetMovements_Update
-- -------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[SP_FixedAssetMovements_Update]
    @TransferId         INT,
    @TransferCode       VARCHAR(30),
    @AssetId            INT,
    @TransferDate       DATE,
    @FromWarehouseId    INT          = NULL,
    @FromEmployeeId     INT          = NULL,
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
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -1  -- No existe
            RETURN
        END

        IF EXISTS (SELECT 1 FROM [dbo].[FixedAssetTransfers]
                   WHERE [TransferCode] = @TransferCode
                   AND   [TransferId]  <> @TransferId)
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -2  -- Código duplicado
            RETURN
        END

        IF @ToWarehouseId IS NULL AND @ToEmployeeId IS NULL
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -3  -- Sin destino definido
            RETURN
        END

        UPDATE [dbo].[FixedAssetTransfers] SET
            [TransferCode]      = @TransferCode,
            [AssetId]           = @AssetId,
            [TransferDate]      = @TransferDate,
            [FromWarehouseId]   = @FromWarehouseId,
            [FromEmployeeId]    = @FromEmployeeId,
            [ToWarehouseId]     = @ToWarehouseId,
            [ToEmployeeId]      = @ToEmployeeId,
            [TransferStatusId]  = @TransferStatusId,
            [Reason]            = UPPER(@Reason),
            [ApprovedByUserId]  = @ApprovedByUserId,
            [ApprovedDate]      = @ApprovedDate,
            [CompletedDate]     = @CompletedDate,
            [ModifiedDate]      = GETDATE(),
            [ModifiedBy]        = @ModifiedBy
        WHERE [TransferId] = @TransferId

        COMMIT TRANSACTION
        SELECT 1

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        SELECT 0
    END CATCH
END
GO

-- -------------------------------------------------------
-- SP_FixedAssetMovements_Inactive
-- -------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[SP_FixedAssetMovements_Inactive]
    @TransferId     INT,
    @ModifiedBy     INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION
    BEGIN TRY

        IF NOT EXISTS (SELECT 1 FROM [dbo].[FixedAssetTransfers]
                       WHERE [TransferId] = @TransferId)
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -1  -- No existe
            RETURN
        END

        -- Solo se pueden cancelar traslados en estado PENDING
        IF NOT EXISTS (
            SELECT 1
            FROM  [dbo].[FixedAssetTransfers] tr
            INNER JOIN [dbo].[FixedAssetTransferStatus] ts
                  ON ts.[TransferStatusId] = tr.[TransferStatusId]
            WHERE tr.[TransferId] = @TransferId
            AND   ts.[StatusCode] = 'PENDING')
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -2  -- Solo se pueden cancelar traslados pendientes
            RETURN
        END

        DECLARE @RejectedStatusId INT
        SELECT @RejectedStatusId = [TransferStatusId]
        FROM [dbo].[FixedAssetTransferStatus]
        WHERE [StatusCode] = 'REJECTED'

        UPDATE [dbo].[FixedAssetTransfers] SET
            [TransferStatusId]  = @RejectedStatusId,
            [ModifiedDate]      = GETDATE(),
            [ModifiedBy]        = @ModifiedBy
        WHERE [TransferId] = @TransferId

        COMMIT TRANSACTION
        SELECT 1

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        SELECT 0
    END CATCH
END
GO