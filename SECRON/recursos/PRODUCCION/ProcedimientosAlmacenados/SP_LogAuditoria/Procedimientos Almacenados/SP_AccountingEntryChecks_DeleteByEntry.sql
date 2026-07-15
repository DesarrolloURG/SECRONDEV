-- =============================================
-- SP_AccountingEntryChecks_DeleteByEntry
-- =============================================
CREATE OR ALTER PROCEDURE SP_AccountingEntryChecks_DeleteByEntry
    @EntryMasterId INT,
    @DeletedBy     INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@DeletedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        DELETE FROM AccountingEntryChecks WHERE EntryMasterId = @EntryMasterId;
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO