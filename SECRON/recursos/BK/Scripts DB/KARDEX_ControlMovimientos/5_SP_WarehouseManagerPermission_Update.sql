-- =============================================
-- SP_WarehouseManagerPermission_Update
-- =============================================
CREATE OR ALTER PROCEDURE SP_WarehouseManagerPermission_Update
    @WarehouseManagerPermissionId INT,
    @IsGranted                    BIT,
    @MaxQuantityPerDispatch       DECIMAL(18,2) = NULL,
    @ModifiedBy                   INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        IF NOT EXISTS (SELECT 1 FROM WarehouseManagerPermissions WHERE WarehouseManagerPermissionId = @WarehouseManagerPermissionId)
        BEGIN ROLLBACK TRANSACTION; SELECT -1; RETURN; END

        UPDATE WarehouseManagerPermissions
           SET IsGranted               = @IsGranted,
               MaxQuantityPerDispatch  = @MaxQuantityPerDispatch
         WHERE WarehouseManagerPermissionId = @WarehouseManagerPermissionId;

        COMMIT TRANSACTION; SELECT 1;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO