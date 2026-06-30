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

        -- Bloquear si el dueño de la fila es SUPERADMIN, salvo que quien ejecuta también lo sea o tenga KARDEX_WAREHOUSE_ADMIN
        IF EXISTS (
            SELECT 1
            FROM WarehouseManagerPermissions wmp
            INNER JOIN WarehouseManagers wm ON wm.WarehouseManagerId = wmp.WarehouseManagerId
            INNER JOIN Users u ON u.UserId = wm.UserId
            INNER JOIN Roles r ON r.RoleId = u.RoleId
            WHERE wmp.WarehouseManagerPermissionId = @WarehouseManagerPermissionId
              AND r.RoleName = 'SUPERADMIN'
        )
        AND NOT EXISTS (
            SELECT 1 FROM Users a, Roles b
            WHERE a.UserId = @ModifiedBy AND a.RoleId = b.RoleId AND b.RoleName = 'SUPERADMIN'
        )
        AND NOT EXISTS (
            SELECT 1 FROM Users a, UserPermissions b, Permissions c
            WHERE a.UserId = @ModifiedBy AND a.UserId = b.UserId
              AND b.PermissionId = c.PermissionId
              AND c.PermissionName = 'KARDEX_WAREHOUSE_ADMIN'
              AND b.IsGranted = 1
        )
        BEGIN ROLLBACK TRANSACTION; SELECT -2; RETURN; END

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