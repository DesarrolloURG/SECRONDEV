CREATE OR ALTER PROCEDURE SP_AccountingEntryTransfers_UpdateTransferId
    @TransferIdAntiguo INT, @TransferIdNuevo INT, @UpdatedBy INT = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@UpdatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        UPDATE AccountingEntryTransfers
        SET TransferId = @TransferIdNuevo
        WHERE TransferId = @TransferIdAntiguo;
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO