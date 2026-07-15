-- =============================================
-- SP_AccountingEntryChecks_UpdateCheckId
-- 1 = éxito, -1 = CheckId antiguo no existe, 0 = error
-- =============================================
CREATE OR ALTER PROCEDURE SP_AccountingEntryChecks_UpdateCheckId
    @CheckIdAntiguo INT,
    @CheckIdNuevo   INT,
    @UpdatedBy      INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@UpdatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        UPDATE AccountingEntryChecks
        SET CheckId = @CheckIdNuevo
        WHERE CheckId = @CheckIdAntiguo;
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO