CREATE OR ALTER PROCEDURE SP_ItemWarehouseStock_UpdateFull
    @ItemWarehouseStockId INT, @CurrentStock DECIMAL(18,2),
    @MinimumStock DECIMAL(18,2), @MaximumStock DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;

    BEGIN TRANSACTION
    BEGIN TRY
        UPDATE ItemWarehouseStock SET
            CurrentStock = @CurrentStock, MinimumStock = @MinimumStock,
            MaximumStock = @MaximumStock, LastMovementDate = GETDATE()
        WHERE ItemWarehouseStockId = @ItemWarehouseStockId;
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO