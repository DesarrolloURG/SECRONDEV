CREATE OR ALTER PROCEDURE SP_ItemSubCategories_Insert
    @CategoryId INT, @SubCategoryCode VARCHAR(20), @SubCategoryName VARCHAR(150), @CreatedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@CreatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    IF EXISTS (SELECT 1 FROM ItemSubCategories
               WHERE CategoryId = @CategoryId AND SubCategoryCode = @SubCategoryCode)
    BEGIN SELECT -1; RETURN; END

    BEGIN TRANSACTION
    BEGIN TRY
        INSERT INTO ItemSubCategories (CategoryId, SubCategoryCode, SubCategoryName, IsActive, CreatedBy)
        VALUES (@CategoryId, @SubCategoryCode, @SubCategoryName, 1, @CreatedBy);

        COMMIT TRANSACTION; SELECT 1;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO