CREATE OR ALTER PROCEDURE SP_Transfers_MarkLastComplement
    @TransferNumber VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;

    BEGIN TRANSACTION
    BEGIN TRY
        UPDATE Transfers SET LastComplement = 1 WHERE TransferNumber = @TransferNumber AND IsActive = 1;
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO