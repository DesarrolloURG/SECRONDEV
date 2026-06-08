CREATE OR ALTER PROCEDURE SP_FixedAssetSubCategories_Update
    @SubCategoryId   INT,
    @SubCategoryCode VARCHAR(10),
    @SubCategoryName VARCHAR(200),
    @IsActive        BIT,
    @ModifiedBy      INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION
    BEGIN TRY

        IF NOT EXISTS (SELECT 1 FROM FixedAssetSubCategories WHERE SubCategoryId = @SubCategoryId)
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -1
            RETURN
        END

        DECLARE @AssetCategoryId INT
        SELECT @AssetCategoryId = AssetCategoryId
        FROM FixedAssetSubCategories WHERE SubCategoryId = @SubCategoryId

        IF EXISTS (SELECT 1 FROM FixedAssetSubCategories
                   WHERE AssetCategoryId = @AssetCategoryId
                     AND SubCategoryCode = UPPER(@SubCategoryCode)
                     AND SubCategoryId  != @SubCategoryId)
        BEGIN
            ROLLBACK TRANSACTION
            SELECT -2
            RETURN
        END

        UPDATE FixedAssetSubCategories SET
            SubCategoryCode = UPPER(@SubCategoryCode),
            SubCategoryName = UPPER(@SubCategoryName),
            IsActive        = @IsActive,
            ModifiedDate    = GETDATE(),
            ModifiedBy      = @ModifiedBy
        WHERE SubCategoryId = @SubCategoryId

        COMMIT TRANSACTION
        SELECT 1

    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION
        SELECT 0
    END CATCH
END
GO