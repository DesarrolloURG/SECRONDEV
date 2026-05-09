create or ALTER PROCEDURE SP_FixedAssetCategories_Insert
    @CategoryCode       VARCHAR(20),
    @CategoryName       VARCHAR(100),
    @Description        VARCHAR(255) = NULL,
    @DepreciationMethod VARCHAR(30),
    @DepreciationYears  DECIMAL(4,1),
    @AccountAccumDepId  INT,
    @AccountExpenseId   INT,
    @CreatedBy          INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRANSACTION
    BEGIN TRY

        -- Validar que no exista el mismo código
        IF EXISTS (SELECT 1 FROM FixedAssetCategories WHERE CategoryCode = UPPER(@CategoryCode))
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -1  -- Código duplicado
            RETURN
        END

        INSERT INTO FixedAssetCategories
            (CategoryCode, CategoryName, Description,
             DepreciationMethod, DepreciationYears,
             AccountAccumDepId, AccountExpenseId,
             IsActive, CreatedDate, CreatedBy)
        VALUES
            (UPPER(@CategoryCode), UPPER(@CategoryName), UPPER(@Description),
             @DepreciationMethod, @DepreciationYears,
             @AccountAccumDepId, @AccountExpenseId,
             1, GETDATE(), @CreatedBy)

        COMMIT TRANSACTION
        SELECT 1

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        SELECT 0
    END CATCH
END
GO