CREATE OR ALTER PROCEDURE SP_Checks_UpdateFileControl
    @CheckId INT, @FileControl VARCHAR(20), @UserId INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@UserId, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        UPDATE Checks SET FileControl = @FileControl, ModifiedDate = GETDATE(),
            ModifiedBy = NULLIF(@UserId, 0)
        WHERE CheckId = @CheckId;
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO