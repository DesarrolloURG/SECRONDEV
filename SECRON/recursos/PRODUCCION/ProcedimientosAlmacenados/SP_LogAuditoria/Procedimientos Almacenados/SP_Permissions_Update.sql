-- @IsInactivation = 1 => solo IsActive=0 (InactivarPermiso)
-- @IsInactivation = 0 => update normal (ActualizarPermiso)
CREATE OR ALTER PROCEDURE SP_Permissions_Update
    @PermissionId INT, @IsInactivation BIT,
    @PermissionCode VARCHAR(50) = NULL, @PermissionName VARCHAR(150) = NULL,
    @Description VARCHAR(255) = NULL, @ModuleName VARCHAR(100) = NULL, @ActionType VARCHAR(50) = NULL,
    @ModifiedBy INT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    DECLARE @ctx BINARY(128) = CAST(CONVERT(BINARY(4), ISNULL(@ModifiedBy, 0)) AS BINARY(128));
    SET CONTEXT_INFO @ctx;

    BEGIN TRANSACTION
    BEGIN TRY
        IF @IsInactivation = 1
            UPDATE Permissions SET IsActive = 0 WHERE PermissionId = @PermissionId;
        ELSE
            UPDATE Permissions SET PermissionCode = @PermissionCode, PermissionName = @PermissionName,
                Description = @Description, ModuleName = @ModuleName, ActionType = @ActionType
            WHERE PermissionId = @PermissionId;

        DECLARE @rows INT = @@ROWCOUNT;
        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO