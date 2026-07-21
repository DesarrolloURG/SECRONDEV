-- ============================================================
-- SP: Obtener próximo código de traslado (automático)
-- ============================================================
CREATE OR ALTER PROCEDURE [dbo].[SP_FixedAssetTransfers_NextCode]
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @ultimo INT;

    SELECT @ultimo = MAX(CAST(SUBSTRING([TransferCode], 5, LEN([TransferCode])) AS INT))
    FROM [dbo].[FixedAssetTransfers]
    WHERE [TransferCode] LIKE 'TRA-%'
      AND ISNUMERIC(SUBSTRING([TransferCode], 5, LEN([TransferCode]))) = 1;

    SET @ultimo = ISNULL(@ultimo, 0) + 1;

    SELECT 'TRA-' + RIGHT('000000' + CAST(@ultimo AS VARCHAR), 6) AS NextCode;
END
GO

-- ============================================================
-- SP: SELECT Maestro
-- ============================================================
CREATE OR ALTER PROCEDURE [dbo].[SP_FixedAssetTransfers_Select]
    @TransferCode       VARCHAR(30) = NULL,
    @TransferStatusId   INT         = NULL,
    @ToWarehouseId      INT         = NULL,
    @FechaInicio        DATE        = NULL,
    @FechaFin           DATE        = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [TransferId], [TransferCode], [TransferDate],
        [ToWarehouseId], [ToWarehouseName], [ToEmployeeId], [ToEmployeeName],
        [TransferStatusId], [StatusCode], [StatusName],
        [Reason], [ApprovedByUserId], [ApprovedDate], [CompletedDate],
        [CreatedDate], [CreatedBy], [CreatedByName],
        [ModifiedDate], [ModifiedBy], [ModifiedByName]
    FROM [dbo].[V_FixedAssetTransfers]
    WHERE
        (@TransferCode     IS NULL OR [TransferCode]    LIKE '%' + @TransferCode + '%')
    AND (@TransferStatusId IS NULL OR [TransferStatusId] = @TransferStatusId)
    AND (@ToWarehouseId    IS NULL OR [ToWarehouseId]    = @ToWarehouseId)
    AND (@FechaInicio      IS NULL OR [TransferDate]    >= @FechaInicio)
    AND (@FechaFin         IS NULL OR [TransferDate]    <= @FechaFin)
    ORDER BY [TransferDate] DESC, [TransferId] DESC;
END
GO

-- ============================================================
-- SP: INSERT Maestro
-- ============================================================
CREATE OR ALTER PROCEDURE [dbo].[SP_FixedAssetTransfers_Insert]
    @TransferCode       VARCHAR(30),
    @TransferDate       DATE,
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
                   WHERE [TransferCode] = UPPER(@TransferCode))
        BEGIN ROLLBACK TRANSACTION; SELECT -1; RETURN; END -- Código duplicado

        IF @ToWarehouseId IS NULL AND @ToEmployeeId IS NULL
        BEGIN ROLLBACK TRANSACTION; SELECT -2; RETURN; END -- Sin destino

        INSERT INTO [dbo].[FixedAssetTransfers]
            ([TransferCode],[TransferDate],[ToWarehouseId],[ToEmployeeId],
             [TransferStatusId],[Reason],[CreatedBy])
        VALUES
            (UPPER(@TransferCode), @TransferDate, @ToWarehouseId, @ToEmployeeId,
             @TransferStatusId, UPPER(@Reason), @CreatedBy);

        SELECT SCOPE_IDENTITY(); -- Retorna el TransferId generado
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO

-- ============================================================
-- SP: UPDATE Maestro
-- ============================================================
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

        IF NOT EXISTS (SELECT 1 FROM [dbo].[FixedAssetTransfers] WHERE [TransferId] = @TransferId)
        BEGIN ROLLBACK TRANSACTION; SELECT -1; RETURN; END

        IF EXISTS (SELECT 1 FROM [dbo].[FixedAssetTransfers]
                   WHERE [TransferCode] = UPPER(@TransferCode) AND [TransferId] <> @TransferId)
        BEGIN ROLLBACK TRANSACTION; SELECT -2; RETURN; END

        IF @ToWarehouseId IS NULL AND @ToEmployeeId IS NULL
        BEGIN ROLLBACK TRANSACTION; SELECT -3; RETURN; END

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

        COMMIT TRANSACTION; SELECT 1;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO

-- ============================================================
-- SP: INACTIVE Maestro (cancela → REJECTED)
-- ============================================================
CREATE OR ALTER PROCEDURE [dbo].[SP_FixedAssetTransfers_Inactive]
    @TransferId     INT,
    @ModifiedBy     INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION
    BEGIN TRY

        IF NOT EXISTS (SELECT 1 FROM [dbo].[FixedAssetTransfers] WHERE [TransferId] = @TransferId)
        BEGIN ROLLBACK TRANSACTION; SELECT -1; RETURN; END

        IF NOT EXISTS (
            SELECT 1 FROM [dbo].[FixedAssetTransfers] t
            INNER JOIN [dbo].[FixedAssetTransferStatus] ts ON ts.[TransferStatusId] = t.[TransferStatusId]
            WHERE t.[TransferId] = @TransferId AND ts.[StatusCode] = 'PENDING')
        BEGIN ROLLBACK TRANSACTION; SELECT -2; RETURN; END

        DECLARE @RejectedId INT;
        SELECT @RejectedId = [TransferStatusId] FROM [dbo].[FixedAssetTransferStatus]
        WHERE [StatusCode] = 'REJECTED';

        UPDATE [dbo].[FixedAssetTransfers] SET
            [TransferStatusId] = @RejectedId,
            [ModifiedDate]     = GETDATE(),
            [ModifiedBy]       = @ModifiedBy
        WHERE [TransferId] = @TransferId;

        COMMIT TRANSACTION; SELECT 1;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO