-- =============================================
-- SP_WarehouseManagerPermission_Insert
-- =============================================
CREATE OR ALTER PROCEDURE SP_WarehouseManagerPermission_Insert
    @WarehouseManagerId     INT,
    @WarehousePermissionId  INT,
    @MaxQuantityPerDispatch DECIMAL(18,2) = NULL,
    @CreatedBy              INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@CreatedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM WarehouseManagers WHERE WarehouseManagerId = @WarehouseManagerId AND IsActive = 1)
        BEGIN ROLLBACK TRANSACTION; SELECT -1; RETURN; END

        IF EXISTS (SELECT 1 FROM WarehouseManagerPermissions
                   WHERE WarehouseManagerId = @WarehouseManagerId
                     AND WarehousePermissionId = @WarehousePermissionId)
        BEGIN ROLLBACK TRANSACTION; SELECT -2; RETURN; END

        INSERT INTO WarehouseManagerPermissions
            (WarehouseManagerId, WarehousePermissionId, MaxQuantityPerDispatch, IsGranted, CreatedDate, CreatedBy)
        VALUES
            (@WarehouseManagerId, @WarehousePermissionId, @MaxQuantityPerDispatch, 1, GETDATE(), @CreatedBy);

        COMMIT TRANSACTION; SELECT 1;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO