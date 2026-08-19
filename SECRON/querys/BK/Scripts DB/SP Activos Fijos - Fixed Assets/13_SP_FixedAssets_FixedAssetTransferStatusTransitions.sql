-- ============================================================
-- SPs - TRANSICIONES ENTRE ESTADOS
-- ============================================================

-- -------------------------------------------------------
-- SP_FixedAssetTransferStatusTransitions_Select
-- -------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[SP_FixedAssetTransferStatusTransitions_Select]
    @FromStatusId   INT = NULL,
    @ToStatusId     INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [TransitionId],
        [FromStatusId],
        [FromStatusCode],
        [FromStatusName],
        [ToStatusId],
        [ToStatusCode],
        [ToStatusName],
        [CreatedDate],
        [CreatedBy],
        [CreatedByName]
    FROM [dbo].[V_FixedAssetTransferStatusTransitions]
    WHERE
        (@FromStatusId IS NULL OR [FromStatusId] = @FromStatusId)
    AND (@ToStatusId   IS NULL OR [ToStatusId]   = @ToStatusId)
    ORDER BY [FromStatusName], [ToStatusName];
END
GO

-- -------------------------------------------------------
-- SP_FixedAssetTransferStatusTransitions_Insert
-- -------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[SP_FixedAssetTransferStatusTransitions_Insert]
    @FromStatusId   INT,
    @ToStatusId     INT,
    @CreatedBy      INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION
    BEGIN TRY

        IF @FromStatusId = @ToStatusId
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -1  -- Origen y destino iguales
            RETURN
        END

        IF NOT EXISTS (SELECT 1 FROM [dbo].[FixedAssetTransferStatus]
                       WHERE [TransferStatusId] = @FromStatusId AND [IsActive] = 1)
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -2  -- Estado origen no válido
            RETURN
        END

        IF NOT EXISTS (SELECT 1 FROM [dbo].[FixedAssetTransferStatus]
                       WHERE [TransferStatusId] = @ToStatusId AND [IsActive] = 1)
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -3  -- Estado destino no válido
            RETURN
        END

        IF EXISTS (SELECT 1 FROM [dbo].[FixedAssetTransferStatus]
                   WHERE [TransferStatusId] = @FromStatusId AND [IsFinal] = 1)
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -4  -- Estado final no puede tener transiciones de salida
            RETURN
        END

        IF EXISTS (SELECT 1 FROM [dbo].[FixedAssetTransferStatusTransitions]
                   WHERE [FromStatusId] = @FromStatusId AND [ToStatusId] = @ToStatusId)
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -5  -- Transición ya existe
            RETURN
        END

        INSERT INTO [dbo].[FixedAssetTransferStatusTransitions]
            ([FromStatusId],[ToStatusId],[CreatedBy])
        VALUES
            (@FromStatusId, @ToStatusId, @CreatedBy)

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
-- SP_FixedAssetTransferStatusTransitions_Delete
-- -------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[SP_FixedAssetTransferStatusTransitions_Delete]
    @TransitionId   INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION
    BEGIN TRY

        IF NOT EXISTS (SELECT 1 FROM [dbo].[FixedAssetTransferStatusTransitions]
                       WHERE [TransitionId] = @TransitionId)
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -1  -- No existe
            RETURN
        END

        DELETE FROM [dbo].[FixedAssetTransferStatusTransitions]
        WHERE [TransitionId] = @TransitionId

        COMMIT TRANSACTION
        SELECT 1

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        SELECT 0
    END CATCH
END
GO