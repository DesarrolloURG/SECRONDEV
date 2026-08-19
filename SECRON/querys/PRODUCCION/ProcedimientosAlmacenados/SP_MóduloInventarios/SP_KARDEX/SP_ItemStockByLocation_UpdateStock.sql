CREATE OR ALTER PROCEDURE SP_ItemStockByLocation_UpdateStock
    @ItemId INT, @LocationId INT, @CurrentStock DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;

    BEGIN TRANSACTION
    BEGIN TRY
        UPDATE ItemStockByLocation SET CurrentStock = @CurrentStock, LastMovementDate = GETDATE()
        WHERE ItemId = @ItemId AND LocationId = @LocationId;
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO