CREATE OR ALTER PROCEDURE SP_FixedAssetAttributeDefinitions_Insert
    @AssetCategoryId INT,
    @AttributeKey    VARCHAR(50),
    @AttributeLabel  VARCHAR(100),
    @DataType        VARCHAR(20),
    @IsRequired      BIT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION
    BEGIN TRY

        -- Validar que no exista la misma clave en la misma categoría
        IF EXISTS (SELECT 1 FROM FixedAssetAttributeDefinitions 
                   WHERE AssetCategoryId = @AssetCategoryId 
                   AND AttributeKey = UPPER(@AttributeKey))
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -1  -- Clave duplicada en la categoría
            RETURN
        END

        INSERT INTO FixedAssetAttributeDefinitions
            (AssetCategoryId, AttributeKey, AttributeLabel,
             DataType, IsRequired, IsActive, IsSystem)
        VALUES
            (@AssetCategoryId, UPPER(@AttributeKey), UPPER(@AttributeLabel),
             UPPER(@DataType), @IsRequired, 1, 0)

        COMMIT TRANSACTION
        SELECT 1

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        SELECT 0
    END CATCH
END
GO