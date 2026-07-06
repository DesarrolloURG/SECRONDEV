CREATE OR ALTER PROCEDURE SP_ItemSubCategories_Update
    @SubCategoryId INT, @SubCategoryName VARCHAR(150), @IsActive BIT, @ModifiedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    IF NOT EXISTS (SELECT 1 FROM ItemSubCategories WHERE SubCategoryId = @SubCategoryId)
    BEGIN SELECT -2; RETURN; END

    BEGIN TRANSACTION
    BEGIN TRY
        UPDATE ItemSubCategories SET
            SubCategoryName = @SubCategoryName, IsActive = @IsActive,
            ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy
        WHERE SubCategoryId = @SubCategoryId;

        COMMIT TRANSACTION; SELECT 1;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO