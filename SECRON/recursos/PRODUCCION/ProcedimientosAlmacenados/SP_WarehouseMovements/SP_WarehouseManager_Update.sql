-- =============================================
-- SP_WarehouseManager_Update
-- =============================================
CREATE OR ALTER PROCEDURE SP_WarehouseManager_Update
    @WarehouseManagerId INT,
    @UserId             INT,
    @ModifiedBy         INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        DECLARE @WarehouseId INT;
        SELECT @WarehouseId = WarehouseId FROM WarehouseManagers WHERE WarehouseManagerId = @WarehouseManagerId;

        IF @WarehouseId IS NULL
        BEGIN ROLLBACK TRANSACTION; SELECT -1; RETURN; END

        IF EXISTS (SELECT 1 FROM WarehouseManagers
                   WHERE WarehouseId = @WarehouseId AND UserId = @UserId
                     AND WarehouseManagerId <> @WarehouseManagerId)
        BEGIN ROLLBACK TRANSACTION; SELECT -2; RETURN; END

        UPDATE WarehouseManagers
           SET UserId       = @UserId,
               ModifiedDate = GETDATE(),
               ModifiedBy   = @ModifiedBy
         WHERE WarehouseManagerId = @WarehouseManagerId;

        COMMIT TRANSACTION; SELECT 1;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO