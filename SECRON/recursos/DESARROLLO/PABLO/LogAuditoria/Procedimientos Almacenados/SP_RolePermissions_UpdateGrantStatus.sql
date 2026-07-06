-- Ninguno de los métodos originales recibe usuario, por eso no lleva @ctx
CREATE OR ALTER PROCEDURE SP_RolePermissions_UpdateGrantStatus
    @RolePermissionId INT, @IsGranted BIT
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;

    BEGIN TRANSACTION
    BEGIN TRY
        UPDATE RolePermissions SET IsGranted = @IsGranted WHERE RolePermissionId = @RolePermissionId;
        DECLARE @rows INT = @@ROWCOUNT;

        COMMIT TRANSACTION; SELECT @rows;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION; SELECT 0;
    END CATCH
END
GO