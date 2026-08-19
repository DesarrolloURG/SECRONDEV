CREATE OR ALTER PROCEDURE SP_WarehouseManagerPermission_Insert
    @WarehouseManagerId      INT,
    @WarehousePermissionId   INT,
    @MaxQuantityPerDispatch  DECIMAL(18,2) = NULL,
    @CreatedBy               INT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRANSACTION
    BEGIN TRY
        IF NOT EXISTS (
            SELECT 1 FROM WarehouseManagers
            WHERE WarehouseManagerId = @WarehouseManagerId AND IsActive = 1
        )
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT -1;
            RETURN;
        END

        -- Bloquear si el dueño de la fila es SUPERADMIN, salvo que quien ejecuta también lo sea o tenga KARDEX_WAREHOUSE_ADMIN
        IF EXISTS (
            SELECT 1
            FROM WarehouseManagers wm
            INNER JOIN Users u ON u.UserId = wm.UserId
            INNER JOIN Roles r ON r.RoleId = u.RoleId
            WHERE wm.WarehouseManagerId = @WarehouseManagerId
              AND r.RoleName = 'SUPERADMIN'
        )
        AND NOT EXISTS (
            SELECT 1 FROM Users a, Roles b
            WHERE a.UserId = @CreatedBy AND a.RoleId = b.RoleId AND b.RoleName = 'SUPERADMIN'
        )
        AND NOT EXISTS (
            SELECT 1 FROM Users a, UserPermissions b, Permissions c
            WHERE a.UserId = @CreatedBy AND a.UserId = b.UserId
              AND b.PermissionId = c.PermissionId
              AND c.PermissionName = 'KARDEX_WAREHOUSE_ADMIN'
              AND b.IsGranted = 1
        )
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT -3;
            RETURN;
        END

        IF EXISTS (
            SELECT 1 FROM WarehouseManagerPermissions
            WHERE WarehouseManagerId = @WarehouseManagerId
              AND WarehousePermissionId = @WarehousePermissionId
              AND IsGranted = 1
        )
        BEGIN
            ROLLBACK TRANSACTION;
            SELECT -2;
            RETURN;
        END

        IF EXISTS (
            SELECT 1 FROM WarehouseManagerPermissions
            WHERE WarehouseManagerId = @WarehouseManagerId
              AND WarehousePermissionId = @WarehousePermissionId
              AND IsGranted = 0
        )
        BEGIN
            UPDATE WarehouseManagerPermissions
               SET IsGranted = 1,
                   MaxQuantityPerDispatch = @MaxQuantityPerDispatch
             WHERE WarehouseManagerId = @WarehouseManagerId
               AND WarehousePermissionId = @WarehousePermissionId;

            COMMIT TRANSACTION;
            SELECT 1;
            RETURN;
        END

        INSERT INTO WarehouseManagerPermissions
            (WarehouseManagerId, WarehousePermissionId, MaxQuantityPerDispatch, IsGranted, CreatedDate, CreatedBy)
        VALUES
            (@WarehouseManagerId, @WarehousePermissionId, @MaxQuantityPerDispatch, 1, GETDATE(), @CreatedBy);

        COMMIT TRANSACTION;
        SELECT 1;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SELECT 0;
    END CATCH
END
GO