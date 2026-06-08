CREATE OR ALTER PROCEDURE SP_FixedAssetSubCategories_Insert
    @AssetCategoryId INT,
    @SubCategoryCode VARCHAR(10),
    @SubCategoryName VARCHAR(200),
    @CreatedBy       INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION
    BEGIN TRY

        IF NOT EXISTS (SELECT 1 FROM FixedAssetCategories
                       WHERE AssetCategoryId = @AssetCategoryId AND IsActive = 1)
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -2
            RETURN
        END

        IF EXISTS (SELECT 1 FROM FixedAssetSubCategories
                   WHERE AssetCategoryId = @AssetCategoryId
                     AND SubCategoryCode = UPPER(@SubCategoryCode))
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -1
            RETURN
        END

        INSERT INTO FixedAssetSubCategories
            (AssetCategoryId, SubCategoryCode, SubCategoryName, IsActive, CreatedBy)
        VALUES
            (@AssetCategoryId, UPPER(@SubCategoryCode), UPPER(@SubCategoryName), 1, @CreatedBy)

        COMMIT TRANSACTION
        SELECT 1

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        SELECT 0
    END CATCH
END
GO