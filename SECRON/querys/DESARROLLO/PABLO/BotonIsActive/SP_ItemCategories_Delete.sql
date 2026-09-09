CREATE OR ALTER PROCEDURE SP_ItemCategories_Delete
    @CategoryId INT,
    @IsActive   BIT,
    @ModifiedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY

        IF NOT EXISTS (SELECT 1 FROM ItemCategories WHERE CategoryId = @CategoryId)
        BEGIN ROLLBACK TRANSACTION; SELECT -1; RETURN; END

        UPDATE ItemCategories SET
            IsActive     = @IsActive,
            ModifiedDate = GETDATE(),
            ModifiedBy   = @ModifiedBy
        WHERE CategoryId = @CategoryId

        COMMIT TRANSACTION; SELECT 1;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END