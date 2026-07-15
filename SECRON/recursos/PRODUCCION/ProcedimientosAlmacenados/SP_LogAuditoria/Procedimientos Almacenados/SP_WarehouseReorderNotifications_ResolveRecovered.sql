CREATE OR ALTER PROCEDURE SP_WarehouseReorderNotifications_ResolveRecovered
    @WarehouseId INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;

    BEGIN TRANSACTION
    BEGIN TRY
        UPDATE wrn
        SET ResolvedDate = GETDATE()
        FROM WarehouseReorderNotifications wrn
        INNER JOIN ItemWarehouseStock s ON s.ItemId = wrn.ItemId AND s.WarehouseId = wrn.WarehouseId
        WHERE wrn.WarehouseId = @WarehouseId
          AND wrn.ResolvedDate IS NULL
          AND (s.ReorderPoint IS NULL OR s.CurrentStock > s.ReorderPoint);
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO