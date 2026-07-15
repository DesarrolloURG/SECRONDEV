CREATE OR ALTER PROCEDURE SP_LocationCategories_Update
    @LocationCategoryId INT, @IsInactivation BIT,
    @CategoryCode VARCHAR(20) = NULL, @CategoryName VARCHAR(150) = NULL, @Description VARCHAR(255) = NULL,
    @ModifiedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        IF @IsInactivation = 1
            UPDATE LocationCategories SET IsActive = 0 WHERE LocationCategoryId = @LocationCategoryId;
        ELSE
            UPDATE LocationCategories SET CategoryCode = @CategoryCode, CategoryName = @CategoryName,
                Description = @Description
            WHERE LocationCategoryId = @LocationCategoryId;

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO