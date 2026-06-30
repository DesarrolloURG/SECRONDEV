-- ============================================================
-- SP_FixedAssetAttributeValues_Update
-- ============================================================
CREATE OR ALTER PROCEDURE SP_FixedAssetAttributeValues_Update
    @AttributeValueId INT,
    @Value            NVARCHAR(500) = NULL,
    @ModifiedBy       INT           = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION
    BEGIN TRY

        IF NOT EXISTS (SELECT 1 FROM FixedAssetAttributeValues WHERE AttributeValueId = @AttributeValueId)
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -1  -- Registro no encontrado
            RETURN
        END

        UPDATE FixedAssetAttributeValues SET
            Value        = UPPER(@Value),
            ModifiedDate = GETDATE(),
            ModifiedBy   = @ModifiedBy
        WHERE AttributeValueId = @AttributeValueId

        COMMIT TRANSACTION
        SELECT 1

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        SELECT 0
    END CATCH
END
GO