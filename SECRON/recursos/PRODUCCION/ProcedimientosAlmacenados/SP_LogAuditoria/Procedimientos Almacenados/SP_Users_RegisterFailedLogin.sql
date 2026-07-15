-- No recibe usuario en la firma original: sin @ctx
CREATE OR ALTER PROCEDURE SP_Users_RegisterFailedLogin
    @Username VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;

    BEGIN TRANSACTION
    BEGIN TRY
        UPDATE Users SET FailedLoginAttempts = FailedLoginAttempts + 1,
            IsLocked = CASE WHEN FailedLoginAttempts >= 4 THEN 1 ELSE 0 END
        WHERE Username = @Username;
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO