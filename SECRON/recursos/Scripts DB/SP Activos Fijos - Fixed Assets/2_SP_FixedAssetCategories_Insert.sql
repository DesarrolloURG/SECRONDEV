create or ALTER PROCEDURE SP_FixedAssetCategories_Insert
    @CategoryCode       VARCHAR(20),
    @CategoryName       VARCHAR(100),
    @Description        VARCHAR(255) = NULL,
    @DepreciationMethod VARCHAR(30),
    @DepreciationYears  DECIMAL(4,1),
    @AccountAccumDepId  INT,
    @AccountExpenseId   INT,
    @IsTangible         BIT,
    @CreatedBy          INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION
    BEGIN TRY

        IF EXISTS (SELECT 1 FROM FixedAssetCategories WHERE CategoryCode = UPPER(@CategoryCode))
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -1
            RETURN
        END

        INSERT INTO FixedAssetCategories
            (CategoryCode, CategoryName, Description,
             DepreciationMethod, DepreciationYears,
             AccountAccumDepId, AccountExpenseId,
             IsTangible, IsActive, CreatedBy)
        VALUES
            (UPPER(@CategoryCode), UPPER(@CategoryName), UPPER(@Description),
             @DepreciationMethod, @DepreciationYears,
             @AccountAccumDepId, @AccountExpenseId,
             @IsTangible, 1, @CreatedBy)

        DECLARE @NewCategoryId INT = SCOPE_IDENTITY()
        DECLARE @AttrDefId INT

        EXEC SP_FA_ObtenerOCrearAtributoSistema @NewCategoryId, 'BRAND',  'MARCA',  @AttrDefId OUTPUT
        SET @AttrDefId = NULL

        EXEC SP_FA_ObtenerOCrearAtributoSistema @NewCategoryId, 'MODEL',  'MODELO', @AttrDefId OUTPUT
        SET @AttrDefId = NULL

        EXEC SP_FA_ObtenerOCrearAtributoSistema @NewCategoryId, 'SERIAL', 'SERIE',  @AttrDefId OUTPUT
        SET @AttrDefId = NULL

        COMMIT TRANSACTION
        SELECT 1

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        SELECT 0
    END CATCH
END
GO
