CREATE OR ALTER PROCEDURE SP_ItemStockByLocation_Upsert
    @ItemId INT, @LocationId INT, @CurrentStock DECIMAL(18,2), @ReservedStock DECIMAL(18,2),
    @MinimumStock DECIMAL(18,2), @MaximumStock DECIMAL(18,2) = NULL
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;

    BEGIN TRANSACTION
    BEGIN TRY
        IF EXISTS (SELECT 1 FROM ItemStockByLocation WHERE ItemId = @ItemId AND LocationId = @LocationId)
            UPDATE ItemStockByLocation SET
                CurrentStock = @CurrentStock, ReservedStock = @ReservedStock,
                MinimumStock = @MinimumStock, MaximumStock = @MaximumStock,
                IsActive = 1, LastMovementDate = GETDATE()
            WHERE ItemId = @ItemId AND LocationId = @LocationId;
        ELSE
            INSERT INTO ItemStockByLocation
                (ItemId, LocationId, CurrentStock, ReservedStock, MinimumStock, MaximumStock, IsActive)
            VALUES
                (@ItemId, @LocationId, @CurrentStock, @ReservedStock, @MinimumStock, @MaximumStock, 1);

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO