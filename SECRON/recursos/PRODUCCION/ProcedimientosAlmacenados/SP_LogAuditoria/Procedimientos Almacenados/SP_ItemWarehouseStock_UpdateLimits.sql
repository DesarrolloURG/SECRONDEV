CREATE OR ALTER PROCEDURE SP_ItemWarehouseStock_UpdateLimits
    @ItemWarehouseStockId INT, @MinimumStock DECIMAL(18,2), @MaximumStock DECIMAL(18,2), @ReorderPoint DECIMAL(18,2),
    @ModifiedBy INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        UPDATE ItemWarehouseStock SET
            MinimumStock = @MinimumStock, MaximumStock = @MaximumStock, ReorderPoint = @ReorderPoint
        WHERE ItemWarehouseStockId = @ItemWarehouseStockId;
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO