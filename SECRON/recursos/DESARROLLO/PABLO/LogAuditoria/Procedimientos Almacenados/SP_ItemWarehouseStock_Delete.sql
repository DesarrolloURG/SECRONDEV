CREATE OR ALTER PROCEDURE SP_ItemWarehouseStock_Delete
    @ItemWarehouseStockId INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;

    BEGIN TRANSACTION
    BEGIN TRY
        DELETE FROM ItemWarehouseStock WHERE ItemWarehouseStockId = @ItemWarehouseStockId;
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO