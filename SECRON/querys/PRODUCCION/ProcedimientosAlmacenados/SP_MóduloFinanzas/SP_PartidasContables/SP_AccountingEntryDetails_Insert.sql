CREATE OR ALTER PROCEDURE SP_AccountingEntryDetails_Insert
    @EntryMasterId INT, @AccountId INT, @Debit DECIMAL(18,2),
    @Credit DECIMAL(18,2), @Remarks VARCHAR(255) = NULL, @CreatedBy INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@CreatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        INSERT INTO AccountingEntryDetails (EntryMasterId, AccountId, Debit, Credit, Remarks)
        VALUES (@EntryMasterId, @AccountId, @Debit, @Credit, @Remarks);
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO