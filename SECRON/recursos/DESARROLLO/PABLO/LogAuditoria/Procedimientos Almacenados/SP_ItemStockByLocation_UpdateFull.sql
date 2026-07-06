CREATE OR ALTER PROCEDURE SP_ItemStockByLocation_UpdateFull
    @ItemStockLocationId INT, @CurrentStock DECIMAL(18,2),
    @MinimumStock DECIMAL(18,2), @MaximumStock DECIMAL(18,2) = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;

    BEGIN TRANSACTION
    BEGIN TRY
        UPDATE ItemStockByLocation SET
            CurrentStock = @CurrentStock, MinimumStock = @MinimumStock,
            MaximumStock = @MaximumStock, LastMovementDate = GETDATE()
        WHERE ItemStockLocationId = @ItemStockLocationId;
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO