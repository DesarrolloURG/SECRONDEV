-- ============================================================
-- SP_FA_ObtenerOCrearAtributoSistema
-- ============================================================
CREATE OR ALTER PROCEDURE SP_FA_ObtenerOCrearAtributoSistema
    @AssetCategoryId INT,
    @AttrKey         VARCHAR(50),
    @AttrLabel       VARCHAR(100),
    @AttributeDefId  INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
 
    SELECT @AttributeDefId = AttributeDefId
    FROM   FixedAssetAttributeDefinitions
    WHERE  AssetCategoryId = @AssetCategoryId
    AND    AttributeKey    = @AttrKey
    AND    IsSystem        = 1
 
    IF @AttributeDefId IS NULL
    BEGIN
        INSERT INTO FixedAssetAttributeDefinitions
            (AssetCategoryId, AttributeKey, AttributeLabel, DataType, IsRequired, IsActive, IsSystem)
        VALUES
            (@AssetCategoryId, UPPER(@AttrKey), UPPER(@AttrLabel), 'TEXTO', 1, 1, 1)
 
        SET @AttributeDefId = SCOPE_IDENTITY()
    END
END
GO