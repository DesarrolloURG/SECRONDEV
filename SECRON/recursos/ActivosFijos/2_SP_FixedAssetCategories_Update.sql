CREATE OR ALTER PROCEDURE SP_FixedAssetCategories_Update
    @AssetCategoryId    INT,
    @CategoryCode       VARCHAR(20),
    @CategoryName       VARCHAR(100),
    @Description        VARCHAR(255) = NULL,
    @DepreciationMethod VARCHAR(30),
    @DepreciationYears  DECIMAL(4,1),
    @AccountAccumDepId  INT,
    @AccountExpenseId   INT,
    @IsActive           BIT,
    @ModifiedBy         INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION
    BEGIN TRY

        -- Validar que el registro exista
        IF NOT EXISTS (SELECT 1 FROM FixedAssetCategories WHERE AssetCategoryId = @AssetCategoryId)
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -1  -- Registro no encontrado
            RETURN
        END

        -- Validar código duplicado excluyendo el registro actual
        IF EXISTS (SELECT 1 FROM FixedAssetCategories 
                   WHERE CategoryCode = UPPER(@CategoryCode) 
                   AND AssetCategoryId != @AssetCategoryId)
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -2  -- Código duplicado en otro registro
            RETURN
        END

        UPDATE FixedAssetCategories SET
            CategoryCode       = UPPER(@CategoryCode),
            CategoryName       = UPPER(@CategoryName),
            Description        = UPPER(@Description),
            DepreciationMethod = @DepreciationMethod,
            DepreciationYears  = @DepreciationYears,
            AccountAccumDepId  = @AccountAccumDepId,
            AccountExpenseId   = @AccountExpenseId,
            IsActive           = @IsActive,
            ModifiedDate       = GETDATE(),
            ModifiedBy         = @ModifiedBy
        WHERE AssetCategoryId  = @AssetCategoryId

        COMMIT TRANSACTION
        SELECT 1

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        SELECT 0
    END CATCH
END
GO