-- ============================================================
-- SPs - ESTADOS DE TRASLADOS
-- ============================================================

-- -------------------------------------------------------
-- SP_FixedAssetTransferStatus_Select
-- -------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[SP_FixedAssetTransferStatus_Select]
    @StatusCode     VARCHAR(20)  = NULL,
    @StatusName     VARCHAR(50)  = NULL,
    @IsActive       BIT          = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [TransferStatusId],
        [StatusCode],
        [StatusName],
        [Description],
        [Order],
        [IsFinal],
        [IsActive],
        [CreatedDate],
        [CreatedBy],
        [CreatedByName],
        [ModifiedDate],
        [ModifiedBy],
        [ModifiedByName]
    FROM [dbo].[V_FixedAssetTransferStatus]
    WHERE
        (@StatusCode IS NULL OR [StatusCode] LIKE '%' + UPPER(@StatusCode) + '%')
    AND (@StatusName IS NULL OR [StatusName] LIKE '%' + UPPER(@StatusName) + '%')
    AND (@IsActive   IS NULL OR [IsActive]  = @IsActive)
    ORDER BY [Order] ASC;
END
GO

-- -------------------------------------------------------
-- SP_FixedAssetTransferStatus_Insert
-- -------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[SP_FixedAssetTransferStatus_Insert]
    @StatusCode     VARCHAR(20),
    @StatusName     VARCHAR(50),
    @Description    VARCHAR(255) = NULL,
    @Order          INT,
    @IsFinal        BIT          = 0,
    @CreatedBy      INT          = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION
    BEGIN TRY

        -- Código duplicado
        IF EXISTS (SELECT 1 FROM [dbo].[FixedAssetTransferStatus]
                   WHERE [StatusCode] = UPPER(@StatusCode))
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -1  -- Código ya existe
            RETURN
        END

        -- Orden duplicado
        IF EXISTS (SELECT 1 FROM [dbo].[FixedAssetTransferStatus]
                   WHERE [Order] = @Order)
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -2  -- Orden ya existe
            RETURN
        END

        INSERT INTO [dbo].[FixedAssetTransferStatus]
            ([StatusCode],[StatusName],[Description],[Order],[IsFinal],[IsActive],[CreatedBy])
        VALUES
            (UPPER(@StatusCode), UPPER(@StatusName), UPPER(@Description),
             @Order, @IsFinal, 1, @CreatedBy)

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
-- SP_FixedAssetTransferStatus_Update
-- -------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[SP_FixedAssetTransferStatus_Update]
    @TransferStatusId   INT,
    @StatusCode         VARCHAR(20),
    @StatusName         VARCHAR(50),
    @Description        VARCHAR(255) = NULL,
    @Order              INT,
    @IsFinal            BIT,
    @IsActive           BIT,
    @ModifiedBy         INT          = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION
    BEGIN TRY

        IF NOT EXISTS (SELECT 1 FROM [dbo].[FixedAssetTransferStatus]
                       WHERE [TransferStatusId] = @TransferStatusId)
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -1  -- No existe
            RETURN
        END

        -- Código duplicado en otro registro
        IF EXISTS (SELECT 1 FROM [dbo].[FixedAssetTransferStatus]
                   WHERE [StatusCode] = UPPER(@StatusCode)
                   AND   [TransferStatusId] <> @TransferStatusId)
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -2  -- Código duplicado
            RETURN
        END

        -- Orden duplicado en otro registro
        IF EXISTS (SELECT 1 FROM [dbo].[FixedAssetTransferStatus]
                   WHERE [Order] = @Order
                   AND   [TransferStatusId] <> @TransferStatusId)
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -3  -- Orden duplicado
            RETURN
        END

        UPDATE [dbo].[FixedAssetTransferStatus] SET
            [StatusCode]    = UPPER(@StatusCode),
            [StatusName]    = UPPER(@StatusName),
            [Description]   = UPPER(@Description),
            [Order]         = @Order,
            [IsFinal]       = @IsFinal,
            [IsActive]      = @IsActive,
            [ModifiedDate]  = GETDATE(),
            [ModifiedBy]    = @ModifiedBy
        WHERE [TransferStatusId] = @TransferStatusId

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
-- SP_FixedAssetTransferStatus_Inactive
-- -------------------------------------------------------
CREATE OR ALTER PROCEDURE [dbo].[SP_FixedAssetTransferStatus_Inactive]
    @TransferStatusId   INT,
    @ModifiedBy         INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION
    BEGIN TRY

        IF NOT EXISTS (SELECT 1 FROM [dbo].[FixedAssetTransferStatus]
                       WHERE [TransferStatusId] = @TransferStatusId)
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -1  -- No existe
            RETURN
        END

        -- Verificar si tiene traslados asociados
        IF EXISTS (SELECT 1 FROM [dbo].[FixedAssetTransfers]
                   WHERE [TransferStatusId] = @TransferStatusId)
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -2  -- Tiene traslados, no se puede inactivar
            RETURN
        END

        UPDATE [dbo].[FixedAssetTransferStatus] SET
            [IsActive]      = 0,
            [ModifiedDate]  = GETDATE(),
            [ModifiedBy]    = @ModifiedBy
        WHERE [TransferStatusId] = @TransferStatusId

        COMMIT TRANSACTION
        SELECT 1

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        SELECT 0
    END CATCH
END
GO