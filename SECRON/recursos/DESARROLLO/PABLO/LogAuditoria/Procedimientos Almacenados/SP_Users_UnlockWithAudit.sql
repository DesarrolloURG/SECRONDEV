-- Corresponde a DesbloquearUsuario(int userId, int modifiedBy)
CREATE OR ALTER PROCEDURE SP_Users_UnlockWithAudit
    @UserId INT, @ModifiedBy INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        UPDATE Users SET IsLocked = 0, FailedLoginAttempts = 0,
            ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy
        WHERE UserId = @UserId;
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO