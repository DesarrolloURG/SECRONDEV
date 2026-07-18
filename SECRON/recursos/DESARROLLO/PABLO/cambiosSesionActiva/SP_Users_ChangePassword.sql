CREATE OR ALTER PROCEDURE SP_Users_ChangePassword
    @UserId INT, @PasswordHash VARCHAR(255), @IsTemporaryPassword BIT, @PasswordExpiryDate DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@UserId, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;
    BEGIN TRANSACTION
    BEGIN TRY
        UPDATE Users SET PasswordHash = @PasswordHash, IsTemporaryPassword = @IsTemporaryPassword,
            PasswordExpiryDate = @PasswordExpiryDate, LastPasswordChanged = GETDATE(), ModifiedDate = GETDATE()
        WHERE UserId = @UserId;
        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO