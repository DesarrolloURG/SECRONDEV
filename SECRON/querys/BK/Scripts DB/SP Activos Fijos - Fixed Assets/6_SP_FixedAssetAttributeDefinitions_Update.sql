CREATE OR ALTER PROCEDURE SP_FixedAssetAttributeDefinitions_Update
    @AttributeDefId  INT,
    @AttributeKey    VARCHAR(50),
    @AttributeLabel  VARCHAR(100),
    @DataType        VARCHAR(20),
    @IsRequired      BIT,
    @IsActive        BIT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION
    BEGIN TRY

        -- Validar que el registro exista
        IF NOT EXISTS (SELECT 1 FROM FixedAssetAttributeDefinitions 
                       WHERE AttributeDefId = @AttributeDefId)
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -1  -- Registro no encontrado
            RETURN
        END

        -- Validar clave duplicada en la misma categoría excluyendo el registro actual
        IF EXISTS (SELECT 1 FROM FixedAssetAttributeDefinitions 
                   WHERE AttributeKey    = UPPER(@AttributeKey)
                   AND   AssetCategoryId = (SELECT AssetCategoryId FROM FixedAssetAttributeDefinitions 
                                            WHERE AttributeDefId = @AttributeDefId)
                   AND   AttributeDefId != @AttributeDefId)
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -2  -- Clave duplicada en otro registro
            RETURN
        END

        UPDATE FixedAssetAttributeDefinitions SET
            AttributeKey   = UPPER(@AttributeKey),
            AttributeLabel = UPPER(@AttributeLabel),
            DataType       = @DataType,
            IsRequired     = @IsRequired,
            IsActive       = @IsActive
        WHERE AttributeDefId = @AttributeDefId

        COMMIT TRANSACTION
        SELECT 1

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        SELECT 0
    END CATCH
END
GO