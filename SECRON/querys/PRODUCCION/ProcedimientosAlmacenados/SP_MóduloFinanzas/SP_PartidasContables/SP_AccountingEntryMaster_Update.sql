CREATE OR ALTER PROCEDURE SP_AccountingEntryMaster_Update
    @EntryMasterId INT, @EntryDate DATETIME, @Concept VARCHAR(255),
    @StatusId INT, @TotalAmount DECIMAL(18,2), @ModifiedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        UPDATE AccountingEntryMaster
        SET EntryDate = @EntryDate, Concept = @Concept, StatusId = @StatusId,
            TotalAmount = @TotalAmount, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy
        WHERE EntryMasterId = @EntryMasterId;
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO