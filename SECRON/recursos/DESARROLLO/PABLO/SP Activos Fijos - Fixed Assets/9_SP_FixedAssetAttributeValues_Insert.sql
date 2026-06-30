-- ============================================================
-- SP_FixedAssetAttributeValues_Insert
-- ============================================================
CREATE OR ALTER PROCEDURE SP_FixedAssetAttributeValues_Insert
    @AssetId        INT,
    @AttributeDefId INT,
    @Value          NVARCHAR(500) = NULL,
    @CreatedBy      INT           = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION
    BEGIN TRY

        -- Validar que el activo exista
        IF NOT EXISTS (SELECT 1 FROM FixedAssets WHERE AssetId = @AssetId)
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -1  -- Activo no encontrado
            RETURN
        END

        -- Validar que la definición de atributo exista y esté activa
        IF NOT EXISTS (SELECT 1 FROM FixedAssetAttributeDefinitions WHERE AttributeDefId = @AttributeDefId AND IsActive = 1)
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -2  -- Atributo no válido
            RETURN
        END

        -- Si ya existe el valor para este activo+atributo, actualizar en lugar de insertar
        IF EXISTS (SELECT 1 FROM FixedAssetAttributeValues WHERE AssetId = @AssetId AND AttributeDefId = @AttributeDefId)
        BEGIN
            UPDATE FixedAssetAttributeValues SET
                Value        = UPPER(@Value),
                ModifiedDate = GETDATE(),
                ModifiedBy   = @CreatedBy
            WHERE AssetId = @AssetId AND AttributeDefId = @AttributeDefId

            COMMIT TRANSACTION
            SELECT 1
            RETURN
        END

        INSERT INTO FixedAssetAttributeValues
            (AssetId, AttributeDefId, Value, CreatedBy)
        VALUES
            (@AssetId, @AttributeDefId, UPPER(@Value), @CreatedBy)

        COMMIT TRANSACTION
        SELECT 1

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        SELECT 0
    END CATCH
END
GO