-- =============================================
-- SP_AccountingEntryChecks_Insert
-- =============================================
CREATE OR ALTER PROCEDURE SP_AccountingEntryChecks_Insert
    @EntryMasterId INT,
    @CheckId       INT,
    @CreatedBy     INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@CreatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        INSERT INTO AccountingEntryChecks (EntryMasterId, CheckId)
        VALUES (@EntryMasterId, @CheckId);

        COMMIT TRANSACTION; SELECT 1;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO