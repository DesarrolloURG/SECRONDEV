-- Usado por ObtenerOCrearStock cuando no existe registro
CREATE OR ALTER PROCEDURE SP_ItemStockByLocation_GetOrCreate
    @ItemId INT, @LocationId INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;

    IF EXISTS (SELECT 1 FROM ItemStockByLocation WHERE ItemId = @ItemId AND LocationId = @LocationId)
    BEGIN
        SELECT * FROM ItemStockByLocation WHERE ItemId = @ItemId AND LocationId = @LocationId;
        RETURN;
    END

    BEGIN TRANSACTION
    BEGIN TRY
        INSERT INTO ItemStockByLocation (ItemId, LocationId, CurrentStock, ReservedStock, MinimumStock, IsActive)
        VALUES (@ItemId, @LocationId, 0, 0, 0, 1);

        DECLARE @NewId INT = CAST(SCOPE_IDENTITY() AS INT);
        COMMIT TRANSACTION;

        SELECT * FROM ItemStockByLocation WHERE ItemStockLocationId = @NewId;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
    END CATCH
END
GO