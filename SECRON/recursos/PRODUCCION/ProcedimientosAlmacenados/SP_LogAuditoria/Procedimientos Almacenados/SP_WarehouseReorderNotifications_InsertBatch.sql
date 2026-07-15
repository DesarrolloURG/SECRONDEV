CREATE OR ALTER PROCEDURE SP_WarehouseReorderNotifications_InsertBatch
    @WarehouseId INT, @ItemIdsJson NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;

    BEGIN TRANSACTION
    BEGIN TRY
        INSERT INTO WarehouseReorderNotifications (WarehouseId, ItemId, NotifiedDate)
        SELECT @WarehouseId, Id, GETDATE()
        FROM OPENJSON(@ItemIdsJson) WITH (Id INT '$');

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO