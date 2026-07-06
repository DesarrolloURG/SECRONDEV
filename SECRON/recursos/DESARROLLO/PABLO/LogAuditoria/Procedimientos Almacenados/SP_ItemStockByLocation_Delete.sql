-- DELETE físico, se mantiene tal cual (comportamiento original)
CREATE OR ALTER PROCEDURE SP_ItemStockByLocation_Delete
    @ItemStockLocationId INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;

    BEGIN TRANSACTION
    BEGIN TRY
        DELETE FROM ItemStockByLocation WHERE ItemStockLocationId = @ItemStockLocationId;
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO